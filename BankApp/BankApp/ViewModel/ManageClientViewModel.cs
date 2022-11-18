using BankApp.Model;

namespace BankApp.ViewModel
{
    class ManageClientViewModel : ViewModelCommon
    {

        public string Pseudo
        {
            get => ClientSelected?.Pseudo;
            set => SetProperty(ClientSelected.Pseudo, value, ClientSelected, (c, v) =>
            {
                c.Pseudo = v;
                Validate();
            });
        }

        public string Firstname
        {
            get => ClientSelected?.Firstname;
            set => SetProperty(ClientSelected.Firstname, value, ClientSelected, (c, v) =>
            {
                c.Firstname = v;
                Validate();
            });
        }

        public string Lastname
        {
            get => ClientSelected?.Lastname;
            set => SetProperty(ClientSelected.Lastname, value, ClientSelected, (c, v) =>
            {
                c.Lastname = v;
                Validate();
            });
        }

        public string Email
        {
            get => ClientSelected?.Email;
            set => SetProperty(ClientSelected.Email, value, ClientSelected, (c, v) =>
            {
                c.Email = v;
                Validate();
            });
        }

        public string Password
        {
            get => ClientSelected?.Password;
            set => SetProperty(ClientSelected.Password, value, ClientSelected, (c, v) =>
            {
                c.Password = v;
                Validate();
            });
        }

        private string _confirmPassword;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value, () =>
            {
                Validate();
            });
        }

        private bool _isEditable;
        public bool IsEditable
        {
            get => _isEditable;
            set => SetProperty(ref _isEditable, value);
        }

        private Client _clientSelected;

        public Client ClientSelected
        {
            get => _clientSelected;
            set => SetProperty(ref _clientSelected, value, OnRefreshData);
        }

        //public User UserEmail { get; set; } = new Client();

        public ManageClientViewModel()
        {
        }

        protected override void OnRefreshData()
        {
            EditionMode();
            RaisePropertyChanged();
        }

        private void EditionMode()
        {
            if (ClientSelected != null)
            {
                IsEditable = true;
                ConfirmPassword = Password;
                //UserEmail.Email = User.GetEmailByUser(ClientSelected.Id);
            }
            else
            {
                IsEditable = false;
            }
        }

        public override bool Validate()
        {
            ClearErrors();
            //if(UserEmail!=null)
            //Console.WriteLine("userEmail " + UserEmail.Email);
            //Console.WriteLine("client " + ClientSelected.Email);
            if (string.IsNullOrWhiteSpace(Pseudo))
                AddError(nameof(Pseudo), "required");

            if (string.IsNullOrWhiteSpace(Firstname))
                AddError(nameof(Firstname), "required");

            if (string.IsNullOrWhiteSpace(Lastname))
                AddError(nameof(Lastname), "required");

            if (string.IsNullOrWhiteSpace(Email))
                AddError(nameof(Email), "required");
            else if (!IsValidEmail(Email))
                AddError(nameof(Email), "invalid format");
            //else if (!Email.Equals(UserEmail) && User.GetUserByEmail(Email) != null)
            //    AddError(nameof(Email), "email already exist");

            if (string.IsNullOrWhiteSpace(Password))
                AddError(nameof(Password), "required");
            else if (!Password.Equals(ConfirmPassword))
                AddError(nameof(Password), "passwords should be the same");

            if (string.IsNullOrWhiteSpace(ConfirmPassword))
                AddError(nameof(ConfirmPassword), "required");
            else if (!Password.Equals(ConfirmPassword))
                AddError(nameof(ConfirmPassword), "passwords should be the same");

            RaiseErrors();
            return !HasErrors;
        }
    }
}
