using System;
using System.IO;
using System.Net;
using HEMS.Core.NEW;
using HEMS.Core.NEW.Infrastructure;

namespace BankApp.Infrastructure
{
    public class FibiPostRequest
    {
        private readonly string _url;
        private readonly WebSiteSession _session;
        public byte[] RequestData { get; set; }

        public FibiPostRequest(string url)
        {
            _url = url;    
            _session = new WebSiteSession();
        }

        public FibiPostRequest(string url, WebSiteSession session)
        {
            _url = url;
            _session = session;
        }

        public FibiPostResponse Process()
        {
            if (RequestData == null) RequestData = new byte[0];
            var httpWReq = WebRequest.Create(_url) as HttpWebRequest;
            if (httpWReq == null) throw new Exception("TODO: Add exception for this situation");

            httpWReq.Method = "POST";
            httpWReq.Host = "www.fibi-online.co.il";
            httpWReq.KeepAlive = true;
            httpWReq.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.4 (KHTML, like Gecko) Chrome/22.0.1229.79 Safari/537.4";
            httpWReq.ContentType = "application/x-www-form-urlencoded";
            httpWReq.Accept = " text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            httpWReq.ContentLength = RequestData.Length;
            httpWReq.CookieContainer = _session.Cookies;

            using (Stream newStream = httpWReq.GetRequestStream())
            {
                newStream.Write(RequestData, 0, RequestData.Length);
            }
            return new FibiPostResponse {HttpResponse = (HttpWebResponse) httpWReq.GetResponse()};
        }
    }
}
