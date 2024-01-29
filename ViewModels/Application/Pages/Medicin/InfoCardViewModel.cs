using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public enum InfoCardType
    {
        Stock = 1,
        Prix = 2,
        Exportation = 3
    }
   public class InfoCardViewModel : BaseViewModel
    {
        /// <summary>
        /// The background color of the Box in ARGB value
        /// </summary>
        public string? PrimaryBackground { get; set; }
        public string? PrimaryBackground2 { get; set; }
        public string? EllipseBackground1 { get; set; }
        public string? EllipseBackground2 { get; set; }
       

        /// <summary>
        /// The Line color of the Box in ARGB value
        /// </summary>
        public string? Title { get; set; }
        public string? Number { get; set; }

        public InfoCardType InfoCardType { get; set; }

        public IconType IconType { get; set; }

       
    }
}
