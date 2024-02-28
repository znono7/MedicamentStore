using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public class FacturePageViewModel : BaseViewModel
    {
        public ObservableCollection<InvoiceItem> InvoiceItems { get; set; }
        public Invoice Invoice { get; set; }
        public bool HeaderVisible { get; set; }
        public bool FooterVisible { get; set; }
        public string NumPage { get; set; }
        public string TypeString { get; set; }
        public string DateTod { get; set; }
        public double TotalAmount { get; set; }
        public FacturePageViewModel(Invoice invoice, bool headerVisible, bool footerVisible, string numPage)
        {
            
            Invoice = invoice;
            HeaderVisible = headerVisible;
            FooterVisible = footerVisible;
            NumPage = numPage;
            SetDate();
        }
        private void SetDate()
        {
            if (InvoiceItems.Any())
            {
                // TypeString = $"{((ProduitsPharmaceutiquesType)Stocks.FirstOrDefault().Type).ToProduitsPharmaceutiques()}";
                TotalAmount = Invoice.MontantTotal;
            }
            DateTod = DateTime.Today.ToString("dd/MM/yyyy");
        }

        public async Task GetInvoiceItems(string num)
        {

        }
    }
}
