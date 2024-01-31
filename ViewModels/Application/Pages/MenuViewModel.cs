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
        public ICommand? MedicamentPage { get; set; }
        public ICommand? UsersPage { get; set; }
        public ICommand? ParemetrePage { get; set; }
        public ICommand? NewInvoicePage { get; set; }
        public ICommand? StockPage { get; set; }
        public ICommand? InvoicePage { get; set; }
        public ICommand? MouvementPage { get; set; }//

        public MenuViewModel()
        {
            HomePage = new RelayCommand(async () => await HomePageAsync());
            UsersPage = new RelayCommand(async () => await UsersPageAsync());
            ParemetrePage = new RelayCommand(async () => await ParemPageAsync());
            MedicamentPage = new RelayCommand(async () => await MedPageAsync());
            NewInvoicePage = new RelayCommand(async () => await NewInvoicePageAsync());
            StockPage = new RelayCommand(async () => await StockPageAsync());
            InvoicePage = new RelayCommand(async () => await InvoicePageAsync());
            MouvementPage = new RelayCommand(async () => await ToMouvementPagePageAsync());
        }

        private async Task ToMouvementPagePageAsync()
        {
            IoC.Application.GoToPage(ApplicationPage.MouvementPage);
            await Task.Delay(1);
        }

        private async Task InvoicePageAsync()
        {
            IoC.Application.GoToPage(ApplicationPage.InvoiceHostPage);
            await Task.Delay(1);
        }

        private async Task StockPageAsync()
        {
            IoC.Application.GoToPage(ApplicationPage.StockHostPage);
            await Task.Delay(1);
        }

        private async Task NewInvoicePageAsync()
        {
            IoC.Application.GoToPage(ApplicationPage.NewInvoice);
            await Task.Delay(1);
        }

        private async Task MedPageAsync()
        {
            IoC.Application.GoToPage(ApplicationPage.Medicament);
            await Task.Delay(1);
        }

        private async Task ParemPageAsync()
        {
            IoC.Application.GoToPage(ApplicationPage.Paremetres);
            await Task.Delay(1);
        }

        public async Task HomePageAsync()
        {
            // Go to register page?
            IoC.Application.GoToPage(ApplicationPage.Home);

            await Task.Delay(1);
        }
        public async Task UsersPageAsync()
        {
            // Go to register page?
            IoC.Application.GoToPage(ApplicationPage.Users);

            await Task.Delay(1);
        }
    }
}
