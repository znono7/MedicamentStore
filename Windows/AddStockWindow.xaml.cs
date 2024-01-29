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
using System.Windows.Shapes;

namespace MedicamentStore
{
    /// <summary>
    /// Interaction logic for AddStockWindow.xaml
    /// </summary>
    public partial class AddStockWindow : Window
    {
        public AddStockWindow( AddStockWindowViewModel viewModel) 
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Check if the input is a number
            if (!IsNumeric(e.Text))
            {
                e.Handled = true; // Cancel the input
            }
        }

        private bool IsNumeric(string input)
        {
            return int.TryParse(input, out _);
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            await ((AddStockWindowViewModel)DataContext).UpdateQ();
            Close();
        }
    }
}
