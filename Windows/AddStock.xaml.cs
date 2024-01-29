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
    /// Interaction logic for AddStock.xaml
    /// </summary>
    public partial class AddStock : Window
    {
        public AddStock(StockItemsWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
          
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            (DataContext as StockItemsWindowViewModel).DimmableOverlayVisible = false;

        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            // Show overlay if we lose focus
            (DataContext as StockItemsWindowViewModel).DimmableOverlayVisible = true;
        }

       
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Attach the KeyDown event handler to the window
            PreviewKeyDown += new KeyEventHandler(HandleEnterKey);
        }

        private void HandleEnterKey(object sender, KeyEventArgs e)
        {
            // Check if the pressed key is Enter
            if (e.Key == Key.Enter)
            {
                (DataContext as StockItemsWindowViewModel).SearchCommand.Execute(null);
            }
        }
    }
}
