using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public class Transaction
    {
        public int Id {  get; set; }
        public int IdStock {  get; set; }
        public int TypeTransaction { get; set; }
    }

    public class TransactionDto : Transaction
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
        public int Quantite { get; set; }

        public double Prix { get; set; }

        public double PrixTotal { get; set; }
        public int IdSupplie { get; set; }
        public string? Nom { get; set; }
        public string SymbleType { get; set; }
    }
}
