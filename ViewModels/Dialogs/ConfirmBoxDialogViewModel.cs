using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MedicamentStore
{
    public class ConfirmBoxDialogViewModel : BaseDialogViewModel
    {
        /// <summary>
        /// The message to display
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// The text to use for the OK button
        /// </summary>
        public string YesText { get; set; } = "Oui";

        /// <summary>
        /// The text to use for the OK button
        /// </summary>
        public string NoText { get; set; } = "Non";

        /// <summary>
        /// Confirm the message
        /// </summary>
        public bool IsConfirmed { get; set; }

        /// <summary>
        /// The command to Confirm
        /// </summary>
        public ICommand ConfirmCommand { get; set; }

        public ConfirmBoxDialogViewModel()
        {
            ConfirmCommand = new RelayCommand(async () => await ConfirmAsync());
        }

        public async Task ConfirmAsync()
        {
            IsConfirmed = true;
            await Task.Delay(1);
        }
    }
}
