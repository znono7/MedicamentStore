using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MedicamentStore
{
   public class StockViewModel : BaseViewModel
    {
        #region Search Members

        /// <summary>
        /// The last searched text in this list
        /// </summary>
        protected string mLastSearchText;

        /// <summary>
        /// The text to search for in the search command
        /// </summary>
        protected string mSearchText;

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
                 if (string.IsNullOrEmpty(SearchText))
                // Search to restore messages
                   Search();
            }
        }
        #endregion
        protected ObservableCollection<MedicamentStock> stocks;
        public ObservableCollection<MedicamentStock> Stocks 
        { 
            get => stocks;
            set 
            {  
                if (stocks == value) 
                    return;
                stocks = value;

                FilteredStocks = new ObservableCollection<MedicamentStock>(stocks);
            } 
        }

        public ObservableCollection<MedicamentStock> FilteredStocks { get; set; }

        public bool IsLoading { get; set; }

        public List<string> SortMed { get; set; }
        private int _selectedIndex;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                if (_selectedIndex != value)
                {
                    _selectedIndex = value;
                    OnPropertyChanged(nameof(SelectedIndex));

                    // Sort the YourItems collection based on the selected index
                    if (_selectedIndex == 0)
                    {
                        FilteredStocks = new ObservableCollection<MedicamentStock>(Stocks.OrderBy(item => item.Nom_Commercial));
                    }
                    else if (_selectedIndex == 1)
                    {
                        FilteredStocks = new ObservableCollection<MedicamentStock>(Stocks.OrderBy(item => item.Quantite));
                    }
                    else if (_selectedIndex == 2)
                    {
                        FilteredStocks = new ObservableCollection<MedicamentStock>(Stocks.OrderBy(item => item.Prix));
                    }
                }
            }
        }
        public ICommand NewItemCommand { get; set; }
        public ICommand AllStocksCommand { get; set; }
        public ICommand LowStocksCommand { get; set; }
        public ICommand OutStocksCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand AddStockWindowCommand { get; set; }

        public StockViewModel()
        {
            
            NewItemCommand = new RelayCommand(async () => await ToNewItemStockWindow());
            AllStocksCommand = new RelayCommand(AllStocks);
            LowStocksCommand = new RelayCommand(LowStocks);
            OutStocksCommand = new RelayCommand(OutStocks);
            SearchCommand = new RelayCommand( Search);

            IsLoading = true;
            _ = LoadStocksAsync();
            SortMed = new List<string>
            {
                "Trier par Nom",
                "Trier par Quantité",
                "Trier par Prix"
            };
            SelectedIndex = 0;

        }

      

        public void Search()
        {
            // Make sure we don't re-search the same text
            if ((string.IsNullOrEmpty(mLastSearchText) && string.IsNullOrEmpty(SearchText)) ||
                string.Equals(mLastSearchText, SearchText))
                return;

            // If we have no search text, or no items
            if (string.IsNullOrEmpty(SearchText) || Stocks == null || Stocks.Count <= 0)
            {
                // Make filtered list the same
                 FilteredStocks = new ObservableCollection<MedicamentStock>(Stocks ?? Enumerable.Empty<MedicamentStock>());

                // Set last search text
                mLastSearchText = SearchText;

                return;
            }

            FilteredStocks = new ObservableCollection<MedicamentStock>(
                Stocks.Where(item => item.Nom_Commercial.Contains(SearchText, StringComparison.OrdinalIgnoreCase)) );

            // Set last search text
            mLastSearchText = SearchText;
        }

        private void LowStocks()
        {
            FilteredStocks = new ObservableCollection<MedicamentStock>(Stocks.Where(stock => stock.Quantite > 0 && stock.Quantite < 5));
        }

        private void OutStocks()
        {
            FilteredStocks = new ObservableCollection<MedicamentStock>(Stocks.Where(stock => stock.Quantite == 0 ));

        }

        private void AllStocks()
        {
            FilteredStocks = new ObservableCollection<MedicamentStock>(Stocks);
        }

        private async Task LoadStocksAsync()
        {
            await Task.Delay(2000);
            var Result = await IoC.StockManager.GetMedicamentStocksAsync(ProduitsPharmaceutiquesType.Medicaments);
            foreach (var Stock in Result)
            {
                UpdateStatus(Stock);
            }
            Stocks = new ObservableCollection<MedicamentStock>(Result);
            IsLoading = false;
        }

        private async Task ToNewItemStockWindow()
        {
            IoC.Application.GoToPage(ApplicationPage.NewStockPage);
            await Task.Delay(1);
            //StockItemsWindowViewModel viewModel = new StockItemsWindowViewModel();
            //viewModel.ItemSelected += (sender, e) =>
            //{

            //    if (e.SelectedItemStock != null)
            //    {
            //        if (Stocks.Contains(e.SelectedItemStock))
            //        {
            //            IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Warning, "le Médicament existe déjà"));
            //            return;
            //        }
            //        Stocks.Add(e.SelectedItemStock);
            //    }

            //};
            //AddStock window = new AddStock(viewModel);
            //window.Show();
        }
        private void UpdateStatus(MedicamentStock stock)
        {
            //if(stock.Img == "0")
            //{
            //    stock.Img = $"../Pictures/24.png";

            //}
            //else
            //{
            //    stock.Img = $"../Pictures/{stock.Img}";
            //}
           
            if (stock.Quantite == 0)
            {
                stock.Status = "En Rupture";
                stock.PrimaryBackground = "FF423C";
               
            }else
            if (stock.Quantite > 0 && stock.Quantite < 5)
            {
                stock.Status = "Faible";
                stock.PrimaryBackground = "F09E43";
            }
            else
            {
                stock. Status = "Disponible";
                stock. PrimaryBackground = "349432";
            }
            //double x = stock.Prix * stock.Quantite;
            //stock.PrixTotal = string.Format("{0:0.00}", x);
            stock.PrixTotal = double.Parse(string.Format("{0:0.00}", stock.Prix * stock.Quantite));

        }

    }
}
