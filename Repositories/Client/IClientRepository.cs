using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
   public interface IClientRepository
    {
       
        Task<IEnumerable<Client>> GetAllAsync();

        Task<DbResponse<Client>> InsertClientAsync(Client client);

        Task<IEnumerable<Client>> FindByName(string name);

    }
}
