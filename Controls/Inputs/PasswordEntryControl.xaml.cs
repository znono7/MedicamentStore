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

namespace MedicamentStore
{
    /// <summary>
    /// Interaction logic for TextEntryControl.xaml
    /// </summary>
    public partial class PasswordEntryControl : UserControl
    {
        public PasswordEntryControl()
        {
            InitializeComponent();
            DataContext = new PasswordEntryViewModel();
        }

        private void NewPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is PasswordEntryViewModel viewModel)
                viewModel.Password = NewPassword.SecurePassword;
        }
    }
}
