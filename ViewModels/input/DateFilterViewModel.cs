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
        public bool AttachmentDateVisible { get;  set; }
        public bool AttachmentToDateVisible { get;  set; }

        public DateTime SelectedFromDate { get; set; }
        public DateTime SelectedToDate { get; set; }


        public DateFilterViewModel()
        {
            SelectedFromDate = SelectedToDate = DateTime.Now;
            FromDateButtonCommand = new RelayCommand(AttachmentButton);
            ToDateButtonCommand = new RelayCommand(AttachmentToDateButton);
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
}
