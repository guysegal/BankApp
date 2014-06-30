using System;
using System.Collections.Generic;
using System.Linq;
using BankApp.Infrastructure;

namespace BankApp.Domain
{
    public class Bank : IBank
    {
        private readonly IBankProxy _bankProxy;

        public Bank(IBankProxy bankProxy)
        {
            _bankProxy = bankProxy;
        }

        public void Login(string usernanme, string password)
        {
            _bankProxy.Login(usernanme, password);
        }

        public List<CreditCard> GetCreditCards()
        {
            return _bankProxy.GetCreditCards();
        }

        public List<CreditCardTransaction> GetTranscationsForCreditCard(CreditCard creditCard, DateTime from, DateTime to)
        {
            var billingDetails = GetBillingDetails(from, to);
            var allTransactions = new List<CreditCardTransaction>();
            foreach (var billing in billingDetails)
            {
                var transactions = _bankProxy.GetCreditCardTransactionsFor(creditCard, billing);
                allTransactions.AddRange(transactions);
            }
            var result = allTransactions.Where(t => (t.Date <= to && t.Date >= from) || t.PaymentsTransaction).ToList();
            return result;
        }

        private IEnumerable<BillingDetails> GetBillingDetails(DateTime from, DateTime to)
        {
            var billings = new List<BillingDetails>();           
            to = to.AddMonths(1);
            var movingDate = new DateTime(from.Year, from.Month, 1);
            while (movingDate < to)
            {
                billings.Add(new BillingDetails(movingDate.Month, movingDate.Year));
                movingDate = movingDate.AddMonths(1);
            }
            return billings;
        }
    }    
}
