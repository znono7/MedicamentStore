
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace MedicamentStore 
{
   public class InvoiceItemsWindowViewModel : BaseViewModel
    {
        public bool MenuVisible { get; set; } = false;
        public TypeProduitViewModel TypeProduitCtrl { get; set; }
        public ObservableCollection<MedicamentStock> FilteredStocks { get; set; }
        public ObservableCollection<MedicamentStock> Stocks
        {
            get => mStocks;
            set
            {
                if (mStocks == value) 
                    return;
                mStocks = value;
                FilteredStocks = new ObservableCollection<MedicamentStock>(mStocks);
            }
        }
        protected ObservableCollection<MedicamentStock> mStocks { get; set; }

        public int MaxPageCount { get; set; }
        private ProduitsPharmaceutiquesType _CurrentTypePage { get; set; } = ProduitsPharmaceutiquesType.None;
        public ProduitsPharmaceutiquesType CurrentTypePage
        {
            get => _CurrentTypePage;
            set
            {
                if (_CurrentTypePage != value)
                {
                    _CurrentTypePage = value;
                    OnPropertyChanged(nameof(CurrentTypePage));
                }

            }
        }

        private int _currentPage = 1;
        private int _pageSize = 10; // Number of rows per page       

        public int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    OnPropertyChanged(nameof(CurrentPage));
                    //_ = LoadStockPagedsAsync(CurrentTypePage); // Call a method to load data based on the current page
                }
            }
        }
        public ICommand NextPageCommand { get; private set; }
        public ICommand PreviousPageCommand { get; private set; }//PopupClickawayCommand
        public ICommand PopupClickawayCommand { get;  set; }//
        #region Protected Members

        /// <summary>
        /// The last searched text in this list 
        /// </summary>
        protected string mLastSearchText;

        /// <summary>
        /// The text to search for in the search command
        /// </summary>
        protected string mSearchText;

        public int SIndex { get; set; } = 1;


        #endregion

        #region Public Members 
      

        public ObservableCollection<TypeProduct> TypeItems { get; set; } 
        public event EventHandler<SelectedItemEventArgs> ItemSelected;

        public bool IsLoading { get; set; }
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

                // If the search text is empty...
               // if (string.IsNullOrEmpty(SearchText))
                    // Search to restore messages
                 //   Search();
            }
        }
        #endregion

        #region selected 
        private TypeProduct _selectedType { get; set; }

        public TypeProduct SelectedType
        {
            get { return _selectedType; }
            set
            {
                _selectedType = value;
                //_ = GetProducts(_selectedType.Id);
                //Unit = _selectedUnite?.Id ?? 0;
            }
        }

        
        public InvoiceItem SelectedInvoiceItem { get; set; }

        #endregion

        #region Public Command

        public ICommand SetItemCommand { get; set; }

        /// <summary>
        /// The command for when the user wants to search
        /// </summary>
        public ICommand SearchCommand { get; set; }//Command="{Binding MenuVisibleCommand}"
        public ICommand MenuVisibleCommand { get; set; }//Command="{Binding MenuVisibleCommand}"

        #endregion
        public InvoiceItemsWindowViewModel() 
        {
            TypeProduitCtrl = new TypeProduitViewModel { CommitAction = SetType };
          //_ = GetTypes();
            _ = LoadStockPagedsAsync(CurrentTypePage);
           // SelectedType = TypeItems[0];
            SetItemCommand = new RelayParameterizedCommand(async (param) => await ItemSelectedC(param));

            SearchCommand = new RelayCommand(async()=> await Search());
            MenuVisibleCommand = new RelayCommand(MenuButton);
            PopupClickawayCommand = new RelayCommand(HideMenuButton);
        }

        private void HideMenuButton()
        {
            MenuVisible = false;

        }

        private void MenuButton()
        {
            MenuVisible ^= true;

        }
        private async Task SetType(object arg)
        {
            if(arg is ProduitsPharmaceutiquesType type)
            {
                await LoadStockPagedsAsync(type);
            }
        }

        private async Task LoadStockPagedsAsync(ProduitsPharmaceutiquesType type)
        {
            CurrentTypePage = type;
            IsLoading = true;
            await Task.Delay(1000);
            var Result = await IoC.StockManager.GetPagedStocksAsync(CurrentPage, _pageSize, CurrentTypePage);

            foreach (var Stock in Result)
            {
                UpdateStatus(Stock);
                Stock.TypeMed = ((ProduitsPharmaceutiquesType)Stock.Type).ToProduitsPharmaceutiques();

            }
            Stocks = new ObservableCollection<MedicamentStock>(Result);
            _ = GetStockNumbers(CurrentTypePage);
            IsLoading = false;
        }
        private async Task GetStockNumbers(ProduitsPharmaceutiquesType type)
        {
            var MontantTotal = await IoC.StockManager.GetAmountTotalStockAsync(type);
            var produitTotal = (await IoC.StockManager.GetProduitTotalStockAsync(type)).ToString();
        }

        private void UpdateStatus(MedicamentStock stock)
        {


            if (stock.Quantite == 0)
            {
                stock.Status = "En Rupture";
                stock.PrimaryBackground = "FF423C";

            }
            else
            if (stock.Quantite > 0 && stock.Quantite < 5)
            {
                stock.Status = "Faible";
                stock.PrimaryBackground = "F09E43";
            }
            else
            {
                stock.Status = "Disponible";
                stock.PrimaryBackground = "349432";
            }
            //double x = stock.Prix * stock.Quantite;
            //stock.PrixTotal = string.Format("{0:0.00}", x);
            stock.PrixTotal = double.Parse(string.Format("{0:0.00}", stock.Prix * stock.Quantite));

        }

        public async Task MedicamentButton(object param)
        {
            if (param is ProduitsPharmaceutiquesType selectedType)
            {
                CurrentTypePage = selectedType;
               // TextType = selectedType.ToProduitsPharmaceutiques();
                // _ = LoadStocksAsync(selectedType);
                _ = LoadStockPagedsAsync(CurrentTypePage);
                //MenuVisible = false;
            }
            await Task.Delay(1);
        }
        public async Task GetTypes()
        {
            TypeItems = new ObservableCollection<TypeProduct>();
            foreach (ProduitsPharmaceutiquesType type in Enum.GetValues(typeof(ProduitsPharmaceutiquesType)))
            {
                if (type == ProduitsPharmaceutiquesType.None)
                    continue;
                string convertedValue = type.ToProduitsPharmaceutiques();
                int i = (int)type;
                if (convertedValue != null)
                {
                    TypeItems.Add(new TypeProduct
                    {
                        Id = i,
                        type = convertedValue
                    });
                }
            }
        }
        /// <summary>
        /// Searches the current message list and filters the view
        /// </summary>
        public async Task Search()
        {
            // Make sure we don't re-search the same text
            if ((string.IsNullOrEmpty(mLastSearchText) && string.IsNullOrEmpty(SearchText)) ||
                string.Equals(mLastSearchText, SearchText))
                return;

            // If we have no search text, or no items
            if (string.IsNullOrEmpty(SearchText) || Stocks == null || Stocks.Count <= 0)
            {
                // Make filtered list the same
                FilteredStocks = new ObservableCollection<MedicamentStock>(Stocks);

                // Set last search text
                mLastSearchText = SearchText;

                return;
            }

            // Find all items that contain the given text
            // TODO: Make more efficient search


            FilteredStocks = new ObservableCollection<MedicamentStock>
                (Stocks.Where(x => x.Nom_Commercial.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
           
            // Set last search text
            mLastSearchText = SearchText;
        }
        public void OnItemSelected(InvoiceProduct selectedItem)
        {
            ItemSelected?.Invoke(this, new SelectedItemEventArgs { SelectedItem = selectedItem });
        }
         
       
        public async Task ItemSelectedC(object param)
        {
            if (param is InvoiceProduct item) 
            {
                OnItemSelected(item);

            }
            await Task.Delay(1);

        }
    }
}
