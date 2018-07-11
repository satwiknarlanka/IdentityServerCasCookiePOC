using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace IdentityServerCasPOC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IMemoryCache _cache;
        private readonly IOptions<IdentityServerSettings> _options;
        public AccountController(IMemoryCache cache, IOptions<IdentityServerSettings> options)
        {
            _cache = cache;
            _options = options;
        }
        public IActionResult Login(string returnUrl)
        {
            var key = new Random().Next();
            _cache.CreateEntry(key);
            _cache.Set(key, returnUrl);
            var casRequestUrl = _options.Value.CasRequestUrl +_options.Value.BaseAddress+ "/Account/ValidateCas" +"?cacheKey=" + key;
            return Redirect(casRequestUrl);

        }

        public async Task<IActionResult> ValidateCas(string casticket, string cacheKey)
        {
            string casResponse;
            using (var client = new HttpClient())
            {
                var validateUrl = _options.Value.CasValidateUrl + casticket + "&casurl=" +_options.Value.BaseAddress;
                casResponse = await client.GetStringAsync(validateUrl);
            }

            if (casResponse.ToLower() == "no")
            {
                return new UnauthorizedResult();
            }

            var user = casResponse.Split("\r\n")[1];
            await HttpContext.SignInAsync(user, user);
            if (_cache.TryGetValue(cacheKey, out string returnUrl))
            {
                _cache.Remove(cacheKey);
                return Redirect(returnUrl);
            }
            return Redirect("https://localhost:5003");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            //return Redirect(_options.Value.CasLogoutUrl);
            return Ok();
        }
    }
}