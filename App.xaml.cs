using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MedicamentStore
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Setup the main application 
            ApplicationSetup();


            // Show the main window
            Current.MainWindow =  new MainWindow();


            Current.MainWindow.Show();
        }

        /// <summary>
        /// Configures our application ready for use
        /// </summary>
        private void ApplicationSetup()
        {
            // Setup IoC
            IoC.Setup();

            QuestPDF.Settings.License = LicenseType.Community;

            // Bind a UI Manager
            // IoC.Kernel.Bind<IUIManager>().ToConstant(new UIManager());
        }
    }
}
