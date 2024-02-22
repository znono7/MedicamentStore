using System;
using System.Collections;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MedicamentStore
{
    /// <summary>
    /// Interaction logic for StockHostPage.xaml
    /// </summary>
    public partial class MainSorteStockPage : BasePage<MainSoretStockViewModel> 
    {
        public MainSorteStockPage()
        {
            InitializeComponent();
        } 
        public MainSorteStockPage(MainSoretStockViewModel viewModel) : base(viewModel) 
        {
            InitializeComponent();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text.ToLower();
            foreach (RadioButton button in PanelBtns.Children.OfType<RadioButton>())
            {
                if (button.Content != null && button.Content.ToString().ToLower().Contains(searchText))
                {
                    button.Visibility = Visibility.Visible; // Show the button if it matches the search text
                }
                else
                {
                    button.Visibility = Visibility.Collapsed; // Hide the button if it doesn't match the search text
                }
            }
        }

       

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            ((Image)sender).Source = new BitmapImage(new Uri("pack://application:,,,/Pictures/Lp.jpg", UriKind.RelativeOrAbsolute));
            e.Handled = true;
        }

        private bool isExpanded = false;

        private void expandButton_Click(object sender, RoutedEventArgs e)
        {
            isExpanded = !isExpanded;

            DoubleAnimation rotateAnimation = new DoubleAnimation
            {
                To = isExpanded ? 180 : 0,
                Duration = TimeSpan.FromSeconds(0.3)
            };

            //rotateTransform.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);
        }
    }
}
