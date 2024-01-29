using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public class SelectSuppPharmaEventArgs : EventArgs
    {
        public Supplies? SelectedSupplies { get; set; }

    }
}
