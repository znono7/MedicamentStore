﻿using System;
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
        Task<int> GetTotalInvoices(int type = 0);
        Task<DbResponse<Invoice>> InsertInvoice(Invoice invoice); 
        Task<IEnumerable<Invoice>> GetAllInvoices(int pageNumber, int pageSize, int type);
        Task<IEnumerable<Invoice>> GetAllInvoicesBySupplie(int pageNumber, int pageSize, int type,int idSupplie);
        Task<IEnumerable<Invoice>> GetAllInvoicesByDate(int pageNumber, int pageSize, DateTime startDate , DateTime endDate, int type , int idSupplie = 0);

        Task<IEnumerable<InvoiceItemDto>> GetInvoiceItems (string num); 
        Task<IEnumerable<Invoice>> GetAllInvoices(int pageNumber, int pageSize);
        Task<DbResponse> InsertInvoice(Invoice invoice , ObservableCollection<InvoiceItem> invoiceDetails );
        Task<DbResponse<InvoiceDetail>> InsertInvoiceDetail(ObservableCollection<InvoiceProduct> invoiceDetails);
        Task<IEnumerable<InvoiceProduct>> GetAllInvoiceProduct();
        Task<IEnumerable<InvoiceProduct>> GetAllInvoiceProduct(int type); 
        Task<IEnumerable<InvoiceProduct>> GetSearchInvoiceProduct(string word);
        Task<int> GetTotalInvoicesBySupplie(int type, int id);
        Task<int> GetTotalInvoicesByDate(DateTime startDate, DateTime endDate, int type, int idSupp);
    }
}
