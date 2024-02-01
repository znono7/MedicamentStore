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
    }
}
