using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using BankApp.Model;
using PRBD_Framework;
using BankApp.ViewModel;
using System.Windows.Markup;
using System.Globalization;

namespace BankApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : ApplicationBase<User, BankContext>
    {
        public enum Messages
        {
            MSG_NEW_TRANSFER,
            MSG_STATEMENTS,
            MSG_CLOSE_TAB,
            MSG_LOGIN,
            MSG_CURRENT_DATE,
            MSG_ACCOUNT_CHANGED,
            MSG_TRANSFER_DONE,
            MSG_FROM_ACC_CHANGED,
            MSG_LOGOUT
        }

        private static DateTime _currentDate = DateTime.Now;
        public static DateTime CurrentDate
        {
            get => _currentDate;
            set
            {
                _currentDate = value;
                NotifyColleagues(Messages.MSG_CURRENT_DATE, value);
                InternalAccount.GetSolde();
            }
        }


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Context.Database.EnsureDeleted();
            Context.Database.EnsureCreated();
            Context.SeedData();
            ClearContext();

            Register<User>(this, Messages.MSG_LOGIN, user =>
            {
                Login(user);
                NavigateTo<MainViewModel, User, BankContext>();
            });

            Register(this, Messages.MSG_LOGOUT, () =>
            {
                Logout();
                NavigateTo<LoginViewModel, User, BankContext>();
            });

            FrameworkElement.LanguageProperty.OverrideMetadata(
             typeof(FrameworkElement),
             new FrameworkPropertyMetadata(
             XmlLanguage.GetLanguage(
             CultureInfo.CurrentCulture.IetfLanguageTag)));
        }

        protected override void OnRefreshData()
        {
        }
    }
}
