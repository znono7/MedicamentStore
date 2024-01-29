using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    /// <summary>
    /// Converts the <see cref="MedicamentContent"/> to an actual view/page
    /// </summary>
    public static class MedicamentPageContentConverter
    {
        /// <summary>
        /// Takes a <see cref="MedicamentContent"/> and a view model, if any, and creates the desired page
        /// </summary>
        /// <param name="page"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        /// 
        public static BasePage ToMedicamentBasePage(this MedicamentContent page, object viewModel = null)
        {
            // Find the appropriate page
            switch (page)
            {
                case MedicamentContent.Home:
                    return new MedicamentHome(viewModel as HomeMedicinViewModel);
              
                default:
                    Debugger.Break();
                    return null;
            }
        }

        /// <summary>
        /// Converts a <see cref="BasePage"/> to the specific <see cref="MedicamentContent"/> that is for that type of page
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static MedicamentContent ToMedicamentPage(this BasePage page)
        {
            // Find application page that matches the base page
            if (page is MedicamentHome)
                return MedicamentContent.Home;

           
            // Alert developer of issue
            Debugger.Break();
            return default(MedicamentContent);
        }
    }
}
