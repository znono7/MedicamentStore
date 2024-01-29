using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public interface IProduitRepository
    {
        Task<DbResponse<ProduitPharma>> InsertProduit(ProduitPharma produit);

        Task<IEnumerable<TypeProduct>> GetAllProduitTypes();
    }
}
