using System.Collections.Generic;

namespace BankApp.Model
{
    public class Manager : User
    {
        public Manager() : base()
        {
        }

        public Manager(string firstname, string lastname, string pseudo, string email, string password) : base(firstname, lastname, pseudo, email, password)
        {
        }
        public virtual ICollection<Agency> Agencies { get; set; } = new HashSet<Agency>();
    }
}
