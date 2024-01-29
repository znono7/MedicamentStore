using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MedicamentStore
{
    public class InvoiceHostViewModel : BaseViewModel
    {
        #region Protected Property
        protected string mLastSearchText;
        protected string mSearchText;
        protected ObservableCollection<Invoice> minvoices { get; set; }

        private int _currentPage = 1;
        private int _pageSize = 10; // Number of rows per page 
        #endregion
        #region Public Property
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

        public ObservableCollection<Invoice> FilteredInvoices { get; set; }
        public ObservableCollection<Invoice> Invoices 
        {
            get => minvoices;
            set
            {
                if (minvoices == value)
                    return;
                minvoices = value;
                FilteredInvoices = new ObservableCollection<Invoice>(minvoices);
            }
        }

        public bool AttachmentMenuVisible { get; private set; }

      

        public int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    OnPropertyChanged(nameof(CurrentPage));
                    _ = GetAllInvoice(_currentPage); // Call a method to load data based on the current page
                }
            }
        }
        #endregion

        #region Public Commands
        public ICommand NewItemCommand { get; set; }

        public ICommand SearchCommand { get; set; }

        public ICommand NextPageCommand { get; private set; }
        public ICommand PreviousPageCommand { get; private set; }
        // public ICommand AttachmentButtonCommand { get; set; }
        #endregion

        public InvoiceHostViewModel()
        {
            _ = GetAllInvoice(CurrentPage);

            NewItemCommand = new RelayCommand(async () => await toNewPage());
            SearchCommand = new RelayCommand(Search);
            NextPageCommand = new RelayCommand(NextPage);
            PreviousPageCommand = new RelayCommand(PreviousPage);

         //   AttachmentButtonCommand = new RelayCommand(async () => await AttachmentButton());

        }

        //private async Task AttachmentButton()
        //{
        //    AttachmentMenuVisible ^= true;
        //    await Task.Delay(1);
        //}

        private async Task toNewPage()
        {
            IoC.Application.GoToPage(ApplicationPage.NewInvoice);
            await Task.Delay(1);
        }

        private void Search()
        {
            // Make sure we don't re-search the same text
            if ((string.IsNullOrEmpty(mLastSearchText) && string.IsNullOrEmpty(SearchText)) ||
                string.Equals(mLastSearchText, SearchText))
                return;

            // If we have no search text, or no items
            if (string.IsNullOrEmpty(SearchText) || Invoices == null || Invoices.Count <= 0)
            {
                // Make filtered list the same
                FilteredInvoices = new ObservableCollection<Invoice>(Invoices ?? Enumerable.Empty<Invoice>());

                // Set last search text
                mLastSearchText = SearchText;

                return;
            }

            FilteredInvoices = new ObservableCollection<Invoice>(
                Invoices.Where(item => item.Date.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));

            // Set last search text
            mLastSearchText = SearchText;
        }

        public async Task GetAllInvoice(int page)
        {
            var r = await IoC.InvoiceManager.GetAllInvoices(page,_pageSize);
            Invoices = new ObservableCollection<Invoice>(r);
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
