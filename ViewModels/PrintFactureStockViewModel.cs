using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MedicamentStore
{
    public class PrintFactureStockViewModel : BaseViewModel
    {
        public ICommand ReturnCommand { get; set; }
        public Invoice Invoice { get; }
        public List<InvoiceItemDto> Stocks { get; set; }

        public PrintFactureStockViewModel(Invoice invoice,List<InvoiceItemDto> invoices)
        { 
            ReturnCommand = new RelayCommand(async () => await ToBackPage());
            Invoice = invoice;
            Stocks = invoices;
            
            
        }

        private async Task ToBackPage()
        {
            IoC.Application.GoToPage(ApplicationPage.FacturePage);
            await Task.Delay(1);
        }

       
    }
}
