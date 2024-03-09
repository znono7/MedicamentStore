using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MedicamentStore
{
    public class MainSoretStockViewModel : BaseViewModel
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

        private int _pageSize = 10; // Number of rows per page     

      

       

        public ICommand NextPageCommand { get; private set; }
        public ICommand PreviousPageCommand { get; private set; }

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
        public ICommand PopupClickawayCommand { get; set; }
        public ICommand NewItemCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand ViewDetailCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public MainSoretStockViewModel()
        {
            paginationView = new PaginationViewModel();
            paginationView.TotalPages = CalculateTotalPages(GetTotalItems(CurrentTypePage).Result, _pageSize);
            paginationView.PageIndexChanged += PaginationViewModel_PageIndexChanged;

           
            MedicamentCommand = new RelayParameterizedCommand(async (param) => await MedicamentButton(param));
            MenuVisibleCommand = new RelayCommand(MenuButton);
            NewItemCommand = new RelayCommand(async () => await ToNewStockPage());
            SearchCommand = new RelayCommand(Search);
            ViewDetailCommand = new RelayParameterizedCommand(async (param) => await ViewEntree(param));
            DeleteCommand = new RelayParameterizedCommand(async (param) => await DeleteEntree(param));
            UpdateQuantiteCommand = new RelayParameterizedCommand(async (param) => await ToNewItemStockWindow(param));

            _ = GetStocksAsync(CurrentTypePage, paginationView.CurrentPageIndex, _pageSize);

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
                //UpdateStatus(Stock);
                Stock.TypeMed = ((ProduitsPharmaceutiquesType)Stock.Type).ToProduitsPharmaceutiques();

            }
            EnterMedicaments = new ObservableCollection<MedicamentStock>(Result);
            IsLoading = false;
        }
        private async Task ToNewStockPage()
        {
            IoC.Application.GoToPage(ApplicationPage.NewInvoice);
            await Task.Delay(1);
        }

        private async Task DeleteEntree(object param)
        {
            if(param is MedicamentStock medicament)
            {
                var c = new ConfirmBoxDialogViewModel
                {
                    Title = "Confirmer la Suppression",
                    Message = "Sont sûrs du Processus de Suppression ?"
                };
                await IoC.ConfirmBox.ShowMessage(c);
                if (c.IsConfirmed)
                {
                    var result = await IoC.StockManager.DeleteStockAsync(medicament.Ids);
                    if (result.Successful)
                    {

                        await IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Succes, "Stock a été supprimé"));
                        FilteredMedicaments.Remove(medicament);
                        EnterMedicaments.Remove(medicament);
                    }
                    else
                    {
                        await IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Succes, result.ErrorMessage));

                    }
                }
            }
        }

        private async Task ViewEntree(object param)
        {
            if(param is MedicamentStock medicament)
            { 
                IoC.Application.GoToPage(ApplicationPage.SorteStockPage, new SorteStockViewModel(medicament.Id));
            }
            await Task.Delay(1);
        }

        private async Task ToNewItemStockWindow(object param)
        {
            if (param is MedicamentStock model)
            {
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
                        IdStock = model.Ids,
                        Quantite = model.Quantite,
                        IdUnite = model.IdUnite,
                        Type = model.Type,
                    }
                };
                IoC.Application.GoToPage(ApplicationPage.NewStockPage, new NewStockViewModel(l));
                await Task.Delay(1);
            }
                
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
