using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
   public interface IStockRepository
    { 
      
        Task<IEnumerable<MedicamentStock>> GetPagedStocksAsync(int pageNumber, int pageSize, ProduitsPharmaceutiquesType type);
        Task<DbResponse> DeleteStockAsync(int id); 
        IEnumerable<Unite> GetUnitsAsync();
        Task<double> GetAmountTotalStockAsync(ProduitsPharmaceutiquesType type);
        Task<int> GetProduitTotalStockAsync(ProduitsPharmaceutiquesType type);
        Task<DbResponse> UpdateStock(List<MedicamentUpdateStock> newProduits, Invoice invoice, List<InvoiceItem> invoiceItems);
        Task<DbResponse> NewStock(List<NewProduitPharmaStock> newProduits, Invoice invoice, List<InvoiceItem> invoiceItems);

        Task<DbResponse<int>> GetLastProductNumber();

    }
}
