using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using WebEvent.API.Model.DTO;
using WebEvent.API.Model.ErrorModel;

namespace WebEvent.Client.Controllers
{
    [Route("registration")]
    public class RegistrationController : Controller
    {
        private readonly HttpClient _httpClient;

        public RegistrationController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("Client");
        }

        [Route("registration-page")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("registrate")]
        public async Task<ActionResult> OnPost(UserDto dto)
        {
            JsonContent content = JsonContent.Create(dto);

            using HttpResponseMessage response = await _httpClient.PostAsync("api/account/registration", content);

            if (!response.IsSuccessStatusCode)
            {
                CustomException? errorMessage = JsonSerializer.Deserialize<CustomException>(await response.Content.ReadAsStringAsync());

                ViewBag.Message = errorMessage.Message;

                return View("Index", dto);
            }

            return RedirectToAction("Index", "Login");
        }
    }
}
