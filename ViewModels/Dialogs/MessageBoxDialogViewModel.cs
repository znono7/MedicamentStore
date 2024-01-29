using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public class MessageBoxDialogViewModel : BaseDialogViewModel
    {
        /// <summary>
        /// The message to display
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The text to use for the OK button
        /// </summary>
        public string OkText { get; set; } = "OK";
    }
}
