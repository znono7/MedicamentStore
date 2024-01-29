using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore 
{
    public class Invoice
    {
        public int Id { get; set; }
        public string? Date { get; set; }
        public string? Number { get; set; }
        public double MontantTotal { get; set; }
        public int ProduitTotal { get; set; }
    }
}
