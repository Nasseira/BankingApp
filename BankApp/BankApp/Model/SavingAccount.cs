namespace BankApp.Model
{
    public class SavingAccount : InternalAccount
    {
        public SavingAccount() : base()
        {
        }

        public SavingAccount(string iban, string description, double floorAmount = 0) : base(iban, description, floorAmount)
        {
            Type = AccountType.Saving;
        }
    }
}
