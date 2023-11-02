using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using WebEvent.API.Model.DTO;
using WebEvent.Client.Services.Abstract;

namespace WebEvent.Client.Controllers
{
    [Route("login")]
    public class LoginController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenService _tokenService;

        public LoginController(IHttpClientFactory httpClientFactory, ITokenService tokenService)
        {
            _httpClient = httpClientFactory.CreateClient("Client");
            _tokenService = tokenService;
        }

        [Route("login-page")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LogIn(UserDto dto)
        {
            JsonContent content = JsonContent.Create(dto);

            HttpResponseMessage response = await _httpClient.PostAsync("api/authorization/login", content);

            if (!response.IsSuccessStatusCode)
            {
                string error = await response.Content.ReadAsStringAsync();

                ViewBag.Message = error;

                return View("Index", dto);
            }

            _tokenService.AddTokenToCookie(await response.Content.ReadAsStringAsync(), HttpContext, "token", 1);

            return RedirectToAction("personal-info", "account");
        }
    }
}
