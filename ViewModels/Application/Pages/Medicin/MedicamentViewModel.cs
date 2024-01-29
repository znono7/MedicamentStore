using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public class MedicamentViewModel : BaseViewModel
    {
        #region Public Properties
        /// <summary>
        /// Content of Medicin Page
        /// </summary>
        public MedicamentContent medicamentContent { get; set; } = MedicamentContent.Home;

        /// <summary>
        /// The view model to use for the current page when the CurrentPage changes
        /// NOTE: This is not a live up-to-date view model of the current page
        ///       it is simply used to set the view model of the current page 
        ///       at the time it changes
        /// </summary>
        public BaseViewModel CurrentPageViewModel { get; set; }

        /// <summary>
        /// Visibility of medicin menu
        /// </summary>
        public bool MedicinMenu { get; set; }
        #endregion
    }
}
