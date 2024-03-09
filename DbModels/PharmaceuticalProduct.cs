using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MedicamentStore
{
    public class PharmaceuticalProduct
    {
        public int Id { get; set; }
        public Guid? IdProduct { get; set; }
        public string? Nom_Commercial { get; set; }
        public string? Forme { get; set; }
        public string? Dosage { get; set; }
        public string? Conditionnement { get; set; }
        public string? Img { get; set; }
        public ImageSource? imageSource { get; set; }
        public int Type { get; set; }
    }
}
