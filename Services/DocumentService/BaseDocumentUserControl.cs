using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MedicamentStore
{
   public class BaseDocumentUserControl : UserControl
    {
        /// <summary>
        /// The dialog window we will be contained within
        /// </summary>
        private DocumentsHost mDocumentsHost;

        #region Public Commands

        /// <summary>
        /// Closes this dialog
        /// </summary>
        public ICommand CloseCommand { get; private set; }


        #endregion

        #region Public Properties

        /// <summary>
        /// The minimum width of this dialog
        /// </summary>
        public int WindowMinimumWidth { get; set; } = 250;

        /// <summary>
        /// The minimum height of this dialog
        /// </summary>
        public int WindowMinimumHeight { get; set; } = 100;

        /// <summary>
        /// The height of the title bar
        /// </summary>
        public int TitleHeight { get; set; } = 30;

        /// <summary>
        /// The title for this dialog
        /// </summary>
        public string Title { get; set; }



        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BaseDocumentUserControl()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                // Create a new dialog window
                mDocumentsHost = new DocumentsHost();
                mDocumentsHost.ViewModel = new DocumentsHostViewModel(mDocumentsHost);

                // Create close command
                CloseCommand = new RelayCommand(() => mDocumentsHost.Close());


            }
        }



        #endregion


        #region Public Dialog Show Methods

        /// <summary>
        /// Displays a single message box to the user
        /// </summary>
        /// <param name="viewModel">The view model</param>
        /// <typeparam name="T">The view model type for this control</typeparam>
        /// <returns></returns>
        public Task ShowDocument<T>(T viewModel)
            where T : BaseDocumentViewModel
        {
            // Create a task to await the dialog closing
            var tcs = new TaskCompletionSource<bool>();

            // Run on UI thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    // Match controls expected sizes to the dialog windows view model
                    mDocumentsHost.ViewModel.WindowMinimumWidth = WindowMinimumWidth;
                    mDocumentsHost.ViewModel.WindowMinimumHeight = WindowMinimumHeight;
                    mDocumentsHost.ViewModel.TitleHeight = TitleHeight;
                    mDocumentsHost.ViewModel.Title = string.IsNullOrEmpty(viewModel.Title) ? Title : viewModel.Title;

                    // Set this control to the dialog window content
                    mDocumentsHost.ViewModel.Content = this;

                    // Setup this controls data context binding to the view model
                    DataContext = viewModel;

                    // Show dialog
                    mDocumentsHost.ShowDialog();
                }
                finally
                {
                    // Let caller know we finished
                    tcs.TrySetResult(true);
                }
            });

            return tcs.Task;
        }

        #endregion



    }
}
