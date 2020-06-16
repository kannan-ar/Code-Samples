namespace PurchaseHub.Filters
{
    using System;
    using Microsoft.AspNetCore.Mvc.Filters;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Primitives;
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Http;
    using System.Security.Principal;
    using System.Security.Cryptography;
    using System.Text;
    using System.IO;
    using Microsoft.AspNetCore.Http.Extensions;
    using Microsoft.Extensions.Caching.Distributed;
    using Microsoft.Extensions.DependencyInjection;
    using System.IO.Pipelines;
    using System.Buffers;

    public class HMACFilter : Attribute, IAsyncActionFilter
    {
        private const string authHeaderName = "Authorization";
        private readonly UInt64 requestMaxAgeInSeconds = 300;  //5 mins
        private static Dictionary<string, string> allowedApps = new Dictionary<string, string>();
        public HMACFilter()
        {
            if (allowedApps.Count == 0)
            {
                allowedApps.Add("4d53bce03ec34c0a911182d4c228ee6c", "A93reRTUJHsCuQSHR+L3GxqOJyDmQpCgps102ciuabc=");
            }
        }

        private string[] GetAutherizationHeaderValues(string rawAuthzHeader)
        {
            var credArray = rawAuthzHeader.Split(':');

            if (credArray.Length == 4)
            {
                return credArray;
            }
            else
            {
                return null;
            }
        }

        private async Task<bool> IsValidRequest(HttpContext context, IDistributedCache memoryCache, string APPId, string incomingBase64Signature, string nonce, string requestTimeStamp)
        {
            HttpRequest req = context.Request;
            string requestContentBase64String = "";
            string requestUri = System.Web.HttpUtility.UrlEncode(req.GetEncodedUrl().ToLower());
            string requestHttpMethod = req.Method;

            if (!allowedApps.ContainsKey(APPId))
            {
                return false;
            }

            var sharedKey = allowedApps[APPId];

            if (await IsReplayRequest(req, memoryCache, nonce, requestTimeStamp))
            {
                return false;
            }

            requestContentBase64String = Convert.ToString(context.Items["RequestHash"]);

            string data = String.Format("{0}{1}{2}{3}{4}{5}", APPId, requestHttpMethod, requestUri, requestTimeStamp, nonce, requestContentBase64String);

            var secretKeyBytes = Convert.FromBase64String(sharedKey);

            byte[] signature = Encoding.UTF8.GetBytes(data);

            using (HMACSHA256 hmac = new HMACSHA256(secretKeyBytes))
            {
                byte[] signatureBytes = hmac.ComputeHash(signature);

                return (incomingBase64Signature.Equals(Convert.ToBase64String(signatureBytes), StringComparison.Ordinal));
            }
        }

        private async Task<bool> IsReplayRequest(HttpRequest req, IDistributedCache memoryCache, string nonce, string requestTimeStamp)
        {
            var nonceCacheValue = await memoryCache.GetAsync(nonce);
            if (nonceCacheValue != null)
            {
                return true;
            }

            DateTime epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan currentTs = DateTime.UtcNow - epochStart;

            var serverTotalSeconds = Convert.ToUInt64(currentTs.TotalSeconds);
            var requestTotalSeconds = Convert.ToUInt64(requestTimeStamp);

            if ((serverTotalSeconds - requestTotalSeconds) > requestMaxAgeInSeconds)
            {
                return true;
            }

            await memoryCache.SetAsync(nonce, Encoding.UTF8.GetBytes(requestTimeStamp), new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.UtcNow.AddSeconds(requestMaxAgeInSeconds)
            });

            return false;
        }
        
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            StringValues authHeaderValue;

            if (!context.HttpContext.Request.Headers.TryGetValue(authHeaderName, out authHeaderValue) || authHeaderValue.Count == 0)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var authValue = authHeaderValue[0];

            if (!authValue.StartsWith("amx"))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            authValue = authValue.Replace("amx ", string.Empty);

            var autherizationHeaderArray = GetAutherizationHeaderValues(authValue);
            var memoryCache = context.HttpContext.RequestServices.GetService<IDistributedCache>();

            if (autherizationHeaderArray != null)
            {
                var isValid = await IsValidRequest(
                    context.HttpContext,
                    memoryCache,
                    autherizationHeaderArray[0],  //APPId 
                    autherizationHeaderArray[1],  //incomingBase64Signature
                    autherizationHeaderArray[2],  //nonce
                    autherizationHeaderArray[3]); //requestTimeStamp

                if (isValid)
                {
                    context.HttpContext.User = new GenericPrincipal(new GenericIdentity(autherizationHeaderArray[0]), null); ;
                }
                else
                {
                    context.Result = new UnauthorizedObjectResult(authHeaderValue[0]);
                    return;
                }
            }
            else
            {
                context.Result = new UnauthorizedObjectResult(authHeaderValue[0]);
                return;
            }

            await next();
        }
    }
}