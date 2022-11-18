namespace BankApp.Model
{
    public class CurrentAccount : InternalAccount
    {

        public CurrentAccount() : base()
        {
        }

        public CurrentAccount(string iban, string description, double floorAmount) : base(iban, description, floorAmount)
        {
            Type = AccountType.Current;
        }
    }
}
