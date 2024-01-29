using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public class PrintStockListViewModel : BaseDocumentViewModel
    {
        public ObservableCollection<MedicamentStock> Stocks {  get; set; }
        
       
        public string TypeString { get; set; }
        public ProduitsPharmaceutiquesType Type { get; set; }

        public double TotalAmount { get; set; }

        private async Task LoadStocksAsync(ProduitsPharmaceutiquesType t)
        {


            var Result = await IoC.StockManager.GetMedicamentStocksAsync(t);
            foreach (var Stock in Result)
            {
                UpdateStatus(Stock);
            }
            TotalAmount = Result.Sum(d => d.PrixTotal);

            Stocks = new ObservableCollection<MedicamentStock>(Result.Where(x => x.Quantite > 0));

        }
        private void UpdateStatus(MedicamentStock stock)
        {

            //double x = stock.Prix * stock.Quantite;
            //var s = string.Format("{0:0.00}", x);
            stock.PrixTotal = double.Parse(string.Format("{0:0.00}", stock.Prix * stock.Quantite));

        }

        public PrintStockListViewModel(ProduitsPharmaceutiquesType type)
        {
            Type = type; 
            _ = LoadStocksAsync(Type);
        }
    }
}
