
using QuestPDF.Infrastructure;
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
        public PaginationViewModel paginationView { get; set; }
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
            paginationView = new PaginationViewModel();
            paginationView.TotalPages = CalculateTotalPages(GetTotalItems(CurrentTypePage).Result, _pageSize);
            paginationView.PageIndexChanged += PaginationViewModel_PageIndexChanged;
            _ = GetStocksAsync(CurrentTypePage, paginationView.CurrentPageIndex, _pageSize);

            SetItemCommand = new RelayParameterizedCommand(async (param) => await ItemSelectedC(param));

            SearchCommand = new RelayCommand(async()=> await Search());
            MenuVisibleCommand = new RelayCommand(MenuButton);
            PopupClickawayCommand = new RelayCommand(HideMenuButton);
        }
        private void PaginationViewModel_PageIndexChanged(object? sender, int e)
        {
            _ = GetStocksAsync(CurrentTypePage, e, _pageSize);
        }

        private int CalculateTotalPages(int totalItems, int itemsPerPage)
        {
            if (totalItems == 0)
                return 0;
            if (totalItems <= itemsPerPage)
                return 1;
            return totalItems / itemsPerPage + (totalItems % itemsPerPage == 0 ? 0 : 1);
        }
        private async Task<int> GetTotalItems(ProduitsPharmaceutiquesType type)
        {
            return await IoC.StockManager.GetProduitTotalStockAsync(type);
        }

        private async Task GetStocksAsync(ProduitsPharmaceutiquesType type, int currentPage, int pageSize)
        {
            CurrentTypePage = type;
            IsLoading = true;
            await Task.Delay(1000);
            var Result = await IoC.StockManager.GetPagedStocksAsync(currentPage, pageSize, CurrentTypePage);

            foreach (var Stock in Result)
            {
                UpdateStatus(Stock);
                Stock.TypeMed = ((ProduitsPharmaceutiquesType)Stock.Type).ToProduitsPharmaceutiques();

            }
            Stocks = new ObservableCollection<MedicamentStock>(Result);
            IsLoading = false;
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
                CurrentTypePage = type;
                paginationView.Reset();
                await GetStocksAsync(CurrentTypePage, paginationView.CurrentPage, _pageSize);
                paginationView.TotalPages = CalculateTotalPages(GetTotalItems(CurrentTypePage).Result, _pageSize);
            }
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
        public void OnItemSelected(MedicamentStock selectedItem)
        {
            ItemSelected?.Invoke(this, new SelectedItemEventArgs { SelectedItem = selectedItem });
        }
         
       
        public async Task ItemSelectedC(object param)
        {
            if (param is MedicamentStock item) 
            {
                OnItemSelected(item);

            }
            await Task.Delay(1);

        }
    }
}
