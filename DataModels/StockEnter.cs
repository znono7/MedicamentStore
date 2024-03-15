using System;

namespace MedicamentStore
{
    public class StockEnter
    {
        public int Id { get; set; }
        public int IdStock {  get; set; }
        public int QuantiteAdded { get; set;}
        public int Quantite { get; set; }
        public int IdSupplie { get; set; }
        public DateTime Date { get; set; }
    }

    public class StockEnterDto : StockEnter
    {
        private string? source;

        public string? Img
        {
            get => source;
            set
            {
                if (value == "0")
                {
                    source = $"pack://application:,,,/Pictures/Lp.jpg";
                    return;
                }
                source = $"pack://application:,,,/Pictures/{value}";

            }
        }
        public string? Nom_Commercial { get; set; }
        public string? Forme { get; set; }
        public string? Dosage { get; set; }
        public string? Conditionnement { get; set; }
        public string? Unite { get; set; }
        public string? PrimaryBackground { get; set; }
       // public int Quantite { get; set; }

        public double Prix { get; set; }

        public double PrixTotal { get; set; }
       // public int IdSupplie { get; set; }
        public string? Nom { get; set; } 
       // public DateTime Date { get; set; }
        //public string SymbleType { get; set; }
        //public int Type { get; set; }
    }
    public class EnterMedicaments 
    { 
        public int IdMedicament { get; set; }
        public string? Nom_Commercial { get; set; }
        public string? Forme { get; set; }
        public string? Dosage { get; set; }
        public string? Conditionnement { get; set; }
        public string? TypeMed { get; set; }
        public int Type { get; set; }
        public string? Img { get; set; }
       
    }
}
