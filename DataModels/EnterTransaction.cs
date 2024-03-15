using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public class EnterTransaction
    {
        public string? Nom_Commercial { get; set; }
        public string? Forme { get; set; }
        public string? Dosage { get; set; }
        public string? Conditionnement { get; set; }
        public string? Img { get; set; }
        public int Type { get; set; }

        public int QuantiteTransaction {  get; set; }   
        public string? PreviousQuantity {  get; set; }
        public DateTime Date { get; set; }
        public string? DateTrans { get; set; }
        public string? Unite { get; set; }   
        public string? Nom { get; set; }
        public string? TypeMed { get; set; }
        public int TypeTransaction { get; set; }
        public int Quantite { get; set; }
        public int IdStock { get; set; }
        public double Prix { get; set; }
        public int IdUnite { get; set; }
        public int IdMedicament { get; set; }

    }
}
