using BankApp.Model;

using System.Text;

namespace BankApp.ViewModel
{
    public class MainViewModel : ViewModelCommon
    {
        public static StringBuilder Title
        {
            get => GetWindowTitle();
        }

        public MainViewModel()
        {
        }
        protected override void OnRefreshData()
        {
        }

        private static StringBuilder GetWindowTitle()
        {
            StringBuilder title = new StringBuilder($"My Bank ({CurrentUser?.Pseudo} - ");
            title.Append(CurrentUser is Manager ? $"Manager)" : $"Client)");
            return title;
        }
    }
}
