using System;
using System.Windows.Input;

using BankApp.Model;

using PRBD_Framework;

namespace BankApp.ViewModel
{
    public class LoginViewModel : ViewModelCommon
    {
        private string _email;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value, () => Validate());
        }

        private string _password;

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value, () => Validate());
        }

        public ICommand LoginCommand { get; set; }

        public LoginViewModel() : base()
        {
            LoginCommand = new RelayCommand(LoginAction,
                () => { return _email != null && _password != null && !HasErrors; });
        }
        private void LoginAction()
        {
            if (Validate())
            {
                var user = User.GetUserByEmail(Email);
                NotifyColleagues(App.Messages.MSG_LOGIN, user);
            }
        }
        protected override void OnRefreshData()
        {
        }
        public override bool Validate()
        {
            ClearErrors();

            var user = User.GetUserByEmail(Email);
            
            if (string.IsNullOrWhiteSpace(Email))
                AddError(nameof(Email), "required");
            else if (!IsValidEmail(Email))
                AddError(nameof(Email), "invalid format");
            else if (user == null)
                AddError(nameof(Email), "does not exist");
            else
            {
                if (string.IsNullOrEmpty(Password))
                    AddError(nameof(Password), "required");
                else if (Password.Length < 3)
                    AddError(nameof(Password), "length must be >= 3");
                else if (user != null && user.Password != Password)
                    AddError(nameof(Password), "wrong password");
            }

            return !HasErrors;
        }
    }
}
