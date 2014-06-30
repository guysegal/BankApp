using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HEMS.Core.NEW.Infrastructure.Login
{
    public class WebProcessor
    {
        private string GeneratedSource { get; set; }
        private string URL { get; set; }

        public string GetGeneratedHtml(string url)
        {
            URL = url;

            var t = new Thread(WebBrowserThread);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();

            return GeneratedSource;
        }

        private void WebBrowserThread()
        {
            var wb = new WebBrowser();
            wb.Navigate(URL);           

            wb.DocumentCompleted += wb_DocumentCompleted;

            while (wb.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }

            //Added this line, because the final HTML takes a while to show up
            GeneratedSource = wb.Document.Body.InnerHtml;

            wb.Dispose();
        }

        private void wb_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            WebBrowser wb = (WebBrowser)sender;
            GeneratedSource = wb.Document.Body.InnerHtml;
        }
    }
}
