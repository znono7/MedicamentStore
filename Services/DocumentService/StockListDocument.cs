using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public class StockListDocument : IStockListDocument
    {
        public Task ShowDocument(PrintStockListViewModel model)
        {
            return new ProduitStockDoc().ShowDocument(model);
        }
    }
}
