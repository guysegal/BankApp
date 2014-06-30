using System;
using System.Collections.Generic;

namespace BankApp.Domain
{
    public interface IBank
    {
        void Login(string usernanme, string password);
        List<CreditCard> GetCreditCards();
        List<CreditCardTransaction> GetTranscationsForCreditCard(CreditCard creditCard, DateTime from, DateTime to);
    }
}