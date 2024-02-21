using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public class InvoiceProduct
    {

       
        public int Id { get; set; }
        public int IdS { get; set; }
         
        public string? InvoiceNumber { get; set; }  
        public int ProductId { get; set; } 

        protected int _quanttie { get; set; } 
        public int Quantite
        {
            get => _quanttie;
            set  
                
            {
              
              
                _quanttie = value;
                
               
            }
        }

        public int QRestant {  get; set; }
        private int _quantiteRest;
        public int QuantiteRest
        {
            get => _quantiteRest;
            set
            {
                _quantiteRest = value;
                QRestant = _quantiteRest;
               
            }
        }
        private double _prixTotal;
        public double PrixTotal
        {
            get => _prixTotal;
            set
            {
                _prixTotal = value;
               
            }
        }
        public string? TypeProduct { get; set; }
        public string? Nom_Commercial { get; set; }
        public string? Forme { get; set; }
        public string? Dosage { get; set; }
        public string? Conditionnement { get; set; }
        public string? Unite { get; set; }
        public string? Type { get; set; }

        private string? source;

        public string? Nom { get; set; }


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

        
        public double Prix { get; set; }

       

    }
}
