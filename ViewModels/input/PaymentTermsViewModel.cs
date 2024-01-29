using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
   public class PaymentTermsViewModel : BaseViewModel
    {
        /// <summary>
        /// The label to identify what this value is for
        /// </summary>
        public string? Label { get; set; } = "Conditions de Paiement";

        public List<string> PaymentTerms { get; set; }

        public string SelectedPaymentTerm { get; set; }

        public PaymentTermsViewModel()
        {
            PaymentTerms = new List<string>
        {
                "---",
            "Net X Jours",
            "Dû à Réception",
            "Paiement à l'Avance",
            "Paiement à la Livraison",
                "2/10 Net 30",
                "Fin de Mois",
                "Paiement Partiel",
                "Conditions Personnalisées"
            
        };
        }
    }
}
