using BankApp.Model;

using PRBD_Framework;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace BankApp.ViewModel
{
    public enum TransferDuration
    {
        OneDay,
        OneWeek,
        TwoWeeks,
        FourWeeks,
        OneYear,
        All
    }
    class StatementViewModel : ViewModelCommon
    {
        private ObservableCollection<BankTransfer> _bankTransfers;

        public ObservableCollection<BankTransfer> BankTransfers
        {
            get => _bankTransfers;
            set => SetProperty(ref _bankTransfers, value);
        }

        private Account _selectedAccount;

        public Account SelectedAccount
        {
            get => _selectedAccount;
            set => SetProperty(ref _selectedAccount, value);
        }

        private ObservableCollection<Category> _categories;

        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        private ObservableCollection<CheckedCategory> _checkCategories;

        public ObservableCollection<CheckedCategory> CheckCategories
        {
            get => _checkCategories;
            set => SetProperty(ref _checkCategories, value, RaisePropertyChanged);
        }

        public ObservableCollectionFast<TransferDuration> TransferDurations { get; set; } = new ObservableCollectionFast<TransferDuration>
            {
                TransferDuration.OneDay,
                TransferDuration.OneWeek,
                TransferDuration.TwoWeeks,
                TransferDuration.FourWeeks,
                TransferDuration.OneYear,
                TransferDuration.All
            };

        private TransferDuration _transferDurationSelected;
        public TransferDuration TransferDurationSelected
        {
            get => _transferDurationSelected;
            set => SetProperty(ref _transferDurationSelected, value, OnRefreshData);
        }

        private bool _futureTransactions;

        public bool FutureTransactions
        {
            get => _futureTransactions;
            set => SetProperty(ref _futureTransactions, value, OnRefreshData);
        }

        private bool _pastTransactions;

        public bool PastTransactions
        {
            get => _pastTransactions;
            set => SetProperty(ref _pastTransactions, value, OnRefreshData);
        }

        private bool _refusedTransactions;

        public bool RefusedTransactions
        {
            get => _refusedTransactions;
            set => SetProperty(ref _refusedTransactions, value, OnRefreshData);
        }

        private string _filter;
        public string Filter
        {
            get => _filter;
            set => SetProperty(ref _filter, value, OnRefreshData);
        }

        public ICommand CategorySelected { get; set; }
        public ICommand CancelFutureTransfer { get; set; }
        public ICommand CheckAll { get; set; }
        public ICommand UnCheckAll { get; set; }
        public StatementViewModel() : base()
        {
            Categories = new ObservableCollectionFast<Category>(Category.GetAll());
            Register<DateTime>(App.Messages.MSG_CURRENT_DATE, (date) => OnRefreshData());
            CategorySelected = new RelayCommand<BankTransfer>((bt) => UpdateCategory(bt));
            CancelFutureTransfer = new RelayCommand<BankTransfer>((bt) => CancelTransfer(bt));
            CheckCategories = new ObservableCollectionFast<CheckedCategory>(CheckedCategory.CheckedCategoriesList());
            UnCheckAll = new RelayCommand(() => { UnCheckAllCategoriesCB(); OnRefreshData(); });
            CheckAll = new RelayCommand(() => { CheckAllCategoriesCB(); OnRefreshData(); });
        }

        protected override void OnRefreshData()
        {
            CheckCategories =  new ObservableCollectionFast<CheckedCategory>(GetCheckedCategories());
            FilterStatement();
        }

        public void Init(Account account)
        {
            SelectedAccount = account;
            TransferDurationSelected = TransferDurations[TransferDurations.Count - 1];
            // Filter on Action !
            PastTransactions = true;
            RaisePropertyChanged();
        }

        private void UpdateCategory(BankTransfer bt)
        {
            var transfer = Context.BankTransfers.Find(bt.Id);
            transfer.Category = bt.Category;
            Context.SaveChanges();
            OnRefreshData();
        }

        private void CancelTransfer(BankTransfer bt)
        {
            Context.BankTransfers.Remove(bt);
            Context.SaveChanges();
            OnRefreshData();
        }

        private void FilterStatement()
        {
            List<BankTransfer> bankTransfers = string.IsNullOrEmpty(Filter) ?
                ClientInternalAccount.GetBankTransfers(SelectedAccount) : ClientInternalAccount.GetTransfersFilterByText(Filter, SelectedAccount);
            List<BankTransfer> filteredTransfers = new List<BankTransfer>();
            if (PastTransactions)
            {
                filteredTransfers = bankTransfers.Where(bt => bt.DateConcerned <= App.CurrentDate && bt.IsValidStatement).ToList();
                if (RefusedTransactions)
                {
                    filteredTransfers = bankTransfers;
                }
                // Get Future and Past Or Future and Past and Refused
                if (FutureTransactions)
                {
                    filteredTransfers = ClientInternalAccount.GetFutureBankTransfers(SelectedAccount).Concat(filteredTransfers).ToList();
                }
                if (!TransferDurationSelected.Equals(TransferDuration.All))
                {
                    filteredTransfers = filteredTransfers.Where(bt => bt.DateConcerned > CalculateDate(TransferDurationSelected)).ToList();
                }
            }
            else if (FutureTransactions)
            {
                filteredTransfers = ClientInternalAccount.GetFutureBankTransfers(SelectedAccount);
                RefusedTransactions = false;
            }
            else
            {
                RefusedTransactions = false;
            }
            BankTransfers = new ObservableCollectionFast<BankTransfer>(filteredTransfers);
        }

        private int GetNbDaysFromSelectedDuration(TransferDuration DurationSelected)
        {
            int duration = 0;
            if (DurationSelected.Equals(TransferDuration.OneDay))
                duration = 1;
            else if (DurationSelected.Equals(TransferDuration.OneWeek))
                duration = 7;
            else if (DurationSelected.Equals(TransferDuration.TwoWeeks))
                duration = 14;
            else if (DurationSelected.Equals(TransferDuration.FourWeeks))
                duration = 30;
            else if (DurationSelected.Equals(TransferDuration.OneYear))
                duration = 365;

            return duration;
        }
        private DateTime CalculateDate(TransferDuration DurationSelected)
        {
            int nbDays = GetNbDaysFromSelectedDuration(DurationSelected);
            return App.CurrentDate.Subtract(TimeSpan.FromDays(nbDays));
        }

        private void CheckAllCategoriesCB()
        {
            foreach (var cat in CheckCategories)
            {
                cat.IsChecked = true;
            }
        }

        private void UnCheckAllCategoriesCB()
        {
            foreach (var cat in CheckCategories)
            {
                cat.IsChecked = false;
            }
        }

        private ObservableCollection<CheckedCategory> GetCheckedCategories()
        {
            return CheckCategories;
        }
    }
}

