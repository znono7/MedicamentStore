using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public class FactureDocViewModel : BaseViewModel
    {
        public Invoice Invoice { get; }
        public List<InvoiceItemDto> InvoiceItems { get; set; }

        public bool HeaderVisible { get; set; }
        public bool FooterVisible { get; set; }
        public string NumPage { get; set; }
        public string TypeString { get; set; }  
        public string NomFournisseur { get; set; }
        public string DateTod { get; set; }
        public double TotalAmount { get; set; }
        public double TotalProduct { get; set; }
        public FactureDocViewModel(Invoice invoice,List<InvoiceItemDto> invoiceItems , bool headerVisible, bool footerVisible, string numPage)
        {
            Invoice = invoice;
            InvoiceItems = invoiceItems ;
            HeaderVisible = headerVisible;
            FooterVisible = footerVisible;
            NumPage = numPage;
            SetDate();
        }
        private void SetDate()
        {
            TotalAmount = Invoice.MontantTotal;
            TotalProduct = Invoice.ProduitTotal;
           if(Invoice.InvoiceType == 1)
            {
                   TypeString = "Facturation des Entrées de Stockage";
            }
            else
            {
                TypeString = "Facturation des Sortie de Stockage";
            }
            NomFournisseur = Invoice.NomSupplie;

            DateTod = DateTime.Today.ToString("dd/MM/yyyy");
            foreach (var item in InvoiceItems)
            {
                item.Indx = InvoiceItems.IndexOf(item) + 1;
            }
        }
    }
}
