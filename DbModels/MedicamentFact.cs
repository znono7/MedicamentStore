using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
   public class MedicamentFact
    {
        public int Id { get; set; }
        //  public string Link { get; set; }

        private string? source {  get ; set; } 
        public string? Img { get => source; set 
            {
                source = $"../Pictures/{value}";

            } }
        //public string Labo { get; set; }
        //public string Pharmacologique { get; set; }
        //public string Thérapeutique { get; set; }
        //public string DCI { get; set; }
        //public string Commercialisation { get; set; }
        //public string Remboursable { get; set; }
        //public string Code_DCI { get; set; }
        public string? Nom_Commercial { get; set; }
        public string? Forme { get; set; }
        public string? Dosage { get; set; }
        public string? Conditionnement { get; set; }
        //public string Type { get; set; }
        //public string Liste { get; set; }
        public string? Tarif_de_Référence { get; set; }
        public string? PPA_indicatif { get; set; }
        //public string Enregistrement { get; set; }
        //public string Pays { get; set; }
    }
}
