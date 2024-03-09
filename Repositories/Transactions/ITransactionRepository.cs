using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<TransactionDto>> GetAll();
        Task<IEnumerable<MouvementStocks>> GetAllMovement(int IdMedicament, int pageNumber, int pageSize);  
        Task<IEnumerable<EnterTransaction>> GetAllEnter(int IdMedicament, int pageNumber, int pageSize);
        Task<IEnumerable<EnterTransaction>> GetAllSorte(int IdMedicament, int pageNumber, int pageSize);
        Task<IEnumerable<EnterTransaction>> GetAllEnter();
        Task<int> GetTotalEnterStockAsync(int IdMedicament);
        Task<int> GetTotalMovmentStockAsync(int IdMedicament);
        Task<int> GetTotalSortieStockAsync(int IdMedicament);

    }
}
