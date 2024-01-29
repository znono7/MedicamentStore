using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for PassEntryEdited.xaml
    /// </summary>
    public partial class PassEntryEdited : UserControl
    {
        #region Dependency Properties

        /// <summary>
        /// The label width of the control
        /// </summary>
        public GridLength LabelWidth
        {
            get => (GridLength)GetValue(LabelWidthProperty);
            set => SetValue(LabelWidthProperty, value);
        }

        // Using a DependencyProperty as the backing store for LabelWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelWidthProperty =
            DependencyProperty.Register("LabelWidth", typeof(GridLength), typeof(PassEntryEdited), new PropertyMetadata(GridLength.Auto, LabelWidthChangedCallback));

        #endregion
        public PassEntryEdited()
        {
            InitializeComponent();
            DataContext = new PasswordEntryEditedViewModel();
        }
        #region Dependency Callbacks

        /// <summary>
        /// Called when the label width has changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void LabelWidthChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                // Set the column definition width to the new value
                (d as PassEntryEdited).LabelColumnDefinition.Width = (GridLength)e.NewValue;
            }
            catch 
            {
                // Make developer aware of potential issue
                Debugger.Break();

                (d as PassEntryEdited).LabelColumnDefinition.Width = GridLength.Auto;
            }
        }

        #endregion
        private void CurrentPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is PasswordEntryEditedViewModel viewModel)
                viewModel.CurrentPassword = CurrentPassword.SecurePassword;
        }

        private void NewPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is PasswordEntryEditedViewModel viewModel)
                viewModel.NewPassword = NewPassword.SecurePassword;
        }

        private void ConfirmPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is PasswordEntryEditedViewModel viewModel)
                viewModel.ConfirmPassword = ConfirmPassword.SecurePassword;
        }
    }
}
