using System;
using System.Collections.Generic;
using BankApp.Domain;

namespace BankApp
{
    public interface ITransancationsRepository
    {
        DateTime GetLastTransactionDate(CreditCard card);

        bool IsExists(CreditCardTransaction transanction);

        bool TryAdd(CreditCardTransaction transaction);

        List<CreditCardTransaction> GetAll(CreditCard card);
    }
}