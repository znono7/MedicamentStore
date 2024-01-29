using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MedicamentStore
{
    /// <summary>
    /// Interaction logic for DialogWindow.xaml
    /// </summary>
    public partial class NotificationWindow : Window
    {
        #region Private Members

        /// <summary>
        /// The view model for this window
        /// </summary>
        private NotificationWindowViewModel mViewModel;

        #endregion

        #region Public Properties

        /// <summary>
        /// The view model for this window
        /// </summary>
        public NotificationWindowViewModel ViewModel
        {
            get => mViewModel;
            set
            {
                // Set new value
                mViewModel = value;

                // Update data context
                DataContext = mViewModel;
            }
        }

        #endregion
        public NotificationWindow()
        {
            InitializeComponent();
        }
    }
}
