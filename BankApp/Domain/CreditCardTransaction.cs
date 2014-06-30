using System;

namespace BankApp.Domain
{
    public class CreditCardTransaction
    {
        public string Id { get; set; }
        public CreditCard CreditCard {get; set; }
        public DateTime Date { get; set;}
        public double Amount { get; set; }
        public bool PaymentsTransaction { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return string.Format("Card {0}, Date: {1}, Description: {2}, Amount: {3}, Payments: {4}", 
                CreditCard.Last4Digits, Date, Amount, Description, PaymentsTransaction);
        }
    }
}
