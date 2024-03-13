using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static SkiaSharp.HarfBuzz.SKShaper;

namespace MedicamentStore
{
    public class MainMovmentStockViewModel : BaseViewModel
    {
        public PaginationViewModel paginationView { get; set; }

        public ICommand MenuVisibleCommand { get; set; }
        public bool MenuVisible { get; set; }
        public string TextType { get; set; } = ProduitsPharmaceutiquesType.None.ToProduitsPharmaceutiques();
        public ICommand MedicamentCommand { get; set; }//
        public ICommand UpdateQuantiteCommand { get; set; }//   
         
        public ObservableCollection<MedicamentStock> FilteredMedicaments { get; set; }

        public ObservableCollection<MedicamentStock> EnterMedicaments  
        {
            get => mEnterMedicaments; 
            set
            {
                if (mEnterMedicaments == value)
                    return; 
                mEnterMedicaments = value;
                FilteredMedicaments = new ObservableCollection<MedicamentStock>(mEnterMedicaments);
            }
        }

        protected ObservableCollection<MedicamentStock> mEnterMedicaments {  get; set; }
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

       
       

      

        protected string mSearchText;

        
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
        public ICommand SearchCommand { get; set; }
        public ICommand ViewDetailCommand { get; set; }
        private int _pageSize = 10;
        public MainMovmentStockViewModel()
        {
            paginationView = new PaginationViewModel();
            paginationView.TotalPages = CalculateTotalPages(GetTotalItems(CurrentTypePage).Result, _pageSize);
            paginationView.PageIndexChanged += PaginationViewModel_PageIndexChanged;
            _ = GetStocksAsync(CurrentTypePage, paginationView.CurrentPageIndex, _pageSize);

            MedicamentCommand = new RelayParameterizedCommand(async (param) => await MedicamentButton(param));
            MenuVisibleCommand = new RelayCommand(MenuButton);
            SearchCommand = new RelayCommand(Search);
            ViewDetailCommand = new RelayParameterizedCommand(async (param) => await ViewEntree(param));

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
            // Stocks = new ObservableCollection<MedicamentStock>();
            CurrentTypePage = type;
            IsLoading = true;
            await Task.Delay(1000);
            var Result = await IoC.StockManager.GetPagedStocksAsync(currentPage, pageSize, CurrentTypePage);

            foreach (var Stock in Result)
            {
                Stock.TypeMed = ((ProduitsPharmaceutiquesType)Stock.Type).ToProduitsPharmaceutiques();

            }
            EnterMedicaments = new ObservableCollection<MedicamentStock>(Result);
            IsLoading = false;
        }
      
       

        private async Task ViewEntree(object param)
        {
            if(param is MedicamentStock medicament)
            { 
                IoC.Application.GoToPage(ApplicationPage.MouvementPage, new MouvementViewModel(medicament.IdProduct));
            }
            await Task.Delay(1); 
        }

       
        private void MenuButton()
        {
            MenuVisible ^= true;

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

            }
            await Task.Delay(1);
        }

       
        protected string mLastSearchText;

        private void Search()
        {
            // Make sure we don't re-search the same text
            if ((string.IsNullOrEmpty(mLastSearchText) && string.IsNullOrEmpty(SearchText)) ||
                string.Equals(mLastSearchText, SearchText))
                return;

            // If we have no search text, or no items
            if (string.IsNullOrEmpty(SearchText) || EnterMedicaments == null || EnterMedicaments.Count <= 0)
            {
                // Make filtered list the same
                FilteredMedicaments = new ObservableCollection<MedicamentStock>(EnterMedicaments ?? Enumerable.Empty<MedicamentStock>());

                // Set last search text
                mLastSearchText = SearchText;

                return;
            }

            FilteredMedicaments = new ObservableCollection<MedicamentStock>(
                EnterMedicaments.Where(item => item.Nom_Commercial.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));

            // Set last search text
            mLastSearchText = SearchText;
        }

       
    }
}
