

namespace MedicamentStore
{
   public class ProduitPharma
    {
        private string? source;

        public int Id { get; set; }
        public string? Nom_Commercial { get; set; }
        public string? Forme { get; set; }
        public string? Dosage { get; set; } 
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
        public string? Conditionnement { get; set; }

        public int Type { get; set; } = 0;


    }
}
