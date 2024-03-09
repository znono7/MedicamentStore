using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input; 

namespace MedicamentStore
{
   public class StockItemsWindowViewModel : BaseViewModel
    {
        #region Protected Members
          
        
         
        /// <summary>
        /// The text to search for in the search command
        /// </summary>
        protected string mSearchText;
        public ObservableCollection<TypeProduct> items { get; set; }

        public bool AttachmentNotifVisible { get; set; }
        #endregion

        #region Public Members

        public event EventHandler<SelectProduitEventArgs> ProduitSelected;

        public NotificationBoxViewModel notificationBoxViewModel { get; set; }



        public ObservableCollection<ProduitPharma> InvoiceItems { get; set; }

        public ObservableCollection<ProductIds> ProductIds { get; set; }
        public bool DimmableOverlayVisible { get; set; }
        /// <summary>
        /// The text to search for when we do a search
        /// </summary>
        public string SearchText
        {
            get => mSearchText;
            set
            {
                // Check value is different
                if (mSearchText == value)
                    return;

                // Update value
                mSearchText = value;

                // If the search text is empty clear items
                if (string.IsNullOrEmpty(SearchText))
                    InvoiceItems = new ObservableCollection<ProduitPharma>();
                    
            }
        }

       
        private TypeProduct _selectedItem { get; set; } 
        public TypeProduct SelectedProduit
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
            }
        }
        #endregion
          
        #region selected Medicament
        public MedicamentStock? SelectedItemStock { get; set; }

        #endregion 

        #region Public Command 
          
        public ICommand SetItemCommand { get; set; }

        /// <summary>
        /// The command for when the user wants to search
        /// </summary>
        public ICommand SearchCommand { get; set; }
        public ICommand NewProduitCommand { get; set; }

        #endregion 

        public int SIndex { get; set; }
        public StockItemsWindowViewModel()
        {
            _ = GetIds();
            _ = GetTypes();
            SIndex = 0;
            SetItemCommand = new RelayParameterizedCommand(async (param) => await ProduitSelectedFunc(param));

            SearchCommand = new RelayCommand(async()=> await Search());

            NewProduitCommand = new RelayCommand(OpenWindow);

        }
        public async Task GetTypes()
        {
            items = new ObservableCollection<TypeProduct>();
            foreach (ProduitsPharmaceutiquesType type in Enum.GetValues(typeof(ProduitsPharmaceutiquesType)))
            {
                if (type == ProduitsPharmaceutiquesType.None)
                    continue;
                string convertedValue = type.ToProduitsPharmaceutiques();
                int i = (int)type;
                if (convertedValue != null)
                {
                    items.Add(new TypeProduct
                    {
                        Id = i,
                        type = convertedValue
                    });
                }
            }
            SIndex = 1;
        }
        public async Task GetIds()
        {
            var res = await IoC.StockManager.GetProductIdsAsync();
            ProductIds = new ObservableCollection<ProductIds>(res);
        }
        private  void OpenWindow()
        {
            NewProduitsPharmaceutiquesViewModel viewModel = new NewProduitsPharmaceutiquesViewModel(SelectedProduit.type.ToProduiyPharmaType());
          
            viewModel.ProduitAded += (sender, e) =>
            {
                if(e.SelectedProductPharma != null)
                {
                    InvoiceItems = new ObservableCollection<ProduitPharma>
                    {
                       // e.SelectedProductPharma
                    };
                }
            };
            NewProduitsPharmaceutiques newProduits = new NewProduitsPharmaceutiques(viewModel);
            newProduits.Show();
        }

        public void OnProduitSelected(ProduitPharma produitPharma)
        {
            
            ProduitSelected?.Invoke(this, new SelectProduitEventArgs { SelectedProductStock = produitPharma});
        }
        public async Task ProduitSelectedFunc(object param)
        {
            if (param is ProduitPharma item)
            {
                if(SelectedProduit.type == ProduitsPharmaceutiquesType.None.ToProduitsPharmaceutiques())
                {
                    notificationBoxViewModel = new NotificationBoxViewModel(NotificationType.Warning, "Vous Devez Choisir le Type de Produit Pharmaceutique")
                    {
                        Title = "Avertissement"
                    };
                    AttachmentNotifVisible = true;
                    await Task.Delay(3000);
                    AttachmentNotifVisible = false;

                    return;
                }
                if (ProductIds.Where(x => x.IdMedicament == item.Id).Any())
                {
                    notificationBoxViewModel = new NotificationBoxViewModel(NotificationType.Warning, "le Produit Pharmaceutique existe Déjà en Stock")
                    {
                        Title = "Avertissement"
                    };
                    AttachmentNotifVisible = true;
                    await Task.Delay(3000);
                    AttachmentNotifVisible = false;

                   // await IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Warning, "le Produit Pharmaceutique existe déjà"));
                    return;
                }
                item.Type = (int)SelectedProduit.type.ToProduiyPharmaType();
                OnProduitSelected(item);

            }
            await Task.Delay(1);

        }
        /// <summary>
        /// Searches the current message list and filters the view
        /// </summary>
        public bool IsLoading { get;  set; }

        public async Task Search()
        {
            if (string.IsNullOrEmpty(SearchText))

                return;

           
            ProduitsPharmaceutiquesType t = SelectedProduit.type.ToProduiyPharmaType();
            int enumValue = (int)t;
           
            IsLoading = true;
            await Task.Delay(1000);
     
            // Find all items that contain the given text
            // TODO: Make more efficient search
            var Result = await IoC.StockManager.GetProduitStocksAsync(SearchText, enumValue);
           
           
            InvoiceItems = new ObservableCollection<ProduitPharma>(Result);
           
            
            IsLoading = false;
        }
       
       
    }
}
