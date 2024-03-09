using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
   public class SelectedItemEventArgs : EventArgs 
    {
        public MedicamentStock? SelectedItem { get; set; } 
    }
}
