using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MedicamentStore
{
    public class UserCredentials
    {
        public int Id { get; set; }
        public string? Nom { get; set; }
        public string? Prenom { get; set; }
        public string? FullName => $"{Nom} {Prenom}";
        public string? UserName { get; set; }
        public string? Type { get; set; }

        public string Initials
        {
            get
            {
                // Logic to calculate initials, e.g., "AB" for "John Doe"
                return $"{Nom?[0]}{Prenom?[0]}".ToUpper();
            }
        }

        public SolidColorBrush BackgroundColor { get; set; } 


        private Random random = new Random();

        private SolidColorBrush GetRandomColor()
        {
            byte[] colorBytes = new byte[3];
            random.NextBytes(colorBytes);
            return new SolidColorBrush(Color.FromRgb(colorBytes[0], colorBytes[1], colorBytes[2]));
        }

        public UserCredentials()
        {
            BackgroundColor = GetRandomColor();
        }
    }
}
