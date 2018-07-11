using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json.Linq;
using SPAhost.Models;

namespace SPAhost.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public async Task<IActionResult> Claims()
        {
            string redirectUrl = Request.Query["redirectUrl"];
            ViewBag.access_token = await HttpContext.GetTokenAsync("access_token");
            ViewBag.refresh_token = await HttpContext.GetTokenAsync("refresh_token");
            if (redirectUrl != null)
            {
                ViewBag.redirect = redirectUrl;
            }

            return View();
        }

        public async Task<IActionResult> ReAuthenticate()
        {
            var redirectUrl = "https://google.com";
            if (Request.Headers.TryGetValue("REFERER", out var requestPath))
            {
                redirectUrl = requestPath;
            }
            await HttpContext.SignOutAsync("Cookies");
            await HttpContext.SignOutAsync("oidc");
            return RedirectToAction(nameof(Claims), new{ redirectUrl });
        }
        //[Authorize]
        //public async Task<IActionResult> GetValues()
        //{
        //    var accessToken = await HttpContext.GetTokenAsync("access_token");
        //    using (var client = new HttpClient())
        //    {
        //        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        //        var content = await client.GetStringAsync("https://localhost:5005/api/values");
        //        ViewBag.Json = JArray.Parse(content).ToString();
        //    }

        //    return View();
        //}

        [Authorize]
        public async Task<IActionResult> RefreshToken(string refreshToken)
        {
            
            var auth = await DiscoveryClient.GetAsync("https://localhost:5001");
            var client = new TokenClient(auth.TokenEndpoint, "MySpaHost", "secret");
            var tokenResponse = await client.RequestRefreshTokenAsync(refreshToken);
            var token = new Mytokens()
            {
                access_token = tokenResponse.AccessToken,
                refresh_token = tokenResponse.RefreshToken
            };
            return Ok(token);
        }

        [Authorize]
        public IActionResult GetValues()
        {
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            await HttpContext.SignOutAsync("oidc");
            return Redirect("https://cas.iu.edu/cas/logout");
        }
    }
}

public class Mytokens
{
    public string access_token { get; set; }
    public string refresh_token { get; set; }
}
