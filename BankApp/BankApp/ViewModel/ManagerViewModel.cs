using BankApp.Model;

using PRBD_Framework;

using System;
using System.Windows.Input;

namespace BankApp.ViewModel
{
    class ManagerViewModel : ViewModelCommon
    {
        public ManageClientViewModel ManageClientVM { get; set; } = new ManageClientViewModel();
        public ManageAccountsViewModel ManageAccountsVM { get; set; } = new ManageAccountsViewModel();

        private ObservableCollectionFast<Agency> _agencies;

        public ObservableCollectionFast<Agency> Agencies
        {
            get => _agencies;
            set => SetProperty(ref _agencies, value);
        }

        private Agency _agencySelected;

        public Agency AgencySelected
        {
            get => _agencySelected;
            set => SetProperty(ref _agencySelected, value, OnRefreshData);
        }

        private ObservableCollectionFast<Client> _clients;

        public ObservableCollectionFast<Client> Clients
        {
            get => _clients;
            set => SetProperty(ref _clients, value);
        }

        private Client _clientSelected;

        public Client ClientSelected
        {
            get => _clientSelected;
            set => SetProperty(ref _clientSelected, value, () =>
            {
                ManageClientVM.ClientSelected = value;
                ManageAccountsVM.ClientSelected = value;
                RaisePropertyChanged();
            });
        }

        private bool _isNew;

        public bool IsNew
        {
            get => _isNew;
            set => SetProperty(ref _isNew, value);
        }

        public ICommand Save { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Delete { get; set; }
        public ICommand NewClient { get; set; }
        public ICommand ViewAccountDetails { get; set; }

        public ManagerViewModel()
        {
            Agencies = new ObservableCollectionFast<Agency>(Agency.GetAllByManager(App.CurrentUser.Id));
            //Clients = new ObservableCollectionFast<Client>(Agency.GetClientByAgency(AgencySelected));
            NewClient = new RelayCommand(AddNewClient, CanAddNewClient);
            Delete = new RelayCommand(DeleteAction, CanDeleteAction);
            Cancel = new RelayCommand(CancelAction);
            Save = new RelayCommand(SaveAction, CanSaveAction);
            ViewAccountDetails = new RelayCommand<Account>(acc =>
            {
                NotifyColleagues(App.Messages.MSG_STATEMENTS, acc);
            });
        }

        protected override void OnRefreshData()
        {
            if (AgencySelected != null)
            {
                Console.WriteLine("Agency selected :" + AgencySelected.Name);
                Clients = new ObservableCollectionFast<Client>(Agency.GetClientByAgency(AgencySelected));
            }
        }

        private void AddNewClient()
        {
            ClearErrors();
            if (AgencySelected != null)
            {
                ClientSelected = new Client();
                Clients.Add(ClientSelected);
                IsNew = true;
                RaisePropertyChanged();
            }
        }

        private bool CanAddNewClient()
        {
            return AgencySelected != null && !IsNew;
        }

        private void DeleteAction()
        {
            //le delete ne sert que pr le client existant
            if (ClientSelected != null)
            {
                if (!IsNew)
                {
                    Context.Clients.Remove(ClientSelected);
                    Context.SaveChanges();
                    ManageAccountsVM.ClientAccounts = null;
                    ManageClientVM.ConfirmPassword = "";
                }

                Clients = new ObservableCollectionFast<Client>(Agency.GetClientByAgency(AgencySelected));

                RaisePropertyChanged();
            }
        }

        private bool CanDeleteAction()
        {
            return ClientSelected != null && !IsNew;
        }

        public override void CancelAction()
        {
            ClearErrors();
            // Reset le Client selectionné
            if (IsNew)
            {
                Clients.Remove(ClientSelected);
                ClientSelected = null;
                ManageClientVM.ConfirmPassword = "";
            }
            else
            {
                //retire le compte rajouté dans la liste des comptes des clients et replace ce compte dans la liste des comptes dispo
                if (ManageAccountsVM.NewClientInternalAccount != null)
                {
                    ManageAccountsVM.NewAccounts = new ObservableCollectionFast<InternalAccount>(InternalAccount.GetOthersInternalAccounts(ClientSelected.Id));
                    ManageAccountsVM.ClientAccounts.Remove(ManageAccountsVM.NewClientInternalAccount);
                }
            }
            ClearErrors();
            IsNew = false;
            RaisePropertyChanged();
        }

        public override void SaveAction()
        {
            if (ManageClientVM.Validate() || ManageAccountsVM.Validate())
            {
                //si le client est nouveau il faut le rajouter en db
                if (IsNew)
                {
                    ClientSelected.Agency = AgencySelected;
                    Context.Clients.Add(ClientSelected);
                    Context.SaveChanges();
                    Clients = new ObservableCollectionFast<Client>(Agency.GetClientByAgency(AgencySelected));

                    //je remets a false pour reactiver le btn New Client
                    IsNew = false;
                }
                else
                {
                    // je sauve dans ma db pr mon client selectionné le nouvel acces au compte
                    if (ManageAccountsVM.NewClientInternalAccount != null)
                    {
                        Context.ClientInternalAccounts.Add(ManageAccountsVM.NewClientInternalAccount);
                        Context.SaveChanges();
                    }
                }
                RaisePropertyChanged();
                Context.SaveChanges();
            }
        }

        private bool CanSaveAction()
        {
            return ClientSelected != null && ManageClientVM.Validate();
        }
    }
}
