using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public class TextEntryInvoiceNumViewModel : BaseViewModel
    {

        /// <summary>
        /// The label to identify what this value is for
        /// </summary>
        public string Label { get; set; } = "Facture #";

        /// <summary>
        /// The current Entered text
        /// </summary>
        public string? EnteredText { get; set; }

        public double height { get; set; } = 32;
    }
}
