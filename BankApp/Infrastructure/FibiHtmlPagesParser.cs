using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BankApp.Domain;
using HtmlAgilityPack;
using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace BankApp.Infrastructure
{
    public class FibiHtmlPagesParser
    {
        public virtual List<CreditCard> ParseCreditCardsFromHtml(string creditCardsHtmlPage)
        {
            var html = new HtmlDocument();
            html.LoadHtml(creditCardsHtmlPage);

            var creditCardsRows = html.DocumentNode.SelectSingleNode("//*[contains(@class,'maintable')]")
                .ChildNodes.Where(x => x.NodeType == HtmlNodeType.Element && x.ChildNodes.Count == 9)
                .Select(x=>x.ChildNodes).ToList();

            var result = creditCardsRows.Select(tr =>
            {
                var fibiId = tr[3].InnerHtml.Split(',')[1].Replace("'", "");
                var last4Digits = tr[7].InnerText.Substring(0, 4);
                return (CreditCard)new FibiCreditCard(fibiId, last4Digits);                             
            }).ToList();

            return result;
        }

        public List<CreditCardTransaction> ParseCreditCardTransactionsFromHtml(string creditCardTransactionsHtmlPage)
        {
            var html = new HtmlDocument();
            html.LoadHtml(creditCardTransactionsHtmlPage);
          
            var transancationsTable = html.GetElementbyId("tK1");
            var transactionsRows = transancationsTable.ChildNodes
                .Where(x => x.NodeType == HtmlNodeType.Element)
                .Select(x=>x.ChildNodes).ToList();

            var index = 0;
            var result = transactionsRows.Select(tr =>
            {
                index++;
                var date = DateTime.Parse(tr[5].InnerText);
                var amount = Convert.ToDouble(tr[2].InnerText);
                return new CreditCardTransaction
                    {
                        Id =date.Ticks +"." +amount + "." + index,  
                        Date = date,
                        Amount = amount,
                        Description = tr[4].InnerText,
                        PaymentsTransaction = tr[1].InnerText.Contains("םולשת")
                    };
            }).ToList();                                              
            return result;
        }
    }
}
