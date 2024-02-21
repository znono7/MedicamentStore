using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public class MouvementStocks
    {
        public int Id { get; set; }
        public int IdMedicament { get; set; }
        public int IdStock { get; set; }
        public int TypeTransaction { get; set; }
        public int QuantiteTransaction { get; set; }
        public string PreviousQuantity { get; set; }
        public DateTime Date { get; set; }
        public string? Nom_Commercial { get; set; } 
        public string? Forme { get; set; }
        public string? Dosage { get; set; }
        public string? Conditionnement { get; set; }
        public string? TypeMed { get; set; }
        public string? Unite { get; set; }
        public string? Supplie { get; set; }
        public int Type { get; set; }
        public string StockIn { get; set; }
        public string StockOut { get; set; }
        public double Prix { get; set; }


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
    }
}
