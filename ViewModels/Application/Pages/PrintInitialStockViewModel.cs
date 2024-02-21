using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MedicamentStore
{
    public class PrintInitialStockViewModel : BaseViewModel
    {
        public ICommand ReturnCommand { get; set; }
        public ObservableCollection<MedicamentStock> Stocks { get; }

        public  PrintInitialStockViewModel(ObservableCollection<MedicamentStock> stocks)
        {
            ReturnCommand = new RelayCommand(async () => await ToBackPage());
            Stocks = stocks;
        }

        private async Task ToBackPage()
        {
            IoC.Application.GoToPage(ApplicationPage.StockHostPage);
            await Task.Delay(1);
        }
    }
}
