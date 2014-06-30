using System.Net;

namespace BankApp.Extensions
{
    public static class CookieCollectionExtensions
    {
        public static CookieContainer ToCookieContainer(this CookieCollection cookieCollection)
        {
            var cookieContainer = new CookieContainer();
            foreach (Cookie cookie in cookieCollection)
                cookieContainer.Add(cookie);
            return cookieContainer;
        }
    }
}
