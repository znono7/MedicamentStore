using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public class PrintStockFixedViewModel : BaseDocumentViewModel
    {
        public ObservableCollection<MedicamentStock> Stocks {  get; set; }
        
        public bool HeaderVisible {  get; set; }
        public bool FooterVisible {  get; set; }
        public string NumPage { get; set; }
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

        public PrintStockFixedViewModel(ObservableCollection<MedicamentStock> _stocks,double t,string ty,bool _headerVisible=true,bool _footerVisible =true,string numPage="1")
        {
            Stocks = _stocks;
            TotalAmount = t;
            TypeString = ty;
            HeaderVisible = _headerVisible;
            FooterVisible = _footerVisible;
            NumPage = numPage;
           // _ = LoadStocksAsync(Type);
        }
    }
}
