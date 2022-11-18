using BankApp.Model;
using PRBD_Framework;

namespace BankApp.ViewModel {
    public abstract class ViewModelCommon : ViewModelBase<User, BankContext> {
        public static bool IsManager => App.IsLoggedIn && App.CurrentUser is Manager;

        public static bool IsNotManager => !IsManager;

        protected bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false;
            }

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }
    }
}
