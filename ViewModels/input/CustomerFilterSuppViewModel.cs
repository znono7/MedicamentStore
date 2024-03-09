﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MedicamentStore
{
   public class CustomerFilterSuppViewModel : BaseViewModel 
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


        /// <summary>
        /// clients for the list
        /// </summary>
        protected ObservableCollection<Supplies> mItems;

        /// <summary>
        /// A flag indicating if the search dialog is open
        /// </summary>
        protected bool mSearchIsOpen;

        #endregion



        #region selected Client
        private int selectedId;
        public int SelectedId
        {
            get { return selectedId; }
            set
            {
                    selectedId = value;
                    OnPropertyChanged(nameof(SelectedId));
                
            }
        }

        private string selectedName;
        public string SelectedName
        {
            get { return selectedName; }
            set
            {
                
                
                    selectedName = value;
                    OnPropertyChanged(nameof(SelectedName));
                
            }
        }

        private string selectedAdresse;
        public string SelectedAdresse
        {
            get { return selectedAdresse; }
            set
            {
                
                
                    selectedAdresse = value;
                    OnPropertyChanged(nameof(SelectedAdresse));
                
            }
        }
        public Supplies Supplies
        { get; set; }
        #endregion

        #region Public Properties

        public bool IsLoading { get; set; }

        public bool FirstLoad { get; set; }
        /// <summary>
        /// The chat thread items for the list
        /// NOTE: Do not call Items.Add to add messages to this list
        ///       as it will make the FilteredItems out of sync
        /// </summary>
        public ObservableCollection<Supplies> Items
        {
            get => mItems;
            set
            {
                // Make sure list has changed
                if (mItems == value)
                    return;

                // Update value
                mItems = value;

                // Update filtered list to match
                FilteredItems = new ObservableCollection<Supplies>(mItems);
            }
        }

        /// <summary>
        /// The chat thread items for the list that include any search filtering
        /// </summary>
        public ObservableCollection<Supplies> FilteredItems { get; set; }





        /// <summary>
        /// True to show the attachment menu, false to hide it
        /// </summary>
        public bool AttachmentMenuVisible { get; set; }

        /// <summary>
        /// True if any popup menus are visible
        /// </summary>
        public bool AnyPopupVisible => AttachmentMenuVisible;

        

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

        /// <summary>
        /// A flag indicating if the search dialog is open
        /// </summary>
        public bool SearchIsOpen
        {
            get => mSearchIsOpen;
            set
            {
                // Check value has changed
                if (mSearchIsOpen == value)
                    return;

                // Update value
                mSearchIsOpen = value;

                // If dialog closes...
                if (!mSearchIsOpen)
                    // Clear search text
                    SearchText = string.Empty;
            }
        }


        #endregion

        #region Public Commands

        /// <summary>
        /// The command for when the attachment button is clicked
        /// </summary>
        public ICommand AttachmentButtonCommand { get; set; }

        /// <summary>
        /// The command for when the area outside of any popup is clicked
        /// </summary>
        public ICommand PopupClickawayCommand { get; set; }

        public ICommand SetClientCommand { get; set; }

        /// <summary>
        /// The command for when the user wants to search
        /// </summary>
        public ICommand SearchCommand { get; set; }

        /// <summary>
        /// The command for when the user wants to open the search dialog
        /// </summary>
        public ICommand OpenSearchCommand { get; set; }

        /// <summary>
        /// The command for when the user wants to close to search dialog
        /// </summary>
        public ICommand CloseSearchCommand { get; set; }

        /// <summary>
        /// The command for when the user wants to clear the search text
        /// </summary>
        public ICommand ClearSearchCommand { get; set; }

        public ICommand NewClientCommand { get; set; }
        public ICommand SetSupplieCommand { get; set; }
        public ICommand ClearCommand { get; set; }

        public Func<Task> SetSupplieAction { get;  set; }
        public Func<Task> AttachmentAction { get;  set; }
        public Func<Task> ClearAction { get;  set; }

        #endregion


        public CustomerFilterSuppViewModel()
        {
            SelectedName = "Choisissez le Fournisseur.."; 
            
            AttachmentButtonCommand = new RelayCommand(async ()=> await AttachmentButton());
            PopupClickawayCommand = new RelayCommand(PopupClickaway);
            SetClientCommand = new RelayCommand(SetClient);

            SearchCommand = new RelayCommand(Search);
            SetSupplieCommand =  new RelayCommand(async () => await SetSupplieAsync());
            ClearCommand =  new RelayCommand(async () => await ClearAsync());

        }

        private async Task ClearAsync()
        {
            await ClearAction();
        }

        private async Task SetSupplieAsync() 
        {
            await SetSupplieAction();
        }

       
        private void SetClient()
        {
            
            //AttachmentMenuVisible = false;
            //SearchIsOpen = false;
            ClearSearch();

        }

        #region Command Methods

        /// <summary>
        /// When the attachment button is clicked show/hide the attachment popup
        /// </summary>
        public async Task AttachmentButton()
        {
           await AttachmentAction();
            // Toggle menu visibility
            //AttachmentMenuVisible ^= true;
            //SearchIsOpen = true;
            //ClearSearch();
            
            //    _= FetchSuppFromDatabase();
               

            
            
        }

        /// <summary>
        /// When the popup clickaway area is clicked hide any popups
        /// </summary>
        public void PopupClickaway()
        {
            // Hide attachment menu
            AttachmentMenuVisible = false;
        }

       

        public async Task FetchSuppFromDatabase()
        {
            if (FirstLoad)
                return;
            IsLoading = true;
            await Task.Delay(2000);
            var result = await IoC.SuppliesManager.GetAllSupplies();

            if (result.Any())
            {
                Items = new ObservableCollection<Supplies>(result);
                IsLoading = false;
                FirstLoad = true;
            }
            else
            {
                IsLoading = false;
                return;
            }

        }

        /// <summary>
        /// Searches the current message list and filters the view
        /// </summary>
        public void Search()
        {
            // Make sure we don't re-search the same text
            if ((string.IsNullOrEmpty(mLastSearchText) && string.IsNullOrEmpty(SearchText)) ||
                string.Equals(mLastSearchText, SearchText))
                return;

            // If we have no search text, or no items
            if (string.IsNullOrEmpty(SearchText) || Items == null || Items.Count <= 0)
            {
                // Make filtered list the same
                FilteredItems = new ObservableCollection<Supplies>(Items);

                // Set last search text
                mLastSearchText = SearchText;

                return;
            }

            // Find all items that contain the given text
            // TODO: Make more efficient search
            FilteredItems = new ObservableCollection<Supplies>(
                Items.Where(item => item.Nom.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));

            // Set last search text
            mLastSearchText = SearchText;
        }

        /// <summary>
        /// Clears the search text
        /// </summary>
        public void ClearSearch()
        {
            // If there is some search text...
            if (!string.IsNullOrEmpty(SearchText))
                // Clear the text
                SearchText = string.Empty;
            // Otherwise...
            else
                // Close search dialog
                SearchIsOpen = false;
        }

        public void HandleSelectedItem(Supplies selectedItem)
        {
            SelectedId = selectedItem.Id;
            SelectedName = selectedItem.Nom;
            SelectedAdresse = selectedItem.Adresse;
            //Supplies = selectedItem;
        }


        #endregion
    }
}
