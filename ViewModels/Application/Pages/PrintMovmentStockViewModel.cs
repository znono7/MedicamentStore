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
        public ObservableCollection<MouvementStocks> Stocks { get; set; }

        public PrintMovmentStockViewModel(ObservableCollection<MouvementStocks> stocks) 
        {
            ReturnCommand = new RelayCommand(async () => await ToBackPage());
            Stocks = stocks;
        }
         
        private async Task ToBackPage() 
        {
            
           // IoC.Application.GoToPage(ApplicationPage.MouvementPage,new MouvementViewModel(Stocks.FirstOrDefault().IdMedicament));
            IoC.Application.GoToPage(ApplicationPage.MainMovmentStockPage);
            await Task.Delay(1);
        }
    }
}
