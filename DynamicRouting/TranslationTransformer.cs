using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicRouting
{
    public class TranslationTransformer : DynamicRouteValueTransformer
    {
        private readonly TranslationDatabase _translationDatabase;

        public TranslationTransformer(TranslationDatabase translationDatabase)
        {
            _translationDatabase = translationDatabase;
        }

        public override ValueTask<RouteValueDictionary> TransformAsync(HttpContext httpContext, RouteValueDictionary values)
        {
            if (!values.ContainsKey("language") || !values.ContainsKey("controller") || !values.ContainsKey("action")) return ValueTask.FromResult(values);

            var language = (string)values["language"];
            var controller =  _translationDatabase.Resolve(language, (string)values["controller"]);
            if (controller == null) return ValueTask.FromResult(values);
            values["controller"] = controller;

            var action =  _translationDatabase.Resolve(language, (string)values["action"]);
            if (action == null) return ValueTask.FromResult(values);
            values["action"] = action;

            return ValueTask.FromResult(values);
        }
    }
}
