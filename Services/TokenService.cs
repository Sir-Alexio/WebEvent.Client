using WebEvent.Client.Services.Abstract;

namespace WebEvent.Client.Services
{
    public class TokenService :ITokenService
    {
        private readonly HttpClient _httpClient;
        public TokenService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("Client");
        }
        public void AddTokenToCookie(string token, HttpContext context, string tokenName, int exipierDays)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(exipierDays)
            };

            context.Response.Cookies.Append(tokenName, token, cookieOptions);
        }

        public void RemoveTokenFromCookie(HttpContext context)
        {
            var options = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(-1)
            };

            context.Response.Cookies.Delete("token", options);
        }

    }
}
