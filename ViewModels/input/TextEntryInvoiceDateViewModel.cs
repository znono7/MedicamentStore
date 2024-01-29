using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MedicamentStore
{
    public class TextEntryInvoiceDateViewModel : BaseViewModel
    {

        /// <summary>
        /// The label to identify what this value is for
        /// </summary>
        public string Label { get; set; } = "Date";

        /// <summary>
        /// The current Entered text
        /// </summary>
        public string? EnteredText { get; set; }

        public double height { get; set; } = 32;
        public double width { get; set; } = 240;

        public DateTime SelectedDate {  get; set; }

        /// <summary>
        /// True to show the attachment menu, false to hide it
        /// </summary>
        public bool AttachmentMenuVisible { get; set; }

        /// <summary>
        /// The command for when the attachment button is clicked
        /// </summary>
        public ICommand AttachmentButtonCommand { get; set; }


        public TextEntryInvoiceDateViewModel()
        {
            SelectedDate = DateTime.Today;
            AttachmentButtonCommand = new RelayCommand( AttachmentButton);

        }


        /// <summary>
        /// When the attachment button is clicked show/hide the attachment popup
        /// </summary>
        public void AttachmentButton()
        {
            
            // Toggle menu visibility
            AttachmentMenuVisible ^= true;
           
        }
        }
}
