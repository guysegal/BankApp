using System;
using System.IO;
using HEMS.Core.NEW;
using HEMS.Core.NEW.Infrastructure.Login;
using HtmlAgilityPack;

namespace BankApp.Infrastructure.Login
{
    public class FibiEncryptionService
    {
        private const string EncryptionHtmlPath = @"Resources\passwordEncryption.html";
        private const string PublicKeyHttpRequestPath = @"https://www.fibi-online.co.il/web/GetPublicKey.do?d=";

        public string EncryptPassword(string password)
        {
            var htmlEncryptionPage = new HtmlDocument();
            htmlEncryptionPage.Load(EncryptionHtmlPath);

            var publicKey = GeneratePublicKey();
            htmlEncryptionPage.DocumentNode.SelectSingleNode("/html[1]/body[1]/input[2]").SetAttributeValue("value", password);
            htmlEncryptionPage.DocumentNode.SelectSingleNode("/html[1]/body[1]/input[1]").SetAttributeValue("value", publicKey);
            htmlEncryptionPage.Save(EncryptionHtmlPath);

            var webProcessor = new WebProcessor();
            var htmlAfterJavascriptEncryption = webProcessor.GetGeneratedHtml(Path.GetFullPath(EncryptionHtmlPath));
            htmlEncryptionPage.LoadHtml(htmlAfterJavascriptEncryption);   
            
            var encryptedPassword = htmlEncryptionPage.GetElementbyId("encryptedPassword").GetAttributeValue("value", "");
            return encryptedPassword;
        }

        private string GeneratePublicKey()
        {
            var d = Convert.ToInt64((Math.Round((DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds)));
            var post = new FibiPostRequest(PublicKeyHttpRequestPath + d);
            var response = post.Process();
            return response.GetHtmlPage();
        }
    }
}
