using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public interface ISuppliesRepository
    {
       Task<IEnumerable<Supplies>> GetAllSupplies();
        Task<DbResponse<Supplies>> InsertSupplieAsync(Supplies supplies);

        Task<IEnumerable<Supplies>> FindByName(string name);


    }
}
