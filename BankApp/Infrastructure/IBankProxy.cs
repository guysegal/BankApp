using System.Collections.Generic;
using BankApp.Domain;

namespace BankApp.Infrastructure
{
    public interface IBankProxy
    {
        List<CreditCard> GetCreditCards();
        List<CreditCardTransaction> GetCreditCardTransactionsFor(CreditCard creditCared, BillingDetails billingDetails);
        void Login(string usernanme, string password);
    }
}