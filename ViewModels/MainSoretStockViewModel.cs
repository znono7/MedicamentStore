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

        private int _currentPage = 1;
        private int _pageSize = 20; // Number of rows per page     

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
            NextPageCommand = new RelayCommand(NextPage);
            PreviousPageCommand = new RelayCommand(PreviousPage);
            MedicamentCommand = new RelayParameterizedCommand(async (param) => await MedicamentButton(param));
            MenuVisibleCommand = new RelayCommand(MenuButton);
            NewItemCommand = new RelayCommand(async () => await ToNewStockPage());
            SearchCommand = new RelayCommand(Search);
            ViewDetailCommand = new RelayParameterizedCommand(async (param) => await ViewEntree(param));
            DeleteCommand = new RelayParameterizedCommand(async (param) => await DeleteEntree(param));
            UpdateQuantiteCommand = new RelayParameterizedCommand(async (param) => await ToNewItemStockWindow(param));

            _ = LoadStockPagedsAsync(CurrentTypePage);
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
                // _ = LoadStocksAsync(selectedType);
                _ = LoadStockPagedsAsync(CurrentTypePage);
                MenuVisible = false;
            }
            await Task.Delay(1);
        }

        private async Task LoadStockPagedsAsync(ProduitsPharmaceutiquesType currentTypePage)
        {
            CurrentTypePage = currentTypePage;
            IsLoading = true; 
            await Task.Delay(1000);
            var Result = await IoC.StockManager.GetAllSorteStocksAsync(CurrentPage, _pageSize, CurrentTypePage);

            foreach (var Stock in Result)
            {
               
                Stock.TypeMed = ((ProduitsPharmaceutiquesType)Stock.Type).ToProduitsPharmaceutiques();
            }
            EnterMedicaments = new ObservableCollection<MedicamentStock>(Result);
            //_ = GetStockNumbers(CurrentTypePage);
            IsLoading = false;
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
    }
}
