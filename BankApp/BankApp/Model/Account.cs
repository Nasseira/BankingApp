using PRBD_Framework;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace BankApp.Model
{
    public abstract class Account : EntityBase<BankContext>
    {
        public int Id { get; set; }
        public string IBAN { get; set; }
        public string Description { get; set; }
        [NotMapped]
        public double Solde { get; set; }
        public double FloorAmount { get; set; }

        public Account()
        {
        }

        public Account(string iban, string description, double floorAmount)
        {
            IBAN = iban;
            Description = description;
            FloorAmount = floorAmount;
        }

        [InverseProperty(nameof(BankTransfer.ToAccount))]
        public virtual ICollection<BankTransfer> ToAccounts { get; set; } = new HashSet<BankTransfer>();
        [InverseProperty(nameof(BankTransfer.FromAccount))]
        public virtual ICollection<BankTransfer> FromAccounts { get; set; } = new HashSet<BankTransfer>();

        public static IQueryable<Account> GetAll()
        {
            return Context.Accounts;
        }
    }
}
