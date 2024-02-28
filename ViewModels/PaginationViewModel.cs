using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MedicamentStore
{
    public class PaginationViewModel : BaseViewModel
    {

        public event EventHandler<int> PageIndexChanged;

        private int currentPageIndex = 0;
        public int CurrentPageIndex
        {
            get { return currentPageIndex; }
            set
            {
                if (currentPageIndex != value)
                {
                    currentPageIndex = value;
                    PageIndexChanged?.Invoke(this, currentPageIndex);
                    OnPropertyChanged(nameof(CurrentPageIndex));
                    OnPropertyChanged(nameof(CurrentPage));
                }
            }
        }

        public int CurrentPage
        {
            get { return TotalPages == 0 ? 0 : currentPageIndex + 1; } 
        }

        private int totalPages {  get; set; }
        public int TotalPages
        {
            get { return totalPages; }
            set
            {
                if (totalPages != value)
                {
                    totalPages = value;
                    OnPropertyChanged(nameof(TotalPages));
                }
            }
        }

        public ICommand PreviousPageCommand { get; set; } 
        public ICommand NextPageCommand { get; set; }
         
        public bool btnPreviousIsEnabled => currentPageIndex > 0;
        public bool btnNextIsEnabled => currentPageIndex < totalPages - 1;
        public PaginationViewModel() 
        {
            PreviousPageCommand = new RelayCommand(SubmitPrevious);

            NextPageCommand = new RelayCommand(SubmitNext);
        }

        private void SubmitNext()
        {
            if (CurrentPageIndex < TotalPages - 1)
            {
                CurrentPageIndex++;
            }
        }

        private void SubmitPrevious()
        {
            if (CurrentPageIndex > 0)
            {
                CurrentPageIndex--;
            }
        }

        public void Reset()
        {
            currentPageIndex = 0;
            totalPages = 0;
            
            OnPropertyChanged(nameof(CurrentPageIndex));
            OnPropertyChanged(nameof(TotalPages));
        }
    }
}
