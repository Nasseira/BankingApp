using BankApp.Model;

using PRBD_Framework;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace BankApp.ViewModel
{
    class ClientViewModel : ViewModelCommon
    {
        private string _userFullName;

        public string UserFullName
        {
            get { return _userFullName; }
            set { _userFullName = value; }
        }

        private ObservableCollection<ClientInternalAccount> _clientInternalAccounts;
        public ObservableCollection<ClientInternalAccount> ClientInternalAccounts
        {
            get => _clientInternalAccounts;
            set => SetProperty(ref _clientInternalAccounts, value);
        }

        private string _filter;
        public string Filter
        {
            get => _filter;
            set => SetProperty(ref _filter, value, OnRefreshData);
        }

        private bool _currentSelected;
        public bool CurrentSelected
        {
            get => _currentSelected;
            set => SetProperty(ref _currentSelected, value, OnRefreshData);
        }

        private bool _savingSelected;
        public bool SavingSelected
        {
            get => _savingSelected;
            set => SetProperty(ref _savingSelected, value, OnRefreshData);
        }

        private bool _allSelected;
        public bool AllSelected
        {
            get => _allSelected;
            set => SetProperty(ref _allSelected, value, OnRefreshData);
        }

        public ICommand ClearFilter { get; set; }
        public ICommand Transfer { get; set; }
        public ICommand Statements { get; set; }

        public ClientViewModel() : base()
        {
            ClientInternalAccounts = new ObservableCollectionFast<ClientInternalAccount>(ClientInternalAccount.GetClientInternalAccountByClient(App.CurrentUser.Id));

            AllSelected = true;

            UserFullName = App.CurrentUser.UserFullName;

            ClearFilter = new RelayCommand(() => Filter = "");
            Transfer = new RelayCommand<Account>((account) => { NotifyColleagues(App.Messages.MSG_NEW_TRANSFER, account); });

            Statements = new RelayCommand<Account>(acc =>
            {
                NotifyColleagues(App.Messages.MSG_STATEMENTS, acc);
            });

            Register<DateTime>(App.Messages.MSG_CURRENT_DATE, date => OnRefreshData());
            Register(App.Messages.MSG_TRANSFER_DONE, OnRefreshData);
        }

        protected override void OnRefreshData()
        {
            InternalAccount.GetSolde();
            FilterWithCheckboxes();
        }

        private void FilterWithCheckboxes()
        {
            if (_savingSelected && _currentSelected ||
                _savingSelected && _allSelected ||
                _currentSelected && _allSelected)
                return;

            IQueryable<ClientInternalAccount> clientInternalAccounts = string.IsNullOrEmpty(Filter) ?
                ClientInternalAccount.GetClientInternalAccountByClient(App.CurrentUser.Id) : ClientInternalAccount.GetAccountsByFilter(Filter, App.CurrentUser.Id);

            var filteredAccounts = clientInternalAccounts.Where(ci => (ci.InternalAccount.Type == AccountType.Current && CurrentSelected)
                                                                || (ci.InternalAccount.Type == AccountType.Saving && SavingSelected)
                                                                || AllSelected);

            ClientInternalAccounts = new ObservableCollectionFast<ClientInternalAccount>(filteredAccounts);
        }
    }
}
