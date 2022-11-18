using Microsoft.EntityFrameworkCore;

using PRBD_Framework;

using System;
using System.Collections.Generic;
using System.Linq;

namespace BankApp.Model
{
    public enum ClientRole
    {
        Holder,
        Agent
    }

    public class ClientInternalAccount : EntityBase<BankContext>
    {
        public int ClientId { get; set; }
        public int InternalAccountId { get; set; }
        public ClientRole ClientRole { get; protected set; }

        public ClientInternalAccount()
        {
        }

        public ClientInternalAccount(Client client, InternalAccount internalAccount, ClientRole role)
        {
            Client = client;
            InternalAccount = internalAccount;
            ClientRole = role;
        }

        public virtual Client Client { get; set; }
        public virtual InternalAccount InternalAccount { get; set; }

        public static IQueryable<ClientInternalAccount> GetClientInternalAccountByClient(int ClientId)
        {
            return Context.ClientInternalAccounts.Where(ci => ci.ClientId == ClientId);
        }

        public static IQueryable<Account> GetAccountsByClient(int ClientId)
        {
            var query = from cia in Context.ClientInternalAccounts
                        where cia.ClientId == ClientId
                        select cia.InternalAccount;
            return query;
        }

        public static IQueryable<Account> GetAllAccountsByClientExceptOne(int ClientId, Account ExcludedAccount)
        {
            IQueryable<Account> query = null;
            if (ExcludedAccount is SavingAccount)
            {
                query = (from cia in Context.ClientInternalAccounts
                         where cia.InternalAccountId != ExcludedAccount.Id && cia.ClientId == ClientId && cia.InternalAccount is CurrentAccount
                         select cia.InternalAccount);

            }
            else if (ExcludedAccount is CurrentAccount)
            {
                query = from cia in Context.ClientInternalAccounts
                        where cia.InternalAccountId != ExcludedAccount.Id && cia.ClientId == ClientId
                        select cia.InternalAccount;

            }
            return query.Distinct();
        }

        public static IQueryable<ClientInternalAccount> GetAccountsByFilter(string Filter, int ClientId)
        {
            return Context.ClientInternalAccounts
                .Where(ci => ci.ClientId == ClientId && (ci.InternalAccount.IBAN.Contains(Filter)
                       || ci.InternalAccount.Description.Contains(Filter)));
        }

        public static List<Account> GetAvailableAccounts(int ClientId, Account ExcludedAccount)
        {
            List<Account> myOtherAccounts = new List<Account>();
            if (ExcludedAccount is CurrentAccount)
            {
                var myAccounts = GetAccountsByClient(ClientId);

                var getAllAccounts = Account.GetAll().ToList();

                myOtherAccounts = getAllAccounts.Where(acc => !myAccounts.Any(myAcc => acc.IBAN.Contains(myAcc.IBAN))).ToList();

            }

            return myOtherAccounts;
        }
        
        //Get All Transfer before current date without checking category
        public static List<BankTransfer> GetBankTransfers(Account Account)
        {
            var query = Context.BankTransfers.AsEnumerable()
                .Where(bt => (bt.FromAccount.IBAN.Equals(Account.IBAN) || bt.ToAccount.IBAN.Equals(Account.IBAN)) && bt.DateConcerned <= App.CurrentDate
               ).ToList();

            var result = query.OrderByDescending(bt => bt.DateConcerned).ToList();

            return result;
        }

        public static List<BankTransfer> GetFutureBankTransfers(Account Account)
        {
            var query = Context.BankTransfers.AsEnumerable()
                .Where(bt => (bt.FromAccount.IBAN.Equals(Account.IBAN) || bt.ToAccount.IBAN.Equals(Account.IBAN))
                        && bt.DateConcerned > App.CurrentDate)
               .ToList();

            var result = query.OrderByDescending(bt => bt.DateConcerned).ToList();

            return result;
        }

        // Comme je n'affiche pas le solde dans mes extraits de compte, je filtre a la place par le montant du virement
        public static List<BankTransfer> GetTransfersFilterByText(string Filter, Account Account)
        {
            return Context.BankTransfers.AsEnumerable()
                .Where(bt => bt.DateConcerned <= App.CurrentDate
                      && (bt.FromAccount.IBAN.Equals(Account.IBAN) || bt.ToAccount.IBAN.Equals(Account.IBAN))
                      && (bt.Description.Contains(Filter)
                      || bt.FromAccount.IBAN.Contains(Filter)
                      || bt.ToAccount.IBAN.Contains(Filter)
                      || bt.FromAccount.Description.Contains(Filter)
                      || bt.ToAccount.Description.Contains(Filter)
                      || (bt.User != null && bt.User.UserFullName.Contains(Filter))
                      || bt.Amount.ToString().Contains(Filter)))
                .OrderByDescending(bt => bt.DateConcerned)
                .ToList();
        }

        public static IQueryable<Account> GetMyAccountsByFilter(string Filter, int ClientId, Account AccountToExclude)
        {
            return GetAllAccountsByClientExceptOne(ClientId, AccountToExclude)
                .Where(acc => acc.IBAN.Contains(Filter)
                        || acc.Description.Contains(Filter));

        }

        public static List<Account> GetOtherAccountsByFilter(string Filter, int ClientId, Account AccountToExclude)
        {
            return GetAvailableAccounts(ClientId, AccountToExclude)
                .Where(acc => acc.IBAN.Contains(Filter)
                        || acc.Description.Contains(Filter)).ToList();
        }
    }
}
