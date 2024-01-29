

namespace MedicamentStore
{
  public  class StockProducts
    {
        private string? _s;

        public int Type { get; set; }

        public int Id { get; set; }
        public string? Nom_Commercial { get; set; }
        public string? Forme { get; set; }
        public string? Dosage { get; set; }

        public string? StatType { get => _s; set 
            {
                if (Type == 1)
                    _s = "Médicament";
                if (Type == 2) _s = "Vaccin";
                if (Type == 3) _s = "Autre";
            } }
        public string? Conditionnement { get; set; }

        public string? Tarif_de_Référence { get; set; }
        public string? PPA_indicatif { get; set; }
        public double Prix { get; set; }
        public int Quantite { get; set; } = 0;
    }
}
