using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MedicamentStore
{
    public class InitialStockViewModel : BaseViewModel
    {
        private ProduitsPharmaceutiquesType _CurrentTypePage { get; set; } = ProduitsPharmaceutiquesType.None;
        public ProduitsPharmaceutiquesType CurrentTypePage { get => _CurrentTypePage;
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
                    _ = LoadStockPagedsAsync(CurrentTypePage); // Call a method to load data based on the current page
                }
            }
        }

        public ICommand NextPageCommand { get; private set; }
        public ICommand PreviousPageCommand { get; private set; }
        public ICommand PrintCommand { get; private set; }
        public ICommand UpdateQuantiteCommand { get; private set; }
        public BaseViewModel CurrentPageViewModel { get; set; } 
         
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
        public ICommand MenuVisibleCommand { get; set; }
        public ICommand NewItemCommand { get; set; }
        public ICommand SearchCommand { get; set; }

        public bool MenuVisible { get; set; }

        public string TextType { get; set; } = ProduitsPharmaceutiquesType.None.ToProduitsPharmaceutiques();

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

        public bool IsLoading { get; set; }
        
        #region Side Menu Buttons
        public ICommand MedicamentCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public List<string> SortMed { get; private set; }

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
                    if (_selectedIndex == 1)
                    {
                        FilteredStocks = new ObservableCollection<MedicamentStock>(Stocks.OrderBy(item => item.Nom_Commercial));
                    }
                    else if (_selectedIndex == 2)
                    {
                        FilteredStocks = new ObservableCollection<MedicamentStock>(Stocks.OrderBy(item => item.Quantite));
                    }
                    else if (_selectedIndex == 3)
                    {
                        FilteredStocks = new ObservableCollection<MedicamentStock>(Stocks.OrderBy(item => item.Prix));
                    }
                }
            }
        }
        #endregion
        public InitialStockViewModel()
        {
            //CurrentTypePage = ProduitsPharmaceutiquesType.None;
            //TextType = ProduitsPharmaceutiquesType.None.ToProduitsPharmaceutiques();


            //_ = LoadStocksAsync(ProduitsPharmaceutiquesType.Medicaments);
            _ = LoadStockPagedsAsync(CurrentTypePage);
            _ = GetStockNumbers(CurrentTypePage);
            SortMed = new List<string>
            {
                "Trier par",
                "Trier par Nom",
                "Trier par Quantité",
                "Trier par Prix"
            };
            
            MenuVisibleCommand = new RelayCommand(MenuButton);
            SearchCommand = new RelayCommand(Search);
            MedicamentCommand = new  RelayParameterizedCommand(async (param)=> await MedicamentButton(param));
            NewItemCommand = new RelayCommand(async () => await ToNewItemStockWindow());
            DeleteCommand = new RelayParameterizedCommand((p) => DeleteProduit(p));
            NextPageCommand = new RelayCommand(NextPage);
            PreviousPageCommand = new RelayCommand(PreviousPage);
            PrintCommand = new RelayCommand(ShowDocument);
            UpdateQuantiteCommand = new RelayParameterizedCommand((p) => UpdateQuantite(p));

        }

       

        private void UpdateQuantite(object p)
        {
            if (p is MedicamentStock medicament)
            {
                
                AddStockWindowViewModel viewModel = new AddStockWindowViewModel(medicament);

                var itemToUpdate = Stocks.FirstOrDefault(item => item.Id == medicament.Id);

                viewModel.UpdateQuantiteProduit += (sender, e) =>
                    {
                        if(itemToUpdate != null)
                        {
                            itemToUpdate.Quantite =  e.UpdateQuantiteStock.Quantite;

                            int index = Stocks.IndexOf(itemToUpdate);
                            Stocks[index] = itemToUpdate;
                            FilteredStocks = new ObservableCollection<MedicamentStock>(Stocks);
                        }
                    };
                AddStockWindow window = new AddStockWindow(viewModel);
                window.Show();
            }
        }

        private void ShowDocument()
        {

            DocumentsReportHost documentsReport = new DocumentsReportHost(CurrentTypePage);
            documentsReport.Show();
            //IoC.StockDocuments.ShowDocument(new PrintStockListViewModel(CurrentTypePage)
            //{
             
            //    Title = "Aperçu Avant Impression",
            //    TypeString = CurrentTypePage.ToProduitsPharmaceutiques()
            //});
        }
        private void NextPage()
        {
            CurrentPage++;
        }

        private void PreviousPage()
        {
            if (CurrentPage > 1)
            {
                CurrentPage--;
            }
        }
        private void DeleteProduit(object p)
        {
            if (p is MedicamentStock newProduit)
            {
                ConfirmBoxDialogViewModel viewModel = new ConfirmBoxDialogViewModel
                {
                    Message = "êtes-vous sûr du Processus de Suppression ?", Title= "Confirmer la suppression"
                };
                IoC.ConfirmBox.ShowMessage(viewModel);
                if (viewModel.IsConfirmed)
                {
                   var res = IoC.StockManager.DeleteStockAsync(new Stock
                    {
                        Id = newProduit.Ids
                    });
                    if (res.Result.Successful)
                    {
                        Stocks.Remove(newProduit);
                        FilteredStocks.Remove(newProduit);
                        IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Succes, "La suppression a été effectuée avec succès"));
                        produitTotal = Stocks.Count().ToString();
                        MontantTotal = Stocks.Sum(d => d.PrixTotal);

                    }
                    else
                    {
                        IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Error, res.Result.ErrorMessage));

                    }

                }
               
            }
        }
        public double MontantTotal {  get; set; }
        public string produitTotal { get; set; }
        private async Task LoadStocksAsync(ProduitsPharmaceutiquesType type)
        {
            IsLoading = true; 
            await Task.Delay(2000);
            var Result = await IoC.StockManager.GetMedicamentStocksAsync(type);
           
            produitTotal = Result.Count().ToString();
            foreach (var Stock in Result)
            {
                UpdateStatus(Stock);
            }
            MontantTotal = Result.Sum(d => d.PrixTotal);
            Stocks = new ObservableCollection<MedicamentStock>(Result);
            IsLoading = false;
        }

        private async Task LoadStockPagedsAsync(ProduitsPharmaceutiquesType type)
        {
           // Stocks = new ObservableCollection<MedicamentStock>();
            CurrentTypePage = type; 
            IsLoading = true;
            await Task.Delay(1000);
            var Result = await IoC.StockManager.GetPagedStocksAsync(CurrentPage, _pageSize, CurrentTypePage);
            
            foreach (var Stock in Result)
            {
                UpdateStatus(Stock);
            }
            Stocks = new ObservableCollection<MedicamentStock>(Result);
            _ = GetStockNumbers(CurrentTypePage);
            IsLoading = false;
        }
        private async Task GetStockNumbers(ProduitsPharmaceutiquesType type)
        {
            MontantTotal = await IoC.StockManager.GetAmountTotalStockAsync(type);
            produitTotal = (await IoC.StockManager.GetProduitTotalStockAsync(type)).ToString();
        }
        private void Search()
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
                Stocks.Where(item => item.Nom_Commercial.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));

            // Set last search text
            mLastSearchText = SearchText;
        }

        private async Task ToNewItemStockWindow()
        {
            IoC.Application.GoToPage(ApplicationPage.NewStockPage);
            await Task.Delay(1);
        }
        public async Task MedicamentButton(object param) 
        {
            if (param is ProduitsPharmaceutiquesType selectedType)
            {
                CurrentTypePage = selectedType;
                TextType = selectedType.ToProduitsPharmaceutiques(); 
                // _ = LoadStocksAsync(selectedType);
                _ = LoadStockPagedsAsync(CurrentTypePage);
                MenuVisible = false;
            }
            await Task.Delay(1);
        }

        private void MenuButton()
        {
            MenuVisible ^= true;

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

    }
}
