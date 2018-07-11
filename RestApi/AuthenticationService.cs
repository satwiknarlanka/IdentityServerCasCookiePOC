using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;

namespace RestApi
{
    public class AuthenticationService
    {
        private readonly RequestDelegate _next;
        private ILogger _logger;
        public AuthenticationService(RequestDelegate next, ILogger<AuthenticationService> logger)
        {
            _next = next;
            _logger = logger;
        }

        public Task Invoke(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("ORIGIN", out var origin))
            {
                _logger.LogDebug(1,origin);
            };
            var values = context.Request.Headers.Values;
            if (context.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var token = authHeader.ToString().Split(" ")[1];
                _logger.LogDebug(1,token);
                context.Request.Headers.Remove("Authorization");
                context.Request.Headers.Add("Authorization", "Bearer " + token);
            }
            return _next(context);
        }
    }
}
