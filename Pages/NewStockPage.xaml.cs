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
    /// Interaction logic for NewStockPage.xaml
    /// </summary>
    public partial class NewStockPage : BasePage<NewStockViewModel>
    {
        public NewStockPage()
        {
            InitializeComponent();
        }
        public NewStockPage(NewStockViewModel viewModel) : base(viewModel) 
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            //InNewStockPage.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            //double currentPageWidth = InNewStockPage.ActualWidth;
            //double currentPageHeight = InNewStockPage.ActualHeight;

            //// Calculate the position above the current page
            //double offsetX = 0; // Set your desired horizontal offset from the left of the screen
            //double offsetY = -currentPageHeight; // Place the new window above the current page
            //// Calculate window position based on the Frame's position
            //Point relativePoint = InNewStockPage.TransformToAncestor(InNewStockPage).Transform(new Point(0, 0));
            //double windowLeft = relativePoint.X + offsetX;
            //double windowTop = relativePoint.Y + offsetY;

            AddStock  newWindow = new AddStock(new StockItemsWindowViewModel());
            //newWindow.Left = windowLeft;
            //newWindow.Top = windowTop;
            //newWindow.Width = currentPageWidth;
            //newWindow.Height = currentPageHeight;
            newWindow.Show();
        }
    }
}
