using System;
using System.Collections.Generic;
using System.Linq;
using BankApp.Domain;
using BankApp.Infrastructure.Login;
using HEMS.Core.NEW.Infrastructure;

namespace BankApp.Infrastructure
{
    public class FibiWebSiteProxy: IBankProxy
    {
        protected WebSiteSession Session;
        private readonly FibiWebSiteLoginService _logicService;
        protected readonly FibiHtmlPagesParser HtmlPagesParser;

        public FibiWebSiteProxy(FibiWebSiteLoginService logicService, FibiHtmlPagesParser htmlPagesParser)
        {
            _logicService = logicService;
            HtmlPagesParser = htmlPagesParser;
        }
     
        public void Login(string usernanme, string password)
        {
            Session = _logicService.Login(usernanme, password);
        }

        public List<CreditCard> GetCreditCards()
        {
            var postRequest = new FibiPostRequest("https://www.fibi-online.co.il/web/Processing?SUGBAKA=211", Session);           
            var response = postRequest.Process();
            var creditCardHtmlPage = response.GetHtmlPage();
            var creditCards = HtmlPagesParser.ParseCreditCardsFromHtml(creditCardHtmlPage);

            return creditCards;
        }

        public virtual List<CreditCardTransaction> GetCreditCardTransactionsFor(CreditCard creditCard, BillingDetails billingDetails)
        {
            var fibiCreditCard = (FibiCreditCard) creditCard;
            var dateString = ToFibiDay(creditCard.BillingDay) + "." + ToFibiMonth(billingDetails.Month) + "." + billingDetails.Year;
            
            var url = "https://www.fibi-online.co.il/web/Processing?SUGBAKA=211&RESFOR=XRL&I-SEL-MS-KARTIS=" + fibiCreditCard.FibiId +
                      "&I-TR-CHIYUV=" + dateString +
                      "&I-D-STATUS=CH-KAROV";

            var postRequest = new FibiPostRequest(url, Session);
            var response = postRequest.Process();
            var creditCardTransactionsHtmlPage = response.GetHtmlPage();
            var transactions = HtmlPagesParser.ParseCreditCardTransactionsFromHtml(creditCardTransactionsHtmlPage);
            transactions.ForEach(x=> x.CreditCard = creditCard);
            return transactions;

        }  

        protected string ToFibiDay(int day)
        {
            if (day < 10)
                return "0" + day;

            return day.ToString();
        }

        protected string ToFibiMonth(Month month)
        {
            switch (month)
            {
                case Month.January: return "01";
                case Month.February: return "02";
                case Month.March: return "03";
                case Month.April: return "04";
                case Month.May: return "05";
                case Month.June: return "06";
                case Month.July: return "07";
                case Month.August: return "08";
                case Month.September: return "09";
                case Month.October: return "10";
                case Month.November: return "11";
                case Month.December: return "12";
                default: throw new Exception("Unreachable code");
            }
        }
    }
}
