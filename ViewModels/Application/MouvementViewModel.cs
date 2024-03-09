using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using QuestPDF.Fluent;
using QuestPDF.Helpers;


namespace MedicamentStore
{
    public class MouvementViewModel : BaseViewModel
    {
        protected ObservableCollection<MouvementStocks> stocks;
        public ObservableCollection<MouvementStocks> Stocks
        {  
            get => stocks;   
            set 
            {
                if (stocks == value)
                    return;
                stocks = value; 

                FilteredStocks = new ObservableCollection<MouvementStocks>(stocks);
            }
        }
         
        public ObservableCollection<MouvementStocks> FilteredStocks { get; set; }
        public DateFilterViewModel DateFilterViewModel { get; set; }
        public ObservableCollection<TypeProduct> TypeItems { get; set; }
        public string DateStat => DateFilterViewModel.DateStat;

        public bool isExpanded {  get; set; }

        public ICommand ExpandCommand { get; set; }
        public ICommand FilterDataCommand { get; set; }
        public ICommand PopupClickawayCommand { get; set; }
        public ICommand PrintCommand { get;  set; }
        public ICommand PrintPdfCommand { get;  set; }

        public bool DimmableOverlayVisible { get; set; }
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
  
                    if(_selectedIndex == 0)
                    {
                        FilteredStocks = new ObservableCollection<MouvementStocks>(Stocks);

                    }
                    else
                    {
                        FilteredStocks = new ObservableCollection<MouvementStocks>(Stocks.Where(item => item.Type == _selectedIndex));

                    }

                }
            }
        }

        public int IdMedicament { get; set; }
        public bool IsLoading { get; set; }
        public ICommand ReturnCommand { get; set; }
        public PaginationViewModel PaginationView { get; private set; }
        public int PageSize { get; private set; } = 10;
        public MouvementViewModel(int idMedicament)
        {
            IdMedicament = idMedicament;
            PaginationView = new PaginationViewModel();
            PaginationView.TotalPages = CalculateTotalPages(GetTotalItems(IdMedicament).Result, PageSize);
            PaginationView.PageIndexChanged += PaginationViewModel_PageIndexChanged;
            DateFilterViewModel = new DateFilterViewModel();
            ExpandCommand = new RelayCommand(AttachmentMenuButton);
            PopupClickawayCommand = new RelayCommand(ClickawayMenuButton);
            FilterDataCommand = new RelayCommand(async () => await FilterData());
            PrintCommand = new RelayCommand(async () => await ShowDocument());
            PrintPdfCommand = new RelayCommand(ShowPdfDocument);
            ReturnCommand = new RelayCommand(async () => await ToBackPage());

            //_ = LoadData();
            
            _ = GetMovment(IdMedicament, PaginationView.CurrentPageIndex, PageSize);

        }
        private void PaginationViewModel_PageIndexChanged(object? sender, int e)
        {
            _ = GetMovment(IdMedicament, e, PageSize);

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
            return await IoC.TransactionManager.GetTotalMovmentStockAsync(i);
        }
        private async Task ToBackPage()
        {
            IoC.Application.GoToPage(ApplicationPage.MainMovmentStockPage);
            await Task.Delay(1);
        }
        private async Task GetMovment(int id, int pageNumber, int pageSize)
        {
            IsLoading = true;
            var Result = await IoC.TransactionManager.GetAllMovement( id,  pageNumber,  pageSize);
            if (Result != null)
            {
                Stocks = new ObservableCollection<MouvementStocks>(Result);
                foreach (var item in Stocks)
                {
                    item.TypeMed = ((ProduitsPharmaceutiquesType)item.Type).ToProduitsPharmaceutiques();
                    if(item.TypeTransaction == 1)
                    {
                        item.StockIn = $"+{item.QuantiteTransaction}";
                        item.StockOut = "/";

                    }
                    else
                    {
                        item.StockIn = "/";

                        item.StockOut = $"-{item.QuantiteTransaction}";
                    }
                }
            }
            IsLoading = false;

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

                var document = new MovmentListeDocument(FilteredStocks);
                document.GeneratePdf(filePath);

                Process.Start("explorer.exe", filePath);

                MessageBox.Show("PDF saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("File saving canceled.", "Canceled", MessageBoxButton.OK, MessageBoxImage.Warning);
            }



        }

        private async Task ShowDocument()
        {
            if(FilteredStocks.Any())
            {
                IoC.Application.GoToPage(ApplicationPage.PrintMovmentStockPage, new PrintMovmentStockViewModel(FilteredStocks));
            }

            await Task.Delay(1);
        }
      
        private async Task FilterData()
        {
            DateTime startDate = new();
            DateTime endDate = new();
            switch (DateFilterViewModel.CurrentDateFilterType)
            {
                case DateFilterType.None:
                    FilteredStocks = new ObservableCollection<MouvementStocks>(Stocks);
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
            FilteredStocks = new ObservableCollection<MouvementStocks>( Stocks.Where(item => item.Date >= startDate.Date &&
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

       
            
        
    }
}
