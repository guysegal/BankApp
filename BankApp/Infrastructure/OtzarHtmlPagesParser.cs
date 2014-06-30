using System.Collections.Generic;
using System.Linq;
using BankApp.Domain;
using HtmlAgilityPack;

namespace BankApp.Infrastructure
{
    public class OtzarHtmlPagesParser : FibiHtmlPagesParser
    {
        public override List<CreditCard> ParseCreditCardsFromHtml(string creditCardsHtmlPage)
        {
            var html = new HtmlDocument();
            html.LoadHtml(creditCardsHtmlPage);

            var creditCardsRows = html.DocumentNode.SelectSingleNode("//*[contains(@class,'maintable')]")
                .ChildNodes.Where(x => x.NodeType == HtmlNodeType.Element && x.ChildNodes.Count == 8)
                .Select(x => x.ChildNodes).ToList();

            var result = creditCardsRows.Select(tr =>
                                                {
                                                    var fibiId = tr[2].InnerHtml.Split(',')[1].Replace("'", "");
                                                    var last4Digits = tr[6].InnerText.Substring(0, 4);
                                                    return (CreditCard)new FibiCreditCard(fibiId, last4Digits);
                                                }).ToList();

            return result;
        }
    }
}