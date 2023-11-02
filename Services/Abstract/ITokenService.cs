namespace WebEvent.Client.Services.Abstract
{
    public interface ITokenService
    {
        public void AddTokenToCookie(string token, HttpContext context, string tokenName, int exipierDays);
        public void RemoveTokenFromCookie(HttpContext context);
    }
}