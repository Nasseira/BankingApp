using BankApp.Model;

using PRBD_Framework;

using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace BankApp.ViewModel
{
    class TransferViewModel : ViewModelCommon
    {
        private BankTransfer _bankTransfer;

        public BankTransfer BankTransfer
        {
            get => _bankTransfer;
            set => SetProperty(ref _bankTransfer, value);
        }

        private ObservableCollection<Account> _fromAccounts;

        public ObservableCollection<Account> FromAccounts
        {
            get => _fromAccounts;
            set => SetProperty(ref _fromAccounts, value);
        }

        private Account _fromAccountSelected;

        public Account FromAccountSelected
        {
            get => _fromAccountSelected;
            set => SetProperty(ref _fromAccountSelected, value,
                () =>
                {
                    Validate();
                });
        }

        public Account FromAccountSelectedSave { get; set; }

        private ObservableCollection<Account> _toAccounts;
        public ObservableCollection<Account> ToAccounts
        {
            get => _toAccounts;
            set => SetProperty(ref _toAccounts, value);
        }

        private Account _toAccountSelected;
        public Account ToAccountSelected
        {
            get => _toAccountSelected;
            set => SetProperty(ref _toAccountSelected, value, () => Validate());
        }

        private double _amount;
        public double Amount
        {
            get => _amount;
            set => SetProperty(ref _amount, value, () => Validate());
        }

        private string _description;

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value, () => Validate());
        }

        private DateTime? _effectDate;

        public DateTime? EffectDate
        {
            get => _effectDate;
            set => SetProperty(ref _effectDate, value, () => Validate());
        }

        private ObservableCollection<Category> _categories;

        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        private Category _category;

        public Category Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }

        public ICommand Cancel { get; set; }
        public ICommand Transfer { get; set; }
        public ICommand SelectDestinationAccount { get; set; }

        public TransferViewModel() : base()
        {
            Categories = new ObservableCollectionFast<Category>(Category.GetAll());
            FromAccounts = new ObservableCollectionFast<Account>(ClientInternalAccount.GetAccountsByClient(App.CurrentUser.Id));
            SelectDestinationAccount = new RelayCommand<InternalAccount>(account =>
            {
                App.ShowDialog<AccountListDialogViewModel, User, BankContext>(account);
            });
            FromAccountSelectedSave = FromAccountSelected;

            Transfer = new RelayCommand(SaveAction, CanSaveAction);
            Cancel = new RelayCommand<Account>((acc) =>
            {
                ClearErrors();
                NotifyColleagues(App.Messages.MSG_CLOSE_TAB, FromAccountSelectedSave);
            });

            Register<Account>(App.Messages.MSG_ACCOUNT_CHANGED, acc => UpdateToAccount(acc));
        }

        public void UpdateToAccount(Account acc)
        {
            if (acc != null)
            {
                Console.WriteLine("IBAN que j'ai récup de mon dialog : " + acc.IBAN);
                ToAccountSelected = acc;
                RaisePropertyChanged(nameof(ToAccountSelected));
            }
        }

        protected override void OnRefreshData()
        {
        }

        public override void SaveAction()
        {
            BankTransfer = new BankTransfer(FromAccountSelected, ToAccountSelected, Amount, Description, App.CurrentUser, App.CurrentDate, EffectDate, Category); //remplir le constructeur
            Context.BankTransfers.Add(BankTransfer);
            Context.SaveChanges();
            NotifyColleagues(App.Messages.MSG_CLOSE_TAB, FromAccountSelectedSave);
            NotifyColleagues(App.Messages.MSG_TRANSFER_DONE);
        }

        private bool CanSaveAction()
        {
            return Amount > 0 && ToAccountSelected != null && FromAccountSelected != null && !string.IsNullOrWhiteSpace(Description); //&& EffectDate >= App.CurrentDate);
        }

        public void Init(Account account)
        {
            FromAccountSelectedSave = account;
            RaisePropertyChanged();
        }

        public override bool Validate()
        {
            ClearErrors();

            if (FromAccountSelected == null)
                AddError(nameof(FromAccountSelected), "required");

            if (ToAccountSelected == null)
                AddError(nameof(ToAccountSelected), "required");

            if (Amount <= 0)
                AddError(nameof(Amount), "should be greater than 0");
            else if (Amount > (FromAccountSelected.Solde - FromAccountSelected.FloorAmount) && EffectDate == null)
                AddError(nameof(Amount), $"balance is insufficient, maximum allowed transfer is {FromAccountSelected.Solde - FromAccountSelected.FloorAmount} €");

            if (string.IsNullOrWhiteSpace(Description) || Description.Length == 0)
                AddError(nameof(Description), "required");

            if (EffectDate < App.CurrentDate)
                AddError(nameof(EffectDate), "should be after CurrentDate");
            RaiseErrors();
            return !HasErrors;
        }

    }
}
