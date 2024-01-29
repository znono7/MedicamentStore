using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{ 
    public class InvoiceDetail
    {
        public int Id { get; set; } 
        public int IdS { get; set; }
        public string? InvoiceNumber { get; set; }
        public int ProductId { get; set; }
        public int Quantite { get; set; }
        public int QuantiteRest { get; set; }
        public double PrixTotal { get; set; }
        
    }
}
