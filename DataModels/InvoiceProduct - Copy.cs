using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public class InvoiceProductDataGrid : BaseViewModel
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
                if (value == 0)
                {
                    QRestant = QuantiteRest;
                    _quanttie = 0;
                    PrixTotal = 0;
                    return;

                } 
                if(value > QuantiteRest)
                {
                    IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Error, "La Quantité ne peut pas être Supérieure à la Quantité Restante."));
                    return;
                }
                
                _quanttie = value;
                
                QRestant = QuantiteRest;
                QRestant = QRestant - _quanttie;
                PrixTotal = _quanttie * Prix;
                OnPropertyChanged(nameof(Quantite));
                OnPropertyChanged(nameof(QRestant));
                OnPropertyChanged(nameof(PrixTotal));
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
                OnPropertyChanged(nameof(QuantiteRest));
            }
        }
        private double _prixTotal;
        public double PrixTotal
        {
            get => _prixTotal;
            set
            {
                _prixTotal = value;
                OnPropertyChanged(nameof(PrixTotal));
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
        public int TypeId { get; set; }

        public int IdUnite { get; set; }

        public double Prix { get; set; }


    }
}
