using PRBD_Framework;

using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BankApp.Model
{
    public enum StatementState
    {
        ACCEPTED,
        REFUSED,
        FUTURE
    }
    public class BankTransfer : EntityBase<BankContext>
    {
        [NotMapped]
        public bool IsValidStatement { get; set; }
        [NotMapped]
        public StatementState StatetementState { get; set; }
        [NotMapped]
        public string ColorStatement { get; set; }
        public BankTransfer()
        {
        }

        public BankTransfer(Account fromAccount, Account toAccount, double amount, string description, User user, DateTime creation, DateTime? execution = null, Category? category = null)
        {
            Amount = amount;
            Description = description;
            Creation = creation;
            Execution = execution;
            ToAccount = toAccount;
            FromAccount = fromAccount;
            Category = category;
            User = user;
            DateConcerned = Execution ?? Creation;
        }

        public int Id { get; set; }
        public double Amount { get; set; }
        public string Description { get; set; }
        public DateTime Creation { get; set; }
        public DateTime? Execution { get; set; }

        public DateTime DateConcerned { get; set; }

        [InverseProperty(nameof(Account.ToAccounts))]
        public virtual Account ToAccount { get; set; }
        [InverseProperty(nameof(Account.FromAccounts))]
        public virtual Account FromAccount { get; set; }
        public virtual Category Category { get; set; }
        public virtual User User { get; set; }

    }
}
