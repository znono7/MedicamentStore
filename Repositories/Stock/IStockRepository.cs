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
        Task<DbResponse> DeleteStockAsync(Stock stock); 
        Task<DbResponse> UpdateStockAsync(MedicamentStock stock);
        Task<DbResponse<NewProduitPharmaStock>> AddNewStockAsync(ObservableCollection<NewProduitPharmaStock> newProducts);
        Task<IEnumerable<ProduitPharma>> GetProduitStocksAsync(string Word, int id);
        Task<IEnumerable<MedicamentStock>> GetMedicamentStocksAsync(ProduitsPharmaceutiquesType type);
        IEnumerable<Unite> GetUnitsAsync();
        Task<double> GetAmountTotalStockAsync(ProduitsPharmaceutiquesType type);
        Task<int> GetProduitTotalStockAsync(ProduitsPharmaceutiquesType type);
        Task<IEnumerable<ProductIds>> GetProductIdsAsync();

        #region Stock Enter
        Task<DbResponse<StockEnter>> AddStockEnterAsync(StockEnter stockEnter);

        #endregion
    }
}
