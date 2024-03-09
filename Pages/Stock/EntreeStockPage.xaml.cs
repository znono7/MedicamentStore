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
    public partial class EntreeStockPage : BasePage 
    {
        public EntreeStockPage()
        {
            InitializeComponent();
        } 
        public EntreeStockPage(EntreeStockViewModel viewModel) 
        {
            InitializeComponent();
            DataContext = viewModel;
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

            rotateTransform.BeginAnimation(RotateTransform.AngleProperty, rotateAnimation);
        }
    }
}
