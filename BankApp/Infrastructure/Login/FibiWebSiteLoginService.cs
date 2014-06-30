using System;
using System.Text;
using BankApp.Extensions;
using HEMS.Core.NEW.Infrastructure;

namespace BankApp.Infrastructure.Login
{
    public class FibiWebSiteLoginService
    {
        private readonly FibiEncryptionService _encryptionService;

        public FibiWebSiteLoginService(FibiEncryptionService encryptionService)
        {
            _encryptionService = encryptionService;
        }

        public WebSiteSession Login(string username, string password)
        {
            var encrypredPassword = _encryptionService.EncryptPassword(password);

            var data = string.Concat(
                "ZIHMIST=" + username + "&",
                "KOD=" + password + "&",
                "_enter=&",
                "requestId=logon&",
                "KODSAFA=HE&",
                "IDW_TNCK_idw=" + encrypredPassword);

            var loginPostRequest = new FibiPostRequest("https://www.fibi-online.co.il/web/fibiwwwc")
            {                         
                RequestData = Encoding.UTF8.GetBytes(data)
            };

            var logicPostResponse = loginPostRequest.Process();
            var loginResultPage = logicPostResponse.GetHtmlPage();

            if (!loginResultPage.Contains("כניסה למערכת"))
                throw new Exception("Login was unsuccsesfull");

            return new WebSiteSession
            {
                Cookies = logicPostResponse.HttpResponse.Cookies.ToCookieContainer()
            };
        }
    }
}
