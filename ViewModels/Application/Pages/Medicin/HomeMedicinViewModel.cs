using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MedicamentStore 
{ 
    public class HomeMedicinViewModel : BaseViewModel
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
        protected string mSearchdciText;

        protected ObservableCollection<MedicinCommercial> mMedicins { get; set; }
        protected ObservableCollection<MedicinDCI> mMedicinsdci { get; set; }

        protected MedicinCommercial _selectedMedicin;

        #endregion

        #region Public Members

        /// <summary>
        /// The chat thread items for the list that include any search filtering
        /// </summary>
        public ObservableCollection<MedicinCommercial> FilteredItems { get; set; }
        public ObservableCollection<MedicinCommercial>? Medicins
        { get => mMedicins;
            set
            {
                // Make sure list has changed
                if (mMedicins == value)
                    return;

                // Update value
                mMedicins = value;

                // Update filtered list to match
                FilteredItems = new ObservableCollection<MedicinCommercial>(mMedicins);
               
            }
        }

        public ObservableCollection<MedicinDCI> FilteredDCI { get; set; }
        public ObservableCollection<MedicinDCI>? MedicinsDCI
        {
            get => mMedicinsdci;
            set
            {
                // Make sure list has changed
                if (mMedicinsdci == value)
                    return;

                // Update value
                mMedicinsdci = value;

                // Update filtered list to match
                FilteredDCI = new ObservableCollection<MedicinDCI>(mMedicinsdci);

            }
        }

        /// <summary>
        /// True to show the attachment menu, false to hide it
        /// </summary>
        public bool AttachmentMenuVisible { get; set; } 
        public bool AttachmentMenuVisible2 { get; set; }

        public MedicinCommercial SelectedMedicin
        {
            get { return _selectedMedicin; }
            set
            {
                _selectedMedicin = value;
                // Notify property change to update the UI
                OnPropertyChanged(nameof(SelectedMedicin));
            }
        }
        public bool IsLoading { get; set; }

        /// <summary>
        /// The text to search for when we do a search
        /// </summary>
        public string SearchText
        {
            get => mSearchText;
            set
            {
               
                AttachmentMenuVisible = true;
                AttachmentMenuVisible2 = false;
                // Update value
                mSearchText = value;
                IsLoading = true;

                if (string.IsNullOrEmpty(SearchText))
                {
                    Medicins = new ObservableCollection<MedicinCommercial>();
                   IsLoading = false;
                    return;
                }

                _ = Search(); 
            }
        }
        public string SearchDCIText
        {
            get => mSearchdciText;
            set
            {

                AttachmentMenuVisible2 = true;
                AttachmentMenuVisible = false;

                // Update value
                mSearchdciText = value;
                IsLoading = true;

                if (string.IsNullOrEmpty(SearchDCIText))
                {
                    MedicinsDCI = new ObservableCollection<MedicinDCI>();
                    IsLoading = false;
                    return;
                }

                _ = SearchDCI();
            }
        }

        private async Task SearchDCI()
        {
            await Task.Delay(2000).ConfigureAwait(false);
            var Result = await IoC.MedicamentManager.GetDCI(SearchDCIText).ConfigureAwait(false);
            if (Result == null)
            {
                IsLoading = false;
                return;
            }
            MedicinsDCI = new ObservableCollection<MedicinDCI>(Result);


            IsLoading = false;
        }

        public ICommand PopupClickawayCommand { get; set; }

        #endregion
        public HomeMedicinViewModel()
        {
            PopupClickawayCommand = new RelayCommand(CloseMenu);
        }

        private void CloseMenu()
        {
            AttachmentMenuVisible = false;
            AttachmentMenuVisible2 = false;
        }

        #region Public Methodes
        /// <summary>
        /// Searches the current message list and filters the view
        /// </summary>
        public async Task Search()
        {
           

            await Task.Delay(2000).ConfigureAwait(false);
            var Result = await IoC.MedicamentManager.GetNomSearch(SearchText).ConfigureAwait(false);
            if (Result == null)
            {
                IsLoading = false;
                return;
            }
            Medicins = new ObservableCollection<MedicinCommercial>(Result);
          

            IsLoading = false;
           
            
          
        }
        #endregion
    }
}
