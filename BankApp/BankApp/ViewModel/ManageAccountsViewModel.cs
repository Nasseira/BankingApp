
using BankApp.Model;

using PRBD_Framework;

using System.Windows.Input;

namespace BankApp.ViewModel
{
    class ManageAccountsViewModel : ViewModelCommon
    {
        private ObservableCollectionFast<ClientInternalAccount> _clientAccounts;

        public ObservableCollectionFast<ClientInternalAccount> ClientAccounts
        {
            get => _clientAccounts;
            set => SetProperty(ref _clientAccounts, value);
        }

        private Client _clientSelected;

        public Client ClientSelected
        {
            get => _clientSelected;
            set => SetProperty(ref _clientSelected, value, OnRefreshData);
        }

        private ObservableCollectionFast<ClientRole> _clientRoles;

        public ObservableCollectionFast<ClientRole> ClientRoles
        {
            get => _clientRoles;
            set => SetProperty(ref _clientRoles, value);
        }

        private ObservableCollectionFast<InternalAccount> _newAccounts;

        public ObservableCollectionFast<InternalAccount> NewAccounts
        {
            get => _newAccounts;
            set => SetProperty(ref _newAccounts, value);
        }

        private InternalAccount _newAccountSelected;

        public InternalAccount NewAccountSelected
        {
            get => _newAccountSelected;
            set => SetProperty(ref _newAccountSelected, value);
        }

        private ClientInternalAccount _newClientInternalAccount;

        public ClientInternalAccount NewClientInternalAccount
        {
            get => _newClientInternalAccount;
            set => SetProperty(ref _newClientInternalAccount, value);
        }


        private ClientRole _clientRoleSelected;

        public ClientRole ClientRoleSelected
        {
            get => _clientRoleSelected;
            set => SetProperty(ref _clientRoleSelected, value);
        }

        public ICommand AddAccount { get; set; }

        public ManageAccountsViewModel()
        {
            AddAccount = new RelayCommand(AddAccountAction, CanAddAccountAction);
            RaisePropertyChanged();
        }

        protected override void OnRefreshData()
        {
            if (ClientSelected != null)
            {
                RaisePropertyChanged();
                ClientAccounts = new ObservableCollectionFast<ClientInternalAccount>(ClientInternalAccount.GetClientInternalAccountByClient(ClientSelected.Id));
                NewAccounts = new ObservableCollectionFast<InternalAccount>(InternalAccount.GetOthersInternalAccounts(ClientSelected.Id));
                ClientRoles = new ObservableCollectionFast<ClientRole> { ClientRole.Agent, ClientRole.Holder };
            }
        }

        private void AddAccountAction()
        {
            if (NewAccountSelected != null)
            {
                NewClientInternalAccount = new ClientInternalAccount(ClientSelected, NewAccountSelected, ClientRoleSelected);
                ClientAccounts.Add(NewClientInternalAccount);
                NewAccounts.Remove(NewAccountSelected);
            }
            RaisePropertyChanged();
        }

        private bool CanAddAccountAction()
        {
            return NewAccountSelected != null;
        }
    }
}
