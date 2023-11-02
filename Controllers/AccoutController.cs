using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using WebEvent.API.Model.DTO;
using WebEvent.API.Model.Entity;
using WebEvent.API.Model.ErrorModel;
using Microsoft.AspNetCore.Identity;

namespace WebEvent.Client.Controllers
{
    [Route("account")]
    public class AccoutController : Controller
    {
        private readonly HttpClient _httpClient;
        private static UserDto _user;
        public AccoutController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("Client");
        }
        [Route("personal-info")]
        public async Task<IActionResult> Index()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);
            
            HttpResponseMessage response = await _httpClient.GetAsync($"api/authorization/user");

            if (!response.IsSuccessStatusCode)
            {
                CustomException? errorMessage = JsonSerializer.Deserialize<CustomException>(await response.Content.ReadAsStringAsync());

                ViewBag.Message = errorMessage.Message;

                return View("personal-info", _user);
            }

            string jsonUser = await response.Content.ReadAsStringAsync();

            _user = JsonSerializer.Deserialize<UserDto>(jsonUser);

            return View("Index", _user);
        }

        [Route("edit-view")]
        public IActionResult EditView(UserDto dto)
        {
            dto = _user;

            return View("Update", dto);
        }

        [Route("update")]
        public async Task<IActionResult> Update(UserDto dto)
        {
            dto.Password = "hiden";

            _user = dto;

            JsonContent content = JsonContent.Create(_user);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            HttpResponseMessage response = await _httpClient.PatchAsync("api/account/modify-user", content);

            if (!response.IsSuccessStatusCode)
            {
                CustomException? errorMessage = JsonSerializer.Deserialize<CustomException>(await response.Content.ReadAsStringAsync());

                ViewBag.Message = errorMessage.Message;

                return View("Update", dto);
            }
            return View("Index", _user);
        }

        [Route("change-password")]
        public async Task<ActionResult> ChangePassword(ChangePasswordModel model, UserDto dto)
        {
            JsonContent content = JsonContent.Create(model);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            HttpResponseMessage response = await _httpClient.PatchAsync("api/account/change-password", content);

            if (!response.IsSuccessStatusCode)
            {
                CustomException? error = JsonSerializer.Deserialize<CustomException>(await response.Content.ReadAsStringAsync());

                return BadRequest(error.Message);
            }

            dto = _user;

            return View("Update", dto);
        }

        [Route("change-password-view")]
        public IActionResult ChangePasswordView()
        {
            return View("ChangePassword");
        }
    }
}
