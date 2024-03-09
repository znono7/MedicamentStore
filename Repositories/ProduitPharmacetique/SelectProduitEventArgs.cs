using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public class SelectProduitPharmaEventArgs : EventArgs
    {
        public PharmaceuticalProduct? SelectedProductPharma { get; set; }

    }
}
