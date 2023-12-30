using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Json;

namespace VideoStreamer.API.Helpers
{
    public class KeycloakClaimsTransformer : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal.Identity == null) return Task.FromResult(principal);

            ClaimsIdentity claimsIdentity = (ClaimsIdentity)principal.Identity;

            if (claimsIdentity.IsAuthenticated && claimsIdentity.HasClaim((claim) => claim.Type == "realm_access"))
            {
                var realmAccessClaim = claimsIdentity.FindFirst((claim) => claim.Type == "realm_access");
                var realmAccessAsDict = JsonSerializer.Deserialize<Dictionary<string, string[]>>(realmAccessClaim!.Value);

                if (realmAccessAsDict != null && realmAccessAsDict["roles"] != null)
                {
                    foreach (var role in realmAccessAsDict["roles"])
                    {
                        claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
                    }
                }
            }

            return Task.FromResult(principal);
        }
    }
}
