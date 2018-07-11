using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace RestApi
{
    public class RolesMiddleware
    {
        private readonly RequestDelegate _next;
        private ILogger _logger;
        public RolesMiddleware(RequestDelegate next, ILogger<RolesMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public Task Invoke(HttpContext context)
        {
            context.User.Claims.Append(new Claim(ClaimTypes.Role, "test"));
            return _next(context);
        }
    }
}
