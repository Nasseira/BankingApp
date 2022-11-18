using BankApp.Model;

using System;
using System.Collections.Generic;
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
using PRBD_Framework;

namespace BankApp.View
{
    /// <summary>
    /// Interaction logic for TransferView.xaml
    /// </summary>
    public partial class TransferView : UserControlBase
    {
        public TransferView(Account acc)
        {
            InitializeComponent();
            Register<DateTime>(App.Messages.MSG_CURRENT_DATE,
                date => Console.WriteLine($"la date a changé depuis le TransferView {date}"));
            vm.FromAccountSelected = acc;
            vm.Init(acc);
        }
    }
}
