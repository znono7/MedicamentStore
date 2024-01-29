using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public interface IMedicamentRepository
    {
        Task<IEnumerable<MedicamentFact>> GetSearch(string Nom);
        Task<IEnumerable<MedicinCommercial>> GetNomSearch(string Nom);
        Task<IEnumerable<MedicamentStock>> GetSearchForStock(string Nom);
        Task<IEnumerable<MedicinDCI>> GetDCI(string Nom);
    }
}
