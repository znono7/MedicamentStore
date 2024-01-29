using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MedicamentStore
{
    public class DialogWindowViewModel : MainWindowViewModel
    {
        /// <summary> 
        /// The title of this dialog window
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// The content to host inside the dialog
        /// </summary>
        public Control? Content { get; set; }
        public DialogWindowViewModel(Window window) : base(window)
        {
            // Make minimum size smaller
            WindowMinimumWidth = 250;
            WindowMinimumHeight = 100;

            // Make title bar smaller
            TitleHeight = 30;
        }
    }
}
