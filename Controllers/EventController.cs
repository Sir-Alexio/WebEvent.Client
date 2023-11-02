using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using WebEvent.API.Model.DTO;
using WebEvent.API.Model.ErrorModel;

namespace WebEvent.Client.Controllers
{
    [Route("event")]
    public class EventController : Controller
    {
        private readonly HttpClient _httpClient;

        public EventController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("Client");
        }

        [HttpPost]
        [Route("create-event")]
        public async Task<IActionResult> OnPost(EventDto dto)
        {
            return Ok(dto);
        }

        [Route("my-events-view")]
        public async Task<IActionResult> MyEvents()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Request.Cookies["token"]);

            HttpResponseMessage response = await _httpClient.GetAsync($"api/event/user-events");

            if (!response.IsSuccessStatusCode)
            {
                CustomException? errorMessage = System.Text.Json.JsonSerializer.Deserialize<CustomException>(await response.Content.ReadAsStringAsync());

                ViewBag.Message = errorMessage.Message;

                return BadRequest(errorMessage.Message);
            }

            string jsonEvents= await response.Content.ReadAsStringAsync();

            List<EventDto>? deserializedEventDtos = JsonSerializer.Deserialize<List<EventDto>>(jsonEvents);

            return View("MyEvents", deserializedEventDtos);
        }

        [Route("create-event-page")]
        public IActionResult CreateEventPage()
        {
            return View("CreateEvent");
        }
    }
}
