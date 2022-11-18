namespace BankApp.Model
{
    public class ExternalAccount : Account
    {
        public ExternalAccount() : base()
        {
        }

        public ExternalAccount(string iban, string description, double floorAmount = 0) : base(iban, description, floorAmount)
        {
        }
    }
}
