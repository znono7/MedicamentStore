

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MedicamentStore
{
   public class NewProduitPharmaStock : BaseViewModel
    { 
        public int Id { get; set; } 
        public int IdStock { get; set; }
        public string? IdProduct { get; set; }
        public string? Img { get; set; }
        public ImageSource? imageSource { get; set; }
        public string? Nom_Commercial { get; set; }   
        public string? Forme { get; set; } 
        public string? Dosage { get; set; } 
        public string? UniteStock { get; set; } 
        public string? Conditionnement { get; set; }
        public int Type { get; set; } = 0; 

        private double _prix {  get; set; }
        public double Prix {get => _prix; 
            set 
            {
                _prix = value;
                PrixTotal = Quantite * _prix;
                OnPropertyChanged(nameof(Quantite));
                OnPropertyChanged(nameof(PrixTotal));
            }
        }
      
        public double PrixTotal { get; set; }

        public int QEnStock { get; set; }
        protected int _quanttie { get; set; }
        public int Quantite
        {
            get => _quanttie;
            set

            {
                if (value == 0)
                {
                   
                    _quanttie = 0;
                    PrixTotal = 0;
                    return;

                }
               
                _quanttie = value;

               
                PrixTotal = _quanttie * Prix;
                OnPropertyChanged(nameof(Quantite));
                OnPropertyChanged(nameof(PrixTotal)); 
            }
        }
        public int Unit { get; set; } 
        public int IdSupplie { get; set; } = 0;
        public string? Date { get; set; }

        public bool IsStock { get; set; }
        private ObservableCollection<Unite> _items;

        public ObservableCollection<Unite> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                
            }
        }
        public Unite SelectedUnite { get; set; }
       
        public NewProduitPharmaStock()
        {
             Initialize();
        }
        private void Initialize()
        {
            SelectedUnite = new Unite();
             GetUnits();
        }

        public void  GetUnits()
        {
            var r =  IoC.StockManager.GetUnitsAsync();
            Items = new ObservableCollection<Unite>(r);
        }
    }
}
