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
    }
}
