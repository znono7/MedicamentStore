using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public class InitialStockDocViewModel : BaseViewModel
    {
        public ObservableCollection<MedicamentStock> Stocks { get; set; }

        public bool HeaderVisible { get; set; } 
        public bool FooterVisible { get; set; }
        public string NumPage { get; set; }
        public string TypeString { get; set; }
        public string DateTod { get; set; } 
        public double TotalAmount { get; set; }
        public InitialStockDocViewModel(ObservableCollection<MedicamentStock> stocks, bool headerVisible, bool footerVisible, string numPage)
        {
            Stocks = stocks;
            HeaderVisible = headerVisible;
            FooterVisible = footerVisible;
            NumPage = numPage;
            SetDate();
        }
        private void SetDate()
        {
           if(Stocks.Any())
            {
               // TypeString = $"{((ProduitsPharmaceutiquesType)Stocks.FirstOrDefault().Type).ToProduitsPharmaceutiques()}";
                TotalAmount = Stocks.Sum(c => c.PrixTotal);
            }
            DateTod = DateTime.Today.ToString("dd/MM/yyyy");
        }
    }
}
