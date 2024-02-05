using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MedicamentStore
{
    public class PrintMovmentStockViewModel : BaseViewModel
    {
        public ICommand ReturnCommand { get; set; }
        public ObservableCollection<TransactionDto> Stocks { get; }

        public PrintMovmentStockViewModel(ObservableCollection<TransactionDto> stocks) 
        {
            ReturnCommand = new RelayCommand(async () => await ToBackPage());
            Stocks = stocks;
        }

        private async Task ToBackPage()
        {
            IoC.Application.GoToPage(ApplicationPage.MouvementPage);
            await Task.Delay(1);
        }
    }
}
