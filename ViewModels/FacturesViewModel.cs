using Microsoft.Win32;
using QuestPDF.Fluent;
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
    public class FacturesViewModel : BaseViewModel
    {
        #region Commands
        public ICommand SavePdfCommand { get; set; }
        public ICommand PrintCommand { get; set; }
        public ICommand ExpandCommand { get; set; }
        public ICommand PopupClickawayCommand { get; set; }
        public ICommand RemoveCommand { get; set; }



        #endregion

        #region Observable collections
        public ObservableCollection<Invoice> MInvoices { get; set; }
        public ObservableCollection<Invoice> Invoices
        
        {
            get => MInvoices;
            set
            {
                MInvoices = value;
                FilterInvoices = new ObservableCollection<Invoice>(MInvoices);
            }
        }

        public ObservableCollection<Invoice> FilterInvoices { get; set; }
        #endregion

        #region Public Properties
        public bool FilterBySupplier{ get; set; }
        public bool FilterByDate { get; set; }
        public bool FilterByType { get; set; }
        public bool FilterGrid => FilterBySupplier || FilterByDate || FilterByType;

        public bool isExpanded { get; set; }
        public bool isDimmedGray { get; set; }

        public FilterTagItem FilterTag { get; set; }
       

        public PaginationViewModel pagination { get; set; }
        public bool IsLoading { get; set; }
        public int PageSize { get; set; } = 10;

        private int _SelectedType { get ; set;}
        public int SelectedType 
        { 
            get => _SelectedType; 
            set 
            { 
                _SelectedType = value;
                FilterByType = value != 0;
                FilterTag.TagName = "Filtrer par type";
                pagination.Reset();
                pagination.TotalPages = CalculateTotalPages(GetTotalItems(value).Result, PageSize);
                _ = GetInvoices(pagination.CurrentPageIndex, PageSize, SelectedType); 
            } 
        }

        public List<string> Types { get; set; }
        public DateFilterViewModel DateFilterViewModel { get; set; }
        public RelayCommand FilterDataCommand { get; private set; }
        public CustomerFilterSuppViewModel customerFilter { get; set; }
        #endregion


        #region Constructor
        public FacturesViewModel()
        {
            FilterByDate = false;
            FilterBySupplier = false;
            pagination = new PaginationViewModel();
            pagination.TotalPages = CalculateTotalPages(GetTotalItems().Result, PageSize);
            pagination.PageIndexChanged += PaginationViewModel_PageIndexChanged;
            _ = GetInvoices(pagination.CurrentPageIndex,PageSize, 0);
           Types = new List<string> { "Tous", "Entrée", "Sortie" };
            ExpandCommand = new RelayCommand(AttachmentMenuButton);
            PopupClickawayCommand = new RelayCommand(ClickawayMenuButton);
            DateFilterViewModel = new DateFilterViewModel();
            FilterDataCommand = new RelayCommand(async () => await GetDataByDate());
            customerFilter = new CustomerFilterSuppViewModel
            {
                AttachmentAction = AttachmentButton,
                SetSupplieAction = GetDataBySupplier,
                
            };
            FilterTag = new FilterTagItem { RemoveAction = ClearItems };
           
            PrintCommand = new RelayParameterizedCommand(async (p) => await PrintInvoice(p));

            SavePdfCommand = new RelayParameterizedCommand(async (p) => await SavePdf(p));
        }

        private async Task SavePdf(object p)
        {
            if(p is Invoice invoice)
            {
                List<InvoiceItemDto> items = (await GetInvoiveItems(invoice.Number)).ToList();
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

                    var document = new FactureDocument(items, invoice);
                    document.GeneratePdf(filePath);

                    Process.Start("explorer.exe", filePath);

                    MessageBox.Show("PDF saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("File saving canceled.", "Canceled", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            await Task.Delay(1);
            
        }

        private async Task PrintInvoice(object p)
        {
            if(p is Invoice invoice)
            {   
                 List<InvoiceItemDto> items = (await GetInvoiveItems(invoice.Number)).ToList();
                IoC.Application.GoToPage(ApplicationPage.PrintFactureStockPage, new PrintFactureStockViewModel(invoice, items));
            }
            await Task.Delay(1);
            
        }
        private async Task<IEnumerable<InvoiceItemDto>> GetInvoiveItems(string InvoiceNumber)
        {
            return await IoC.InvoiceManager.GetInvoiceItems(InvoiceNumber);
        }
        private async Task ClearItems()
        {
            FilterByDate = false;
            FilterBySupplier = false;
            FilterByType = false;
            FilterTag.TagName = "";
            SelectedType = 0;
            DateFilterViewModel = new DateFilterViewModel();
            pagination.Reset();
            pagination.TotalPages = CalculateTotalPages(GetTotalItems().Result, PageSize);
            
            await GetInvoices(pagination.CurrentPageIndex, PageSize, 0);
        }
        #endregion

        #region Methodes
       
        public async Task GetDataBySupplier() 
        {

            if (customerFilter.SelectedId == 0)
                return;
            FilterTag.TagName = "Filtrer par fournisseur";
            FilterBySupplier = true;
            

            customerFilter.AttachmentMenuVisible = false;
                   
            isDimmedGray = false;
                   
            customerFilter.SearchIsOpen = false;
                   
            customerFilter.ClearSearch();

            pagination.Reset();
            pagination.TotalPages = CalculateTotalPages(await GetTotalItems(0, customerFilter.SelectedId), PageSize);

            await LoadDataBySupplier(customerFilter.SelectedId, pagination.CurrentPageIndex, PageSize, 0);
               
        }
        public async Task GetDataByDate() 
        {
            FilterTag.TagName = "Filtrer par date";
            FilterByDate = true;
            customerFilter.AttachmentMenuVisible = false;

            isExpanded = false;
            isDimmedGray = false;
                
            customerFilter.SearchIsOpen = false;
            pagination.Reset();
            pagination.TotalPages = CalculateTotalPages(await GetTotalItems(DateFilterViewModel.SelectedFromDate, DateFilterViewModel.SelectedToDate,0,0), PageSize);
            (DateTime startDate, DateTime endDate) = await FilterData();
            if (startDate > endDate)
                return;
            if(DateFilterViewModel.CurrentDateFilterType == DateFilterType.None)
            {
                FilterByDate = false;
                FilterBySupplier = false;
                pagination.TotalPages = CalculateTotalPages(await GetTotalItems(0), PageSize);
                await GetInvoices(pagination.CurrentPageIndex, PageSize, 0);
                return;
            }
            await LoadDataByDate(startDate, endDate, pagination.CurrentPageIndex, PageSize, 0);
               
        }
        public async Task LoadDataBySupplier(int id, int currentPage, int pageSize, int type)
        {
            IsLoading = true;

            var r = await IoC.InvoiceManager.GetAllInvoicesBySupplie(currentPage, pageSize, type, id);
            foreach (var item in r)
            {
                if (item.InvoiceType == 1)
                {
                    item.FactType = "Entrée";
                    item.FactTypeColor = "5DA329";
                }
                else
                {
                    item.FactType = "Sortie";
                    item.NomSupplie = "/";
                    item.FactTypeColor = "FF817D";
                }
            }
            Invoices = new ObservableCollection<Invoice>(r);

             IsLoading = false;
            await Task.Delay(1);
        }
        public async Task LoadDataByDate(DateTime startDate, DateTime endDate, int currentPage, int pageSize, int type, int idSupp = 0)
        {
            IsLoading = true;
            FilterByDate = true;
            var r = await IoC.InvoiceManager.GetAllInvoicesByDate(currentPage, pageSize,startDate, endDate, type,idSupp);
            foreach (var item in r)
            {
                if (item.InvoiceType == 1)
                {
                    item.FactType = "Entrée";
                    item.FactTypeColor = "5DA329";
                }
                else
                {
                    item.FactType = "Sortie";
                    item.NomSupplie = "/";
                    item.FactTypeColor = "FF817D";
                }
            }
            Invoices = new ObservableCollection<Invoice>(r);
            IsLoading = false;
            await Task.Delay(1);
        }
        public async Task AttachmentButton()
        {

            // Toggle menu visibility
            customerFilter.AttachmentMenuVisible ^= true;
            isDimmedGray ^= true;
            customerFilter.SearchIsOpen = true;
            customerFilter.ClearSearch();

            await customerFilter.FetchSuppFromDatabase();




        }
        private async Task<(DateTime startDate, DateTime endDate)> FilterData()
        {
            DateTime startDate = new();
            DateTime endDate = new();
            switch (DateFilterViewModel.CurrentDateFilterType)
            {
                //case DateFilterType.None:
                //    FilterInvoices = new ObservableCollection<Invoice>(Invoices);
                //    return;
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
            await Task.Delay(1);
            return (startDate, endDate);
           

        }

        private void ClickawayMenuButton()
        {
            isExpanded = false;

            customerFilter.AttachmentMenuVisible = false;

            isDimmedGray = false;

        }
        private void AttachmentMenuButton()
        {
            isExpanded ^= true;
            isDimmedGray = true;
        }
        public async Task GetInvoices(int currentP , int p,int type)
        {
            IsLoading = true;
            
            await Task.Delay(500);
            var Result = await IoC.InvoiceManager.GetAllInvoices(currentP,p,type);
            foreach (var item in Result)
            {
                if (item.InvoiceType == 1)
                {
                    item.FactType = "Entrée";
                    item.FactTypeColor = "5DA329";
                }
                else
                {
                    item.FactType = "Sortie";
                    item.NomSupplie = "/";
                    item.FactTypeColor = "FF817D";
                }
            }
            Invoices = new ObservableCollection<Invoice>(Result);
            IsLoading = false;
        }
        private void PaginationViewModel_PageIndexChanged(object? sender, int e)
        {
            if(FilterBySupplier)
            {
                _ = LoadDataBySupplier(customerFilter.SelectedId, e, PageSize, 0);
                return;
            }else if(FilterByDate)
            {
                (DateTime startDate, DateTime endDate) = FilterData().Result;
                _ = LoadDataByDate(startDate, endDate, e, PageSize, 0);
                
                return;
            }
            else
            {
                _ = GetInvoices(e, PageSize, 0);

            }


        }
       
        private int CalculateTotalPages(int totalItems, int itemsPerPage)
        {
            if (totalItems == 0)
                return 0;
            if (totalItems <= itemsPerPage)
                return 1;
            return totalItems / itemsPerPage + (totalItems % itemsPerPage == 0 ? 0 : 1);
        }
        private async Task<int> GetTotalItems(int type = 0)
        {
            if (type == 0)
                return await IoC.InvoiceManager.GetTotalInvoices();
            else
                return await IoC.InvoiceManager.GetTotalInvoices(type);
        }

        private async Task<int> GetTotalItems(int type, int id)
        {
            return await IoC.InvoiceManager.GetTotalInvoicesBySupplie(type, id);
        }
        private async Task<int> GetTotalItems(DateTime startDate, DateTime endDate, int type, int idSupp = 0)
        {
            return await IoC.InvoiceManager.GetTotalInvoicesByDate(startDate, endDate, type, idSupp);
        }
        #endregion
    }
}
