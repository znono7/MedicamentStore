using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MedicamentStore
{
    public class FacturesViewModel : BaseViewModel
    {
        #region Commands
        public ICommand SavePdfCommand { get; set; }
        public ICommand PrintCommand { get; set; }
        #endregion

        #region Observable collections
        public ObservableCollection<Invoice> MInvoices { get; set; }
        public ObservableCollection<Invoice> Invoices
        
        {
            get => MInvoices;
            set
            {
                MInvoices = value;
                FilterInvoices = new ObservableCollection<Invoice>(MInvoices);
            }
        }

        public ObservableCollection<Invoice> FilterInvoices { get; set; }
        #endregion

        #region Public Properties
        public bool IsLoading { get; set; }
        #endregion


        #region Constructor
        public FacturesViewModel()
        {
            _ = GetInvoices();
        }
        #endregion

        #region Methodes
        public async Task GetInvoices()
        {
            IsLoading = true;
            var Result = await IoC.InvoiceManager.GetAllInvoices();
            foreach (var item in Result)
            {
                if (item.InvoiceType == 1)
                {
                    item.FactType = "Entrée";
                }
                else
                {
                    item.FactType = "Sortie";
                    item.NomSupplie = "/";
                }
            }
            Invoices = new ObservableCollection<Invoice>(Result);
            IsLoading = false;
        } 
        #endregion
    }
}
