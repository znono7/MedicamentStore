using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MedicamentStore
{
   public class DateFilterViewModel : BaseViewModel
    {
        public ICommand FromDateButtonCommand { get; set; }
        public ICommand ToDateButtonCommand { get; set; }
        public ICommand FilerTypeCommand { get; set; }

        public DateFilterType CurrentDateFilterType = DateFilterType.None;
        public bool AttachmentDateVisible { get;  set; }
        public bool AttachmentToDateVisible { get;  set; }

        public DateTime SelectedFromDate { get; set; }
        public DateTime SelectedToDate { get; set; }


        public DateFilterViewModel()
        {
            SelectedFromDate = SelectedToDate = DateTime.Now;
            FromDateButtonCommand = new RelayCommand(AttachmentButton);
            ToDateButtonCommand = new RelayCommand(AttachmentToDateButton);
            FilerTypeCommand = new RelayParameterizedCommand(async (p) => await SetFilterType(p));
        }

        private async Task SetFilterType(object p)
        {
            if (p is DateFilterType type)
            {
                CurrentDateFilterType = type;
            }
            await Task.Delay(1);
        }

        private void AttachmentToDateButton()
        {
            AttachmentToDateVisible ^= true;

        }

        public void AttachmentButton()
        {
            // Toggle menu visibility
            AttachmentDateVisible ^= true;
        }
        //public int FromDay { get; set; }
        //public int FromMonth { get; set; }
        //public int FromYear { get; set; }
        //public int ToDay { get; set; }
        //public int ToMonth { get; set; }
        //public int ToYear { get; set; }

        //public DateTime GetFromDate()
        //{
        //    return new DateTime(FromYear, FromMonth, FromDay);
        //}

        //public DateTime GetToDate()
        //{
        //    return new DateTime(ToYear, ToMonth, ToDay);
        //}

    }

    public enum DateFilterType
    {
        None = 0,
        Today = 1,
        Yesterday = 2,
        ThisMonth = 3,
        PastMonth = 4,
        Past3Month = 5,
        WithDate = 6,
    }
}
