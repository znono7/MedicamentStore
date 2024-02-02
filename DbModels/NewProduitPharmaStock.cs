

using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MedicamentStore
{
   public class NewProduitPharmaStock : BaseViewModel
    {
        public int Id { get; set; }
        public string? Nom_Commercial { get; set; } 
        public string? Forme { get; set; } 
        public string? Dosage { get; set; } 
        public string? Conditionnement { get; set; }
        public int Type { get; set; } = 0;

        public double Prix { get; set; } = 0;  
        public int Quantite { get; set; } = 0; 
        public int Unit { get; set; } 
        public int IdSupplie { get; set; } = 0;
        public string? Date { get; set; }

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
        //private Unite _selectedUnite {  get; set; }

        //public Unite SelectedUnite
        //{
        //    get { return _selectedUnite; }
        //    set
        //    {
        //        OnPropertyChanged(nameof(SelectedUnite));

        //        // Your additional logic when the selection changes
        //        if (_selectedUnite == null)
        //        {
        //            Unit = 1;
        //        }
        //        else
        //        {
        //            Unit = _selectedUnite.Id;
        //        }

        //    }
        //}
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
