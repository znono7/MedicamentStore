using Microsoft.Win32;
using QuestPDF.Fluent;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization; 
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input; 

namespace MedicamentStore
{
    public class StockHostViewModel : BaseViewModel 
    { 
        public PaginationViewModel paginationView { get; set; }
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
           
        private int _pageSize = 10;    
         
        public ICommand NextPageCommand { get; private set; }
        public ICommand PreviousPageCommand { get; private set; } 
        public ICommand PrintCommand { get; private set; }
        public ICommand UpdateQuantiteCommand { get; private set; }
        public ICommand PrintPdfCommand { get; set; }
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

        public ICommand StockInitialCommand { get; set; }
        public ICommand StockEnterCommand { get; set; }
        public bool IsLoading { get; set; }
        public bool StockInitialPanel { get; set; }
        public bool StockEnterPanel { get; set; }

        #region Side Menu Buttons
        public ICommand MedicamentCommand { get; set; }
        public ICommand DeleteCommand { get; set; }

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

        public ICommand PopupClickawayCommand { get; set; }
        public StockHostViewModel()
        {
            paginationView = new PaginationViewModel();
            paginationView.TotalPages = CalculateTotalPages(GetTotalItems(CurrentTypePage).Result, _pageSize);
            paginationView.PageIndexChanged += PaginationViewModel_PageIndexChanged;

            StockInitialCommand = new RelayCommand(SetStockInitial);
            StockEnterCommand = new RelayCommand(SetStockEnter);

            _ = GetStocksAsync(CurrentTypePage, paginationView.CurrentPageIndex, _pageSize);
            //_ = GetStockNumbers(CurrentTypePage);
            
            MenuVisibleCommand = new RelayCommand(MenuButton);
            SearchCommand = new RelayCommand(Search);
            MedicamentCommand = new  RelayParameterizedCommand(async (param)=> await MedicamentButton(param));
            NewItemCommand = new RelayCommand(async () => await ToNewItemStockWindow());
            DeleteCommand = new RelayParameterizedCommand((p) => DeleteProduit(p));
           
            PrintCommand = new RelayCommand(ShowDocument);
            UpdateQuantiteCommand = new RelayParameterizedCommand((p) => UpdateQuantite(p));
            PrintPdfCommand = new RelayCommand(ShowPdfDocument);
            PopupClickawayCommand = new RelayCommand(HideMenu);
        }

        private void HideMenu()
        {
            MenuVisible = false;
        }

        private void PaginationViewModel_PageIndexChanged(object? sender, int e)
        {
            _ = GetStocksAsync(CurrentTypePage, e, _pageSize);
        }

        private int CalculateTotalPages(int totalItems, int itemsPerPage)
        {
            if(totalItems == 0)
                return 0;
            if (totalItems <= itemsPerPage)
                return 1;
            return totalItems / itemsPerPage + (totalItems % itemsPerPage == 0 ? 0 : 1);
        }
        private async Task<int> GetTotalItems(ProduitsPharmaceutiquesType type)
        {
           return await IoC.StockManager.GetProduitTotalStockAsync(type);
        }
        private void ShowPdfDocument()
        {

            // Create a SaveFileDialog
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;

            // Show the dialog and get the selected file path
            bool? result = saveFileDialog.ShowDialog();

            if (result == true)
            {
                string filePath = saveFileDialog.FileName;

                var document = new InitialStockDocument(FilteredStocks);
                document.GeneratePdf(filePath);

                Process.Start("explorer.exe", filePath);

                MessageBox.Show("PDF saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("File saving canceled.", "Canceled", MessageBoxButton.OK, MessageBoxImage.Warning);
            }



        }

        private void SetStockEnter()
        {
            if (StockEnterPanel)
                return;
            StockEnterPanel = true;
            StockInitialPanel = false;
        }

        private void SetStockInitial()
        {
            if (StockInitialPanel)
                return;
            StockEnterPanel = false;
            StockInitialPanel = true;
        }

        private void UpdateQuantite(object p)
        {
            if (p is MedicamentStock medicament) 
            {
                ObservableCollection<MedicamentUpdateStock> l = new ObservableCollection<MedicamentUpdateStock> 
                { 
                    new MedicamentUpdateStock
                    {
                        IdMedicament = medicament.Id,
                        Nom_Commercial = medicament.Nom_Commercial,
                        Forme = medicament.Forme,
                        Dosage = medicament.Dosage,
                        Unite = medicament.Unite,
                        Prix = medicament.Prix,
                        IdStock = medicament.Ids,
                        Quantite = medicament.Quantite,
                        IdUnite = medicament.IdUnite,
                        Type = medicament.Type,
                    }
                };
               
                IoC.Application.GoToPage(ApplicationPage.NewStockPage,new NewStockViewModel(l));

               
            }
        }

        private void ShowDocument()
        {

            IoC.Application.GoToPage(ApplicationPage.PrintInitialStockPage, new PrintInitialStockViewModel(FilteredStocks));
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
                   var res = IoC.StockManager.DeleteStockAsync(newProduit.Ids);
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
       
        private async Task GetStocksAsync(ProduitsPharmaceutiquesType type, int currentPage, int pageSize)
        {
            // Stocks = new ObservableCollection<MedicamentStock>();
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
                paginationView.Reset();
                await GetStocksAsync(CurrentTypePage, paginationView.CurrentPage, _pageSize);
                paginationView.TotalPages = CalculateTotalPages(GetTotalItems(CurrentTypePage).Result, _pageSize);

                //MenuVisible = false;
            }
            await Task.Delay(1);
        }

        private void MenuButton()
        {
            MenuVisible ^= true;

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

    }
}
