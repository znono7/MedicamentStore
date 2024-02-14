using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MedicamentStore
{
    public class PrintSorteStockViewModel : BaseViewModel
    {
        public ICommand ReturnCommand { get; set; }
        public ObservableCollection<TransactionDto> Stocks { get; }

        public PrintSorteStockViewModel(ObservableCollection<TransactionDto> stocks)
        {
            ReturnCommand = new RelayCommand(async () => await ToBackPage());
            Stocks = stocks;
        }

        private async Task ToBackPage()
        {
            IoC.Application.GoToPage(ApplicationPage.InvoiceHostPage);
            await Task.Delay(1);
        }
    }
}
