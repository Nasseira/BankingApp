using System.Collections.Generic;
using System.Linq;

namespace BankApp.Model
{
    public enum AccountType
    {
        Current,
        Saving
    }
    public abstract class InternalAccount : Account
    {
        public AccountType Type { get; set; }

        public InternalAccount() : base()
        {
        }

        public InternalAccount(string iban, string description, double floorAmount) : base(iban, description, floorAmount)
        {
        }

        public virtual ICollection<ClientInternalAccount> ClientInternalAccounts { get; set; } = new HashSet<ClientInternalAccount>();

        public static void GetSolde()
        {
            var getAllTransfersSorted = Context.BankTransfers.OrderBy(bt => bt.DateConcerned).ToList();
            var getAllAccounts = Context.InternalAccounts;

            // pour remettre à 0 tous les soldes internes pour le calcul des soldes.
            foreach (var bt in getAllAccounts)
                bt.Solde = 0;

            foreach (var bt in getAllTransfersSorted)
            {
                if (bt.DateConcerned <= App.CurrentDate)
                {
                    if (bt.FromAccount is ExternalAccount || bt.FromAccount.Solde - bt.Amount >= bt.FromAccount.FloorAmount)
                    {
                        if (bt.FromAccount is not ExternalAccount)
                            bt.FromAccount.Solde -= bt.Amount;

                        bt.ToAccount.Solde += bt.Amount;
                        bt.IsValidStatement = true;
                        bt.StatetementState = StatementState.ACCEPTED;
                        bt.ColorStatement = "#a6aba8";
                    }
                    else
                    {
                        bt.IsValidStatement = false;
                        bt.StatetementState = StatementState.REFUSED;
                        bt.ColorStatement = "#b3889c";
                    }
                }
                else
                {
                    bt.StatetementState = StatementState.FUTURE;
                    bt.ColorStatement = "#6581b8";
                }
            }
        }

        public static List<InternalAccount> GetOthersInternalAccounts(int ClientId)
        {
            var internalAccounts = Context.InternalAccounts;
            var clientAccounts = ClientInternalAccount.GetAccountsByClient(ClientId);
            var result = internalAccounts.Where(acc => !clientAccounts.Any(myAcc => acc.IBAN.Contains(myAcc.IBAN))).ToList();
            return result;
        }
    }
}
