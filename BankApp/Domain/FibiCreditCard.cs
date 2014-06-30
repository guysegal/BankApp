namespace BankApp.Domain
{
    public class FibiCreditCard : CreditCard
    {
        public string FibiId { get; private set; }

        public FibiCreditCard(string fibiId, string last4Digits)
            : base(last4Digits)
        {
            FibiId = fibiId;
        }
    }
}