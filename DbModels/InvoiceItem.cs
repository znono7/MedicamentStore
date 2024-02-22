using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public class InvoiceItem
    {
        public int Id { get; set; }
        public int IdInvoice { get; set; }
        public int IdMedicament { get; set; } 
        public int IdStock { get; set; }
        public int IdTypeProduct { get; set; }
        public int IdUnite { get; set; }
        public string? InvoiceNumber { get; set; }
        public int Quantite { get; set; }
        public double Prix { get; set; }

    }
    public class InvoiceItemDto : InvoiceItem
    {
        public string? Nom_Commercial { get; set; }
        public string? Forme { get; set; }
        public string? Dosage { get; set; }
        public string? Conditionnement { get; set; }
        public string? Unite { get; set; }
        public string? TypeProduct { get; set; }
        public double PrixTotal { get; set; }

    }

}
