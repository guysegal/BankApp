namespace BankApp.Domain
{
    public class BillingDetails
    {
        public int Year { get; private set; }
        public Month Month { get; private set; }

        public BillingDetails(int month, int year)
        {
            Month = (Month) month;
            Year = year;
        }
    }
}
