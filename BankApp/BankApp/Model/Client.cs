using System.Collections.Generic;

namespace BankApp.Model
{
    public class Client : User
    {
        public Client() : base()
        {
        }

        public Client(string firstname, string lastname, string pseudo, string email, string password, Agency agency) : base(firstname, lastname, pseudo, email, password)
        {
            Agency = agency;
        }

        public virtual Agency Agency { get; set; }

        public virtual ICollection<ClientInternalAccount> ClientInternalAccounts { get; set; } = new HashSet<ClientInternalAccount>();
    }
}
