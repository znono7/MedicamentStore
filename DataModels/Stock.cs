using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore 
{ 
   public class Stock
    {
        public int Id { get; set; }
        public int IdMedicament { get; set; } 
        public int IdSupplie { get; set; } 
        public int Quantite { get; set; }

        public double Prix { get; set; }
        public int Type { get; set; }



    }
}
