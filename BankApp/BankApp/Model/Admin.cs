namespace BankApp.Model
{
    public class Admin : User
    {
        public Admin() : base()
        {
        }

        public Admin(string firstname, string lastname, string pseudo, string email, string password) : base(firstname, lastname, pseudo, email, password)
        {
        }
    }
}
