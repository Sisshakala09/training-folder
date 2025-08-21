using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TransportManagement.CustomeFilters
{
    public class SimpleCacheFilter : IResourceFilter

    {

        private static readonly Dictionary<string, IActionResult> _cache = new();


        public void OnResourceExecuting(ResourceExecutingContext context)

        {

            var key = context.HttpContext.Request.Path.ToString();

            if (_cache.ContainsKey(key))

            {

                context.Result = _cache[key]; // short-circuit pipeline

            }

        }


        public void OnResourceExecuted(ResourceExecutedContext context)

        {

            var key = context.HttpContext.Request.Path.ToString();

            if (!_cache.ContainsKey(key))

            {

                _cache[key] = context.Result;

            }

        }
    }
}