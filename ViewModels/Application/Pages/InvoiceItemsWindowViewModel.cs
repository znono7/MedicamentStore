using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MedicamentStore 
{
   public class InvoiceItemsWindowViewModel : BaseViewModel
    {
        #region Protected Members

        /// <summary>
        /// The last searched text in this list 
        /// </summary>
        protected string mLastSearchText;

        /// <summary>
        /// The text to search for in the search command
        /// </summary>
        protected string mSearchText;

        public int SIndex { get; set; } = 1;


        #endregion

        #region Public Members
        public ObservableCollection<InvoiceProduct> FilteredItems { get; set; }
        public ObservableCollection<InvoiceProduct> InvoiceItems { get => minvoiceItems; 
            set
            {
                if (minvoiceItems == value)
                    return;
                minvoiceItems = value;
                FilteredItems = new ObservableCollection<InvoiceProduct>(minvoiceItems);
            } }
        protected ObservableCollection<InvoiceProduct> minvoiceItems { get; set; }

        public ObservableCollection<TypeProduct> TypeItems { get; set; }
        public event EventHandler<SelectedItemEventArgs> ItemSelected;

        public bool IsLoading { get; set; }
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
               // if (string.IsNullOrEmpty(SearchText))
                    // Search to restore messages
                 //   Search();
            }
        }
        #endregion

        #region selected 
        private TypeProduct _selectedType { get; set; }

        public TypeProduct SelectedType
        {
            get { return _selectedType; }
            set
            {
                _selectedType = value;
                _ = GetProducts(_selectedType.Id);
                //Unit = _selectedUnite?.Id ?? 0;
            }
        }

        
        public InvoiceItem SelectedInvoiceItem { get; set; }

        #endregion

        #region Public Command

        public ICommand SetItemCommand { get; set; }

        /// <summary>
        /// The command for when the user wants to search
        /// </summary>
        public ICommand SearchCommand { get; set; }

        #endregion
        public InvoiceItemsWindowViewModel() 
        {
             
            _ = GetTypes();
            _ = GetProducts(1);
            SelectedType = TypeItems[0];
            SetItemCommand = new RelayParameterizedCommand(async (param) => await ItemSelectedC(param));

            SearchCommand = new RelayCommand(async()=> await Search());

        }

        public async Task GetProducts(int type)
        {
            IsLoading = true;
            await Task.Delay(1000);
            var Res = await IoC.InvoiceManager.GetAllInvoiceProduct(type);
            InvoiceItems = new ObservableCollection<InvoiceProduct>(Res);
            IsLoading = false;

        }
        public async Task GetTypes()
        {
            TypeItems = new ObservableCollection<TypeProduct>();
            foreach (ProduitsPharmaceutiquesType type in Enum.GetValues(typeof(ProduitsPharmaceutiquesType)))
            {
                if (type == ProduitsPharmaceutiquesType.None)
                    continue;
                string convertedValue = type.ToProduitsPharmaceutiques();
                int i = (int)type;
                if (convertedValue != null)
                {
                    TypeItems.Add(new TypeProduct
                    {
                        Id = i,
                        type = convertedValue
                    });
                }
            }
        }
        /// <summary>
        /// Searches the current message list and filters the view
        /// </summary>
        public async Task Search()
        {
            // Make sure we don't re-search the same text
            if ((string.IsNullOrEmpty(mLastSearchText) && string.IsNullOrEmpty(SearchText)) ||
                string.Equals(mLastSearchText, SearchText))
                return;

            // If we have no search text, or no items
            if (string.IsNullOrEmpty(SearchText) || InvoiceItems == null || InvoiceItems.Count <= 0)
            {
                // Make filtered list the same
                FilteredItems = new ObservableCollection<InvoiceProduct>(InvoiceItems);

                // Set last search text
                mLastSearchText = SearchText;

                return;
            }

            // Find all items that contain the given text
            // TODO: Make more efficient search


            FilteredItems = new ObservableCollection<InvoiceProduct>(InvoiceItems.Where(x => x.Nom_Commercial.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
           
            // Set last search text
            mLastSearchText = SearchText;
        }
        public void OnItemSelected(InvoiceProduct selectedItem)
        {
            ItemSelected?.Invoke(this, new SelectedItemEventArgs { SelectedItem = selectedItem });
        }
         
       
        public async Task ItemSelectedC(object param)
        {
            if (param is InvoiceProduct item) 
            {
                OnItemSelected(item);

            }
            await Task.Delay(1);

        }
    }
}
