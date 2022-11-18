using BankApp.Model;

using PRBD_Framework;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace BankApp.ViewModel
{
    class AccountListDialogViewModel : DialogViewModelBase<User, BankContext>
    {
        private ObservableCollection<Account> _myAccounts;

        public ObservableCollection<Account> MyAccounts
        {
            get => _myAccounts;
            set => SetProperty(ref _myAccounts, value);
        }

        private ObservableCollection<Account> _otherAccounts;

        public ObservableCollection<Account> OtherAccounts
        {
            get => _otherAccounts;
            set => SetProperty(ref _otherAccounts, value);
        }

        private Account _accountToExclude;

        public Account AccountToExclude
        {
            get => _accountToExclude;
            set => SetProperty(ref _accountToExclude, value);
        }

        private Account _toAccountSelected;

        public Account ToAccountSelected
        {
            get { return _toAccountSelected; }
            set
            {
                SetProperty(ref _toAccountSelected, value);
                Console.WriteLine("toto " + value);
            }
        }

        private string _filter;
        public string Filter
        {
            get => _filter;
            set => SetProperty(ref _filter, value, OnRefreshData);
        }

        public ICommand Confirm { get; set; }

        public AccountListDialogViewModel()
        {
            Confirm = new RelayCommand<Account>(acc =>
            {
                if (acc != null) Console.WriteLine(acc.IBAN);
                NotifyColleagues(App.Messages.MSG_ACCOUNT_CHANGED, acc);
                DialogResult = true;
            });

        }
        public void Init(Account account)
        {
            AccountToExclude = account;
            MyAccounts = new ObservableCollectionFast<Account>(ClientInternalAccount.GetAllAccountsByClientExceptOne(App.CurrentUser.Id, AccountToExclude));
            OtherAccounts = new ObservableCollectionFast<Account>(ClientInternalAccount.GetAvailableAccounts(App.CurrentUser.Id, AccountToExclude));
        }

        protected override void OnRefreshData()
        {
            FilterAccounts();
        }

        private void FilterAccounts()
        {
            IQueryable<Account> myAccountsFiltered = string.IsNullOrEmpty(Filter) ?
                ClientInternalAccount.GetAllAccountsByClientExceptOne(App.CurrentUser.Id, AccountToExclude) : ClientInternalAccount.GetMyAccountsByFilter(Filter, App.CurrentUser.Id, AccountToExclude);
            List<Account> otherAccountsFiltered = string.IsNullOrEmpty(Filter) ?
                ClientInternalAccount.GetAvailableAccounts(App.CurrentUser.Id, AccountToExclude) : ClientInternalAccount.GetOtherAccountsByFilter(Filter, App.CurrentUser.Id, AccountToExclude);
            
            MyAccounts = new ObservableCollection<Account>(myAccountsFiltered);
            OtherAccounts = new ObservableCollection<Account>(otherAccountsFiltered);
        }
    }
}
