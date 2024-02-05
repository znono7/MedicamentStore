using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
   public class MedicamentStock 
    { 
        private string? source;   
        public int Type { get; set; }    
         
        public int Id { get; set; }
        public int Ids { get; set; }
        //  public string Link { get; set; }

          
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


        public string? DesignName => $"{Nom_Commercial} \n {Forme} {Dosage} {Conditionnement}";
        public string? Nom_Commercial { get; set; }
        public string? Forme { get; set; }
        public string? Dosage { get; set; }
        public string? Conditionnement { get; set; }
        public string? Unite { get; set; }

        public string? Tarif_de_Référence { get; set; }
        public string? PPA_indicatif { get; set; }
       
       // private string? primaryColor { get; set; }
        public string? PrimaryBackground { get /*=> primaryColor*/;
            set;
            //{
            //    if (Quantite == 0)
            //    {
                   
            //        primaryColor = "FF423C";

            //    }
            //    else
            //     if (Quantite > 0 && Quantite < 5)
            //    {
                   
            //        primaryColor = "F09E43";
            //    }
            //    else
            //    {
                    
            //        primaryColor = "349432";
            //    }
            //}
        }

        #region states
       
      
        public string? Status
        { get ;
            set
           ;
        }



        public int Quantite { get; set; }

        public int IdSupplie { get; set; }
        public string? Nom { get; set; }

        public double Prix { get; set; }

       
        public double PrixTotal 
        {
            get ;
            set
           ;
        }
        public string? Date { get; set; }


        #endregion


    }
}
