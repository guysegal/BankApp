using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HEMS.Core.NEW.Infrastructure
{
    public class WebSiteSession
    {
        public CookieContainer Cookies { get; set; }

        public WebSiteSession()
        {
            Cookies = new CookieContainer();
        }
    }
}
