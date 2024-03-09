using Microsoft.Win32;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System;
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
    

    public class SorteStockViewModel : BaseViewModel
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

        private const int PageSize = 10;


       
        public ICommand PrintCommand { get; private set; }//ReturnCommand
        public ICommand ReturnCommand { get; private set; }//

       
        public ICommand MenuVisibleCommand { get; set; }
        public ICommand NewItemCommand { get; set; }
        public ICommand SearchCommand { get; set; }


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


      
        public ICommand ExpandCommand { get; set; }
        public ICommand PopupClickawayCommand { get; set; }
        public ICommand FilterDataCommand { get; set; }
        public ICommand PrintPdfCommand { get; set; }

       
        public SorteStockViewModel(int identerMedicaments)
        {
            IdEnterMedicaments = identerMedicaments;

            PaginationView = new PaginationViewModel();
            PaginationView.TotalPages = CalculateTotalPages(GetTotalItems(IdEnterMedicaments).Result, PageSize);
            PaginationView.PageIndexChanged += PaginationViewModel_PageIndexChanged;
            _ = GetSortie(IdEnterMedicaments, PaginationView.CurrentPageIndex, PageSize);
            DateFilterViewModel = new DateFilterViewModel();
           
            FilterDataCommand = new RelayCommand(async () => await FilterData());
            PrintPdfCommand = new RelayCommand(ShowPdfDocument);
           
           
            ExpandCommand = new RelayCommand(AttachmentMenuButton);

            SearchCommand = new RelayCommand(Search);
            NewItemCommand = new RelayCommand(async () => await ToNewItemStockWindow());
           
            PrintCommand = new RelayCommand(ShowDocument);
            PopupClickawayCommand = new RelayCommand(ClickawayMenuButton);
            ReturnCommand = new RelayCommand(BackPage);
        }

        private async Task GetSortie(int idEnterMedicaments, int currentPageIndex, int pageSize)
        {
            IsLoading = true;
            // await Task.Delay(500); 
            var Result = await IoC.TransactionManager.GetAllSorte(idEnterMedicaments, currentPageIndex, pageSize);

            foreach (var S in Result)
            {

                S.TypeMed = ((ProduitsPharmaceutiquesType)S.Type).ToProduitsPharmaceutiques();
            }
            EnterTransactions = new ObservableCollection<EnterTransaction>(Result);
            IsLoading = false;
        }

        private void PaginationViewModel_PageIndexChanged(object? sender, int e)
        {
            _ = GetSortie(IdEnterMedicaments, e, PageSize);

        }

        private int CalculateTotalPages(int totalItems, int itemsPerPage)
        {
            if (totalItems == 0)
                return 0;
            if (totalItems <= itemsPerPage)
                return 1;
            return totalItems / itemsPerPage + (totalItems % itemsPerPage == 0 ? 0 : 1);
        }
        private async Task<int> GetTotalItems(int i)
        {
            return await IoC.TransactionManager.GetTotalSortieStockAsync(i);
        }
        private void BackPage()
        {
            IoC.Application.GoToPage(ApplicationPage.MainSorteStockPage);
        }

        protected ObservableCollection<EnterTransaction> MenterTransactions { get;  set; }
        public ObservableCollection<EnterTransaction> EnterTransactions { get => MenterTransactions; set
            {
                MenterTransactions = value;
                FilterTransactions = new ObservableCollection<EnterTransaction>(MenterTransactions);
            } }
        public ObservableCollection<EnterTransaction> FilterTransactions { get;  set; }
      
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

                var document = new EntreeTransactionDocument(EnterTransactions.ToList());
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
                    FilterTransactions = new ObservableCollection<EnterTransaction>(EnterTransactions);
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
            FilterTransactions = new ObservableCollection<EnterTransaction>(EnterTransactions.Where(item => item.Date >= startDate.Date &&
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
       

       

       

        private void ShowDocument()
        {

            IoC.Application.GoToPage(ApplicationPage.PrintEntreeStockPage, new PrintEntreeStockViewModel(EnterTransactions));
        }
       
       
        public int IdEnterMedicaments { get; set; }
        public PaginationViewModel PaginationView { get; private set; }

       
       
       
        private void Search()
        {
            // Make sure we don't re-search the same text
            if ((string.IsNullOrEmpty(mLastSearchText) && string.IsNullOrEmpty(SearchText)) ||
                string.Equals(mLastSearchText, SearchText))
                return;

            // If we have no search text, or no items
            if (string.IsNullOrEmpty(SearchText) || EnterTransactions == null || EnterTransactions.Count <= 0)
            {
                // Make filtered list the same
                FilterTransactions = new ObservableCollection<EnterTransaction>(EnterTransactions ?? Enumerable.Empty<EnterTransaction>());

                // Set last search text
                mLastSearchText = SearchText;

                return;
            }

            FilterTransactions = new ObservableCollection<EnterTransaction>(
                EnterTransactions.Where(item => item.Nom_Commercial.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));

            // Set last search text
            mLastSearchText = SearchText;
        }

        private async Task ToNewItemStockWindow()
        {
            if (EnterTransactions.Count == 0)
                return;
            var model = EnterTransactions.FirstOrDefault(); 
            ObservableCollection<MedicamentUpdateStock> l = new ObservableCollection<MedicamentUpdateStock>
                {
                    new MedicamentUpdateStock
                    {
                        IdMedicament = model.IdMedicament,
                        Nom_Commercial = model.Nom_Commercial,
                        Forme = model.Forme,
                        Dosage = model.Dosage,
                        Unite = model.Unite,
                        Prix = model.Prix,
                        IdStock = model.IdStock,
                        Quantite = model.Quantite,
                        IdUnite = model.IdUnite,
                        Type = model.Type,
                    }
                };
            IoC.Application.GoToPage(ApplicationPage.NewStockPage , new NewStockViewModel(l)); 
            await Task.Delay(1); 
        }
      
       


       

    }

}
