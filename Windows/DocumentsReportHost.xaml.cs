using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices.ActiveDirectory;
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
    /// Interaction logic for DocumentsHost.xaml
    /// </summary>
    public partial class DocumentsReportHost : Window
    {
       
        public DocumentsReportHost(ProduitsPharmaceutiquesType type = ProduitsPharmaceutiquesType.None)
        { 
            InitializeComponent();

            DataContext = new DocumentsHostReportViewModel(this);
            Type = type;
        }

        public ProduitsPharmaceutiquesType Type { get; }
        //public ObservableCollection<MedicamentStock> Stocks { get; set; }
        public double TotalAmount { get; set; }
        private async Task<ObservableCollection<MedicamentStock>> LoadStocksAsync(ProduitsPharmaceutiquesType t)
        {


            var Result = await IoC.StockManager.GetMedicamentStocksAsync(t);
            foreach (var Stock in Result)
            {
                UpdateStatus(Stock);
            }
            TotalAmount = Result.Sum(d => d.PrixTotal);

            return new ObservableCollection<MedicamentStock>(Result.Where(x => x.Quantite > 0));

        }
        private void UpdateStatus(MedicamentStock stock)
        {

            //double x = stock.Prix * stock.Quantite;
            //var s = string.Format("{0:0.00}", x);
            stock.PrixTotal = double.Parse(string.Format("{0:0.00}", stock.Prix * stock.Quantite));

        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var Result = await LoadStocksAsync(Type);
            GetPages(Result);

        }

        private void GetPages(ObservableCollection<MedicamentStock> Stocks)
        {
            if(Stocks.Count <= 14)
            {
                AddPage(Stocks, true, true, "Page 1 sur 1");
                return;
            }
            int firstPageSize = 14;
            int otherPageSize = 20;

            int firstPageCount = 1;/*Math.Min(firstPageSize, Stocks.Count);*/
            int otherPageCount = Math.Max(0, (Stocks.Count - firstPageCount + otherPageSize - 1) / otherPageSize);

            for (int i = 0; i < firstPageCount; i++)
            {
                var pageStocks = new ObservableCollection<MedicamentStock>(Stocks.Skip(i * firstPageSize).Take(firstPageSize));
                bool isFirstPage = true;
                bool isLastPage = false;

                AddPage(pageStocks, isFirstPage, isLastPage, $"Page 1 sur {firstPageCount+otherPageCount}");
            }

            for (int i = 0; i < otherPageCount; i++)
            {
                var pageStocks = new ObservableCollection<MedicamentStock>(Stocks.Skip(firstPageSize + i * otherPageSize).Take(otherPageSize));
                bool isFirstPage = false;
                bool isLastPage = i == otherPageCount - 1;

                AddPage(pageStocks, isFirstPage, isLastPage, $"Page {i+2} sur {firstPageCount + otherPageCount}");
            }
        }

        private void AddPage(ObservableCollection<MedicamentStock> pageStocks, bool isFirstPage, bool isLastPage , string pageNum = "1")
        {
            PrintStockFixedViewModel printStockFixed = new PrintStockFixedViewModel(pageStocks, TotalAmount,
                                                                                       Type.ToProduitsPharmaceutiques(),
                                                                                       isFirstPage, isLastPage , pageNum);

            ProduitStockDocPages page = new ProduitStockDocPages(printStockFixed);
            PageContent pageContent = new PageContent();
            FixedPage fixedPage = new FixedPage();
            fixedPage.Children.Add(page);
            pageContent.Child = fixedPage;
            fixedDocument.Pages.Add(pageContent);
        }


        //private void GetPages()
        //{
        //    int pageSize = 14;
        //    int pageCount = (Stocks.Count + pageSize - 1) / pageSize;

        //    for (int i = 0; i < pageCount; i++)
        //    {
        //        var pageStocks = new ObservableCollection<MedicamentStock>(Stocks.Skip(i * pageSize).Take(pageSize));
        //        bool isFirstPage = i == 0;
        //        bool isLastPage = i == pageCount - 1;

        //        PrintStockFixedViewModel printStockFixed = new PrintStockFixedViewModel(pageStocks, TotalAmount,
        //                                                                               Type.ToProduitsPharmaceutiques(),
        //                                                                               isFirstPage, isLastPage);

        //        ProduitStockDocPages page = new ProduitStockDocPages(printStockFixed);
        //        PageContent pageContent = new PageContent();
        //        FixedPage fixedPage = new FixedPage();
        //        fixedPage.Children.Add(page);
        //        pageContent.Child = fixedPage;
        //        fixedDocument.Pages.Add(pageContent);
        //    }
        //}

        //private void GetPages()
        // {
        //     if (Stocks.Count <= 14)
        //     {
        //         PrintStockFixedViewModel printStockFixed1 = new PrintStockFixedViewModel(Stocks, TotalAmount,
        //                                                                                 Type.ToProduitsPharmaceutiques()
        //                                                                                     ,true, true);
        //         ProduitStockDocPages page1 = new ProduitStockDocPages(printStockFixed1);
        //         PageContent pageContent1 = new PageContent();
        //         FixedPage fixedPage1 = new FixedPage();
        //         fixedPage1.Children.Add(page1);
        //         pageContent1.Child = fixedPage1;
        //         fixedDocument.Pages.Add(pageContent1);
        //     }
        //     else
        //     if(Stocks.Count > 14 && Stocks.Count <= 34)
        //     {
        //         PrintStockFixedViewModel printStockFixed1 = new PrintStockFixedViewModel(new ObservableCollection<MedicamentStock>(Stocks.Take(14)), TotalAmount,
        //                                                                                Type.ToProduitsPharmaceutiques()
        //                                                                                    , true, false);
        //         ProduitStockDocPages page1 = new ProduitStockDocPages(printStockFixed1);
        //         PageContent pageContent1 = new PageContent();
        //         FixedPage fixedPage1 = new FixedPage();
        //         fixedPage1.Children.Add(page1);
        //         pageContent1.Child = fixedPage1;

        //         PrintStockFixedViewModel printStockFixed2 = new PrintStockFixedViewModel(new ObservableCollection<MedicamentStock>(Stocks.Skip(14).Take(20)), TotalAmount,
        //                                                                                Type.ToProduitsPharmaceutiques()
        //                                                                                    , false, true);
        //         ProduitStockDocPages page2 = new ProduitStockDocPages(printStockFixed2);
        //         PageContent pageContent2 = new PageContent();
        //         FixedPage fixedPage2 = new FixedPage();
        //         fixedPage1.Children.Add(page2);
        //         pageContent1.Child = fixedPage2;
        //         fixedDocument.Pages.Add(pageContent1);
        //         fixedDocument.Pages.Add(pageContent2);
        //     }
        //     else 
        //     if(Stocks.Count > 34 && Stocks.Count <= 54)
        //     {
        //         PrintStockFixedViewModel printStockFixed1 = new PrintStockFixedViewModel(new ObservableCollection<MedicamentStock>(Stocks.Take(14)), TotalAmount,
        //                                                                               Type.ToProduitsPharmaceutiques()
        //                                                                                   , true, false);
        //         ProduitStockDocPages page1 = new ProduitStockDocPages(printStockFixed1);
        //         PageContent pageContent1 = new PageContent();
        //         FixedPage fixedPage1 = new FixedPage();
        //         fixedPage1.Children.Add(page1);
        //         pageContent1.Child = fixedPage1;

        //         PrintStockFixedViewModel printStockFixed2 = new PrintStockFixedViewModel(new ObservableCollection<MedicamentStock>(Stocks.Skip(14).Take(20)), TotalAmount,
        //                                                                                Type.ToProduitsPharmaceutiques()
        //                                                                                    , false, false);
        //         ProduitStockDocPages page2 = new ProduitStockDocPages(printStockFixed2);
        //         PageContent pageContent2 = new PageContent();
        //         FixedPage fixedPage2 = new FixedPage();
        //         fixedPage2.Children.Add(page2);
        //         pageContent2.Child = fixedPage2;

        //         PrintStockFixedViewModel printStockFixed3 = new PrintStockFixedViewModel(new ObservableCollection<MedicamentStock>(Stocks.Skip(34).Take(20)), TotalAmount,
        //                                                                               Type.ToProduitsPharmaceutiques()
        //                                                                                   , false, true);
        //         ProduitStockDocPages page3 = new ProduitStockDocPages(printStockFixed3);
        //         PageContent pageContent3 = new PageContent();
        //         FixedPage fixedPage3 = new FixedPage();
        //         fixedPage3.Children.Add(page3);
        //         pageContent3.Child = fixedPage3;

        //         fixedDocument.Pages.Add(pageContent1);
        //         fixedDocument.Pages.Add(pageContent2);
        //         fixedDocument.Pages.Add(pageContent3);
        //     }
        // }
    }
}
