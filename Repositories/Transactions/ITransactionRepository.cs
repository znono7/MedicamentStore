using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<MouvementStocks>> GetAllMovement(string IdProduct, int pageNumber, int pageSize);  
        Task<IEnumerable<EnterTransaction>> GetAllEnter(string IdProduct, int pageNumber, int pageSize);
        Task<IEnumerable<EnterTransaction>> GetAllSorte(string IdProduct, int pageNumber, int pageSize);
        Task<int> GetTotalEnterStockAsync(string IdProduct); 
        Task<int> GetTotalMovmentStockAsync(string IdProduct);
        Task<int> GetTotalSortieStockAsync(string IdProduct);

    }
}
