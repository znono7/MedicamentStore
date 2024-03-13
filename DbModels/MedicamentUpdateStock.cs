using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public class MedicamentUpdateStock : BaseViewModel
    {
        public int IdMedicament {  get; set; }
        public string IdProduct { get; set; }
        public int IdSupplie { get; set; }
        public int IdStock { get; set; } 
        public int Type { get; set; }
        public int IdUnite { get; set; }
        public string? Nom_Commercial { get; set; }
        public string? Forme { get; set; }
        public string? Dosage { get; set; }
        public string? Conditionnement { get; set; }
        public string? Unite { get; set; }
        public int Quantite { get; set; }
        public double Prix { get; set; }
        public double PrixTotal { get; set; }
        public string? Date { get; set; }
        private int _quantiteAdded { get; set; }
        public int QuantiteAdded
        {
            get => _quantiteAdded;
            set
            {
                _quantiteAdded = value;
                PrixTotal = _quantiteAdded * Prix;
                OnPropertyChanged(nameof(QuantiteAdded));
                OnPropertyChanged(nameof(PrixTotal));
            }
        }
    }
}
