using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MedicamentStore
{
    public class MenuViewModel : BaseViewModel
    {

        public ICommand? HomePage { get; set; }
        public ICommand? StockPage { get; set; }
        public ICommand? InvoicePage { get; set; }
        public ICommand? MouvementPage { get; set; }
        public ICommand? EntreeStockPage { get; set; }
        public ICommand? FacturePage { get; set; }

        private ApplicationPage _currentPage;

        // public ApplicationPage CurrentPage { get; set; }
        public ApplicationPage CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    IoC.Application.GoToPage(value);
                }
            }
        }

        public MenuViewModel()
        {
            HomePage = new RelayCommand(async () => await NavigateToPageAsync(ApplicationPage.Home));
            StockPage = new RelayCommand(async () => await NavigateToPageAsync(ApplicationPage.StockHostPage));
            InvoicePage = new RelayCommand(async () => await NavigateToPageAsync(ApplicationPage.MainSorteStockPage));
            MouvementPage = new RelayCommand(async () => await NavigateToPageAsync(ApplicationPage.MainMovmentStockPage));
            EntreeStockPage = new RelayCommand(async () => await NavigateToPageAsync(ApplicationPage.MainEntreeStockPage));
            FacturePage = new RelayCommand(async () => await NavigateToPageAsync(ApplicationPage.FacturePage));
        }

        private async Task NavigateToPageAsync(ApplicationPage page)
        {
            if (CurrentPage == page)
                return;

            CurrentPage = page;
            await Task.Delay(1);
        }




        //#region MyRegion
        //public MenuViewModel()
        //{
        //    HomePage = new RelayCommand(async () => await HomePageAsync());

        //    StockPage = new RelayCommand(async () => await StockPageAsync());
        //    InvoicePage = new RelayCommand(async () => await InvoicePageAsync());
        //    MouvementPage = new RelayCommand(async () => await ToMouvementPagePageAsync());
        //    EntreeStockPage = new RelayCommand(async () => await ToEntreeStockPageAsync());
        //    FacturePage = new RelayCommand(async () => await ToFacturePageAsync());
        //}

        //private async Task ToFacturePageAsync()
        //{
        //    if (CurrentPage == ApplicationPage.FacturePage)
        //        return;

        //    CurrentPage = ApplicationPage.FacturePage;
        //    IoC.Application.GoToPage(CurrentPage);

        //    await Task.Delay(1);

        //}
        //private async Task ToEntreeStockPageAsync()
        //{
        //    if (CurrentPage == ApplicationPage.MainEntreeStockPage)
        //        return;
        //    CurrentPage = ApplicationPage.MainEntreeStockPage;
        //    IoC.Application.GoToPage(CurrentPage);
        //    await Task.Delay(1);
        //}

        //private async Task ToMouvementPagePageAsync()
        //{
        //    if (CurrentPage == ApplicationPage.MouvementPage)
        //        return;
        //    CurrentPage = ApplicationPage.MouvementPage;
        //    IoC.Application.GoToPage(CurrentPage);
        //    await Task.Delay(1);
        //}

        //private async Task InvoicePageAsync()
        //{
        //    if (CurrentPage == ApplicationPage.MainSorteStockPage)
        //        return;
        //    CurrentPage = ApplicationPage.MainSorteStockPage;
        //    IoC.Application.GoToPage(CurrentPage);
        //    await Task.Delay(1);
        //}

        //private async Task StockPageAsync()
        //{
        //    if (CurrentPage == ApplicationPage.StockHostPage)
        //        return;
        //    CurrentPage = ApplicationPage.StockHostPage;
        //    IoC.Application.GoToPage(CurrentPage);
        //    await Task.Delay(1);
        //}

        //public async Task HomePageAsync()
        //{
        //    if (CurrentPage != ApplicationPage.Home)
        //        return;
        //    CurrentPage = ApplicationPage.Home;
        //    IoC.Application.GoToPage(CurrentPage);

        //    await Task.Delay(1);
        //} 
        //#endregion

    }
}
