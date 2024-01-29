using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    /// <summary>
    /// Converts the <see cref="ApplicationPage"/> to an actual view/page
    /// </summary>
    public static class ApplicationPageHelpers
    {
        /// <summary>
        /// Takes a <see cref="ApplicationPage"/> and a view model, if any, and creates the desired page
        /// </summary>
        /// <param name="page"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        /// 
        public static BasePage ToBasePage(this ApplicationPage page, object viewModel = null)
        {
            // Find the appropriate page
            switch (page)
            {
                case ApplicationPage.Login:
                    return new LoginPage(viewModel as LoginViewModel);
                case ApplicationPage.Users:
                    return new UsersPage(viewModel as UsersViewModel);
                case ApplicationPage.NewUser:
                    return new NewUser(viewModel as NewUserViewModel);
                case ApplicationPage.Paremetres:
                    return new SettingsPage(viewModel as ParemetresViewModel);

                case ApplicationPage.Home:
                    return new HomePage(viewModel as HomeViewModel);
                case ApplicationPage.Medicament:
                    return new MedicamentPage(viewModel as MedicamentViewModel);
                case ApplicationPage.NewInvoice:
                    return new NewInvoice(viewModel as NewInvoiceViewModel);
                case ApplicationPage.StockPage:
                    return new StockPage(viewModel as StockViewModel);
                case ApplicationPage.NewStockPage:
                    return new NewStockPage(viewModel as NewStockViewModel);
                case ApplicationPage.StockHostPage:
                    return new StockHostPage(viewModel as StockHostViewModel);
                case ApplicationPage.InvoiceHostPage:
                    return new InvoiceHostPage(viewModel as InvoiceHostViewModel);

                default:
                    Debugger.Break();
                    return null;
            }
        }

        /// <summary>
        /// Converts a <see cref="BasePage"/> to the specific <see cref="ApplicationPage"/> that is for that type of page
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public static ApplicationPage ToApplicationPage(this BasePage page)
        {
            // Find application page that matches the base page
            if (page is HomePage)
                return ApplicationPage.Home;

            if (page is LoginPage)
                return ApplicationPage.Login;

            if (page is UsersPage)
                return ApplicationPage.Users;
            
            if (page is NewUser)
                return ApplicationPage.NewUser;

            if (page is SettingsPage)
                return ApplicationPage.Paremetres;

            if (page is MedicamentPage)
                return ApplicationPage.Medicament; 

            if (page is NewInvoice)
                return ApplicationPage.NewInvoice;

            if (page is StockPage)
                return ApplicationPage.StockPage;
            if (page is NewStockPage)
                return ApplicationPage.NewStockPage;
            if (page is StockHostPage)
                return ApplicationPage.StockHostPage;
            if (page is InvoiceHostPage)
                return ApplicationPage.InvoiceHostPage;
            // Alert developer of issue
            Debugger.Break();
            return default(ApplicationPage);
        }
    }
}
