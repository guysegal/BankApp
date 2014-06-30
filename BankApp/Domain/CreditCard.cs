namespace BankApp.Domain
{
    public class CreditCard
    {
        public string Last4Digits { get; private set; }
        public int BillingDay = 2;

        public CreditCard(string last4Digits)
        {
            Last4Digits = last4Digits;
        }
    }
}
