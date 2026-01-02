using Covenant.IdentityServer.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace Covenant.IdentityServer.Controllers.Configuration
{
    [Authorize(AuthenticationSchemes = Microsoft365OpenIdConnect.Scheme)]
    public class ConfigurationController : Controller
    {
        public async Task<IActionResult> Index()
        {
            string tokenAsync = await HttpContext.GetTokenAsync(Microsoft365OpenIdConnect.Scheme, "access_token");
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenAsync);
            var message = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/oidc/userinfo");
            HttpResponseMessage response = await httpClient.SendAsync(message);
            ViewBag.UserInfo = await response.Content.ReadAsStringAsync();
            return View();
        }

        public async Task<IActionResult> UserInfo()
        {
            string tokenAsync = await HttpContext.GetTokenAsync(Microsoft365OpenIdConnect.Scheme, "access_token");
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenAsync);
            HttpResponseMessage response = await httpClient.GetAsync("https://graph.microsoft.com/v1.0/me");
            if (!response.IsSuccessStatusCode) return Ok();
            ViewBag.UserInfo = await response.Content.ReadAsStringAsync();
            return View("Index");
        }
    }
}