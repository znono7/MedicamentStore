using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public interface IInvoiceRepository
    {
        Task<int> GetLastInvoiceNumber();  
        Task<DbResponse<Invoice>> InsertInvoice(Invoice invoice);
        Task<IEnumerable<Invoice>> GetAllInvoices();
        Task<IEnumerable<Invoice>> GetAllInvoices(int pageNumber, int pageSize);
        Task<DbResponse> InsertInvoice(Invoice invoice , ObservableCollection<InvoiceItem> invoiceDetails );
        Task<DbResponse<InvoiceDetail>> InsertInvoiceDetail(ObservableCollection<InvoiceProduct> invoiceDetails);
        Task<IEnumerable<InvoiceProduct>> GetAllInvoiceProduct();
        Task<IEnumerable<InvoiceProduct>> GetAllInvoiceProduct(int type); 
        Task<IEnumerable<InvoiceProduct>> GetSearchInvoiceProduct(string word);
    }
}
