using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using BankApp.Model;

using PRBD_Framework;

namespace BankApp.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : WindowBase
    {
        public MainView()
        {
            InitializeComponent();

            Register<Account>(App.Messages.MSG_NEW_TRANSFER, (acc) =>
                 DoDisplayTransfer(acc));

            Register<Account>(App.Messages.MSG_STATEMENTS,
                acc => DoDisplayStatement(acc));

            Register<DateTime>(App.Messages.MSG_CURRENT_DATE,
                date => Console.WriteLine($"la date a changé depuis le MainView {date}"));

            Register<Account>(App.Messages.MSG_CLOSE_TAB,
                account =>
                {
                    Console.WriteLine("Close tab when cancelling transfer for account : " + account.IBAN);
                    DoCloseTransferTab(account);
                });
        }

        private void DoDisplayStatement(Account account)
        {
            if (account != null){
                Console.WriteLine(account.IBAN);
                OpenTab(account.IBAN, account.IBAN, () => new StatementView(account));
            }
        }

        private void DoDisplayTransfer(Account acc)
        {
            OpenTab("New Transfer", $"transfer{acc.IBAN}", () => new TransferView(acc));
        }

        private void OpenTab(string header, string tag, Func<UserControlBase> createView)
        {
            var tab = tabControl.FindByTag(tag);
            if (tab == null)
                tabControl.Add(createView(), header, tag);
            else
                tabControl.SetFocus(tab);
        }
        private void DoCloseTransferTab(Account account)
        {
            Console.WriteLine("close taaaabbb " + account.IBAN);
            tabControl.CloseByTag($"transfer{account.IBAN}");
        }

        private void MenuLogout_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NotifyColleagues(App.Messages.MSG_LOGOUT);
        }

        private void WindowBase_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q && Keyboard.IsKeyDown(Key.LeftCtrl))
                Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            tabControl.Dispose();
        }
    }
}
