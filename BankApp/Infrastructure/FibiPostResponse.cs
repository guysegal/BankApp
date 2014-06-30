using System.IO;
using System.Net;
using System.Text;

namespace BankApp.Infrastructure
{
    public class FibiPostResponse
    {
        public HttpWebResponse HttpResponse { get; set; }

        public string GetHtmlPage()
        {
            var html = string.Empty;
            using (var streamReader = new StreamReader(HttpResponse.GetResponseStream(), Encoding.GetEncoding(1255)))
            {
               html =  streamReader.ReadToEnd();
            }
            return html;
        }
    }
}