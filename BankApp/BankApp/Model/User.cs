using PRBD_Framework;

using System.Collections.Generic;
using System.Linq;

namespace BankApp.Model
{
    public abstract class User : EntityBase<BankContext>
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Pseudo { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public string UserFullName => $"{Firstname} {Lastname}";

        public User() : base()
        {
        }

        public User(string firstname, string lastname, string pseudo, string email, string password)
        {
            Firstname = firstname;
            Lastname = lastname;
            Pseudo = pseudo;
            Email = email;
            Password = password;
        }

        public virtual ICollection<BankTransfer> BankTransfers { get; set; } = new HashSet<BankTransfer>();

        public static User GetUserByEmail(string email)
        {
            return Context.Users.FirstOrDefault(c => c.Email == email);
        }

        public static User GetUserById(int userId)
        {
            return Context.Users.Find(userId);
        }

    }
}
