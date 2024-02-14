using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace MedicamentStore
{
    /// <summary>
    /// Interaction logic for PrintMovmentStockPage.xaml
    /// </summary>
    public partial class PrintSorteStockPage : BasePage
    {
        public PrintSorteStockPage(PrintSorteStockViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        private void BasePage_Loaded(object sender, RoutedEventArgs e)
        {
            GetPages();
        }

        private void GetPages()
        {
            var vModel = DataContext as PrintSorteStockViewModel;

            if(vModel.Stocks.Count <= 14)
            {
                AddPage(vModel.Stocks, true, true, "Page 1 sur 1");
                return;
            }
            int firstPageSize = 14;
            int otherPageSize = 20;

            int firstPageCount = 1;/*Math.Min(firstPageSize, Stocks.Count);*/
            int otherPageCount = Math.Max(0, (vModel.Stocks.Count - firstPageCount + otherPageSize - 1) / otherPageSize);
            for (int i = 0; i < firstPageCount; i++)
            {
                var pageStocks = new ObservableCollection<TransactionDto>(vModel.Stocks.Skip(i * firstPageSize).Take(firstPageSize));
                bool isFirstPage = true;
                bool isLastPage = false;

                AddPage(pageStocks, isFirstPage, isLastPage, $"Page 1 sur {firstPageCount + otherPageCount}");
            }

            for (int i = 0; i < otherPageCount; i++)
            {
                var pageStocks = new ObservableCollection<TransactionDto>(vModel.Stocks.Skip(firstPageSize + i * otherPageSize).Take(otherPageSize));
                bool isFirstPage = false;
                bool isLastPage = i == otherPageCount - 1;

                AddPage(pageStocks, isFirstPage, isLastPage, $"Page {i + 2} sur {firstPageCount + otherPageCount}");
            }
        }

        private void AddPage(ObservableCollection<TransactionDto> pageStocks, bool isFirstPage, bool isLastPage, string pageNum = "1")
        {
            SorteStockDocViewModel model = new SorteStockDocViewModel(pageStocks , isFirstPage, isLastPage, pageNum);

            SorteStockDocControl page = new SorteStockDocControl(model);
            PageContent pageContent = new PageContent();
            FixedPage fixedPage = new FixedPage();
            fixedPage.Children.Add(page);
            pageContent.Child = fixedPage;
            fixedDocument.Pages.Add(pageContent);
        }
    }
}
