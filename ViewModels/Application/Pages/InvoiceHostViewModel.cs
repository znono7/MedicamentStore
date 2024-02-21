using Microsoft.Win32;
using QuestPDF.Fluent;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MedicamentStore 
{
    public class InvoiceHostViewModel : BaseViewModel
    {
        public DateFilterViewModel DateFilterViewModel { get; set; }

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
                    _ = LoadStockPagedsAsync(CurrentTypePage); // Call a method to load data based on the current page
                }
            }
        }

        public ICommand NextPageCommand { get; private set; }
        public ICommand PreviousPageCommand { get; private set; }
        public ICommand PrintCommand { get; private set; }
        public ICommand UpdateQuantiteCommand { get; private set; }
        public BaseViewModel CurrentPageViewModel { get; set; }

        protected ObservableCollection<TransactionDto> stocks;
        public ObservableCollection<TransactionDto> Stocks
        {
            get => stocks;
            set
            {
                if (stocks == value)
                    return;
                stocks = value;

                FilteredStocks = new ObservableCollection<TransactionDto>(stocks);
            }
        }

        public ObservableCollection<TransactionDto> FilteredStocks { get; set; }
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
                        FilteredStocks = new ObservableCollection<TransactionDto>(Stocks.OrderBy(item => item.Nom_Commercial));
                    }
                    else if (_selectedIndex == 2)
                    {
                        FilteredStocks = new ObservableCollection<TransactionDto>(Stocks.OrderBy(item => item.Quantite));
                    }
                    else if (_selectedIndex == 3)
                    {
                        FilteredStocks = new ObservableCollection<TransactionDto>(Stocks.OrderBy(item => item.Prix));
                    }
                }
            }
        }
        #endregion
        public ICommand ExpandCommand { get; set; }
        public ICommand PopupClickawayCommand { get; set; }
        public ICommand FilterDataCommand { get; set; }
        public ICommand PrintPdfCommand { get; set; }

        public InvoiceHostViewModel()
        {
            DateFilterViewModel = new DateFilterViewModel();
            StockInitialCommand = new RelayCommand(SetStockInitial);
            StockEnterCommand = new RelayCommand(SetStockEnter);
            FilterDataCommand = new RelayCommand(async () => await FilterData());
            PrintPdfCommand = new RelayCommand(ShowPdfDocument);

            _ = LoadStockPagedsAsync(CurrentTypePage);
            // _ = GetStockNumbers(CurrentTypePage);
            SortMed = new List<string>
            {
                "Trier par",
                "Trier par Nom",
                "Trier par Quantité",
                "Trier par Prix"
            };
            ExpandCommand = new RelayCommand(AttachmentMenuButton);

            MenuVisibleCommand = new RelayCommand(MenuButton);
            SearchCommand = new RelayCommand(Search);
            MedicamentCommand = new RelayParameterizedCommand(async (param) => await MedicamentButton(param));
            NewItemCommand = new RelayCommand(async () => await ToNewItemStockWindow());
            DeleteCommand = new RelayParameterizedCommand((p) => DeleteProduit(p));
            NextPageCommand = new RelayCommand(NextPage);
            PreviousPageCommand = new RelayCommand(PreviousPage);
            PrintCommand = new RelayCommand(ShowDocument);
            UpdateQuantiteCommand = new RelayParameterizedCommand((p) => UpdateQuantite(p));
            PopupClickawayCommand = new RelayCommand(ClickawayMenuButton);

        }
        public bool isExpanded { get; set; }

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

                var document = new SoretStockDocument(FilteredStocks);
                document.GeneratePdf(filePath);

                Process.Start("explorer.exe", filePath);

                MessageBox.Show("PDF saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("File saving canceled.", "Canceled", MessageBoxButton.OK, MessageBoxImage.Warning);
            }



        }

        private async Task FilterData()
        {
            DateTime startDate = new();
            DateTime endDate = new();
            switch (DateFilterViewModel.CurrentDateFilterType)
            {
                case DateFilterType.None:
                    FilteredStocks = new ObservableCollection<TransactionDto>(Stocks);
                    return;
                case DateFilterType.Today:
                    startDate = DateTime.Today;
                    endDate = DateTime.Today.AddDays(1).AddTicks(-1);
                    break;
                case DateFilterType.Yesterday:
                    startDate = DateTime.Today.AddDays(-1);
                    endDate = DateTime.Today.AddTicks(-1);
                    break;
                case DateFilterType.ThisMonth:
                    startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    endDate = startDate.AddMonths(1).AddTicks(-1);
                    break;
                case DateFilterType.PastMonth:
                    startDate = new DateTime(DateTime.Today.AddMonths(-1).Year, DateTime.Today.AddMonths(-1).Month, 1);
                    endDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddTicks(-1);
                    break;
                case DateFilterType.Past3Month:
                    startDate = new DateTime(DateTime.Today.AddMonths(-3).Year, DateTime.Today.AddMonths(-3).Month, 1);
                    endDate = DateTime.Today.AddTicks(-1);
                    break;
                case DateFilterType.WithDate:
                    startDate = DateFilterViewModel.SelectedFromDate;
                    endDate = DateFilterViewModel.SelectedToDate;
                    break;

                default:
                    startDate = DateTime.Today;
                    endDate = DateTime.Today.AddDays(1).AddTicks(-1);
                    break;
            }
            if (startDate > endDate)
                return;
            FilteredStocks = new ObservableCollection<TransactionDto>(Stocks.Where(item => item.Date >= startDate.Date &&
                                                                                            item.Date <= endDate.Date));
            await Task.Delay(1);
        }
        private void ClickawayMenuButton()
        {
            isExpanded = false;
        }

        private void AttachmentMenuButton()
        {
            isExpanded ^= true;
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

                AddStockWindowViewModel viewModel = new AddStockWindowViewModel(medicament);

                var itemToUpdate = Stocks.FirstOrDefault(item => item.Id == medicament.Id);

                viewModel.UpdateQuantiteProduit += (sender, e) =>
                {
                    if (itemToUpdate != null)
                    {
                        itemToUpdate.Quantite = e.UpdateQuantiteStock.Quantite;

                        int index = Stocks.IndexOf(itemToUpdate);
                        Stocks[index] = itemToUpdate;
                        FilteredStocks = new ObservableCollection<TransactionDto>(Stocks);
                    }
                };
                AddStockWindow window = new AddStockWindow(viewModel);
                window.Show();
            }
        }

        private void ShowDocument()
        {

            IoC.Application.GoToPage(ApplicationPage.PrintSorteStockPage, new PrintSorteStockViewModel(FilteredStocks));
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
            if (p is TransactionDto newProduit)
            {
                ConfirmBoxDialogViewModel viewModel = new ConfirmBoxDialogViewModel
                {
                    Message = "êtes-vous sûr du Processus de Suppression ?",
                    Title = "Confirmer la suppression"
                };
                IoC.ConfirmBox.ShowMessage(viewModel);
                if (viewModel.IsConfirmed)
                {
                    var res = IoC.StockManager.DeleteStockAsync(newProduit.IdStock);
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
        public double MontantTotal { get; set; }
        public string produitTotal { get; set; }
       

        private async Task LoadStockPagedsAsync(ProduitsPharmaceutiquesType type)
        {

            CurrentTypePage = type;
            IsLoading = true;
            await Task.Delay(1000);
            var Result = await IoC.StockManager.GetPagedSorteStocksAsync(CurrentPage, _pageSize, CurrentTypePage);

            foreach (var Stock in Result)
            {
                Stock.SymbleType = "-";
                Stock.PrimaryBackground = "EF4444";
                Stock.PrixTotal = double.Parse(string.Format("{0:0.00}", Stock.Prix * Stock.Quantite));
                Stock.TypeMed = ((ProduitsPharmaceutiquesType)Stock.Type).ToProduitsPharmaceutiques();
            }
            Stocks = new ObservableCollection<TransactionDto>(Result);
            //_ = GetStockNumbers(CurrentTypePage);
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
                FilteredStocks = new ObservableCollection<TransactionDto>(Stocks ?? Enumerable.Empty<TransactionDto>());

                // Set last search text
                mLastSearchText = SearchText;

                return;
            }

            FilteredStocks = new ObservableCollection<TransactionDto>(
                Stocks.Where(item => item.Nom_Commercial.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));

            // Set last search text
            mLastSearchText = SearchText;
        }

        private async Task ToNewItemStockWindow()
        {
            IoC.Application.GoToPage(ApplicationPage.NewInvoice ); 
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


        private void UpdateStatus(TransactionDto stock)
        {

           

            stock.PrixTotal = double.Parse(string.Format("{0:0.00}", stock.Prix * stock.Quantite));

        }

    }
}
