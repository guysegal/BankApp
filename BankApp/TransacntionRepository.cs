using System;
using System.Collections.Generic;
using System.Linq;
using BankApp.Domain;

namespace BankApp
{
    public class TransacntionRepository : ITransancationsRepository
    {
        private readonly List<CreditCardTransaction> _transactions = new List<CreditCardTransaction>();

        public DateTime GetLastTransactionDate(CreditCard card)
        {
            if (!IsTransactionsExistForCard(card))
                return new DateTime(2012,1,1);

            return _transactions
                .Where(t => t.CreditCard.Last4Digits == card.Last4Digits)
                .Max(x => x.Date);
        }

        public bool TryAdd(CreditCardTransaction transaction)
        {
            if (IsExists(transaction))
                return false;

            _transactions.Add(transaction);
            return true;
        }

        public List<CreditCardTransaction> GetAll(CreditCard card)
        {
            return _transactions.Where(t => t.CreditCard.Last4Digits == card.Last4Digits).ToList();         
        }

        public List<CreditCardTransaction> GetAll()
        {
            return _transactions;
        }

        public bool IsExists(CreditCardTransaction transanction)
        {
            return _transactions.Exists(t => t.Id == transanction.Id);
        }

        public bool IsTransactionsExistForCard(CreditCard card)
        {
            return _transactions.Exists(x => x.CreditCard.Last4Digits == card.Last4Digits);
        }
    }
}