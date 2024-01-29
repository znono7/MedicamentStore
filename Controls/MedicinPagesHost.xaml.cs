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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MedicamentStore
{
    /// <summary>
    /// Interaction logic for MedicinPagesHost.xaml
    /// </summary>
    public partial class MedicinPagesHost : UserControl
    {
        #region Dependency Properties

        /// <summary>
        /// The current page to show in the page host
        /// </summary>
        public MedicamentContent CurrentPage
        {
            get => (MedicamentContent)GetValue(CurrentPageProperty);
            set => SetValue(CurrentPageProperty, value);
        }

        /// <summary>
        /// Registers <see cref="CurrentPage"/> as a dependency property
        /// </summary>
        public static readonly DependencyProperty CurrentPageProperty =
            DependencyProperty.Register(nameof(CurrentPage), typeof(MedicamentContent), typeof(MedicinPagesHost), new UIPropertyMetadata(default(MedicamentContent), null, CurrentPagePropertyChanged));


        /// <summary>
        /// The current page to show in the page host
        /// </summary>
        public BaseViewModel CurrentPageViewModel
        {
            get => (BaseViewModel)GetValue(CurrentPageViewModelProperty);
            set => SetValue(CurrentPageViewModelProperty, value);
        }

        /// <summary>
        /// Registers <see cref="CurrentPageViewModel"/> as a dependency property
        /// </summary>
        public static readonly DependencyProperty CurrentPageViewModelProperty =
            DependencyProperty.Register(nameof(CurrentPageViewModel),
                typeof(BaseViewModel), typeof(MedicinPagesHost),
                new UIPropertyMetadata());

        #endregion
        public MedicinPagesHost()
        {
            InitializeComponent();
        }

        #region Property Changed Events

        /// <summary>
        /// Called when the <see cref="CurrentPage"/> value has changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static object CurrentPagePropertyChanged(DependencyObject d, object value)
        {
            // Get current values
            var currentPage = (MedicamentContent)d.GetValue(CurrentPageProperty);
            var currentPageViewModel = d.GetValue(CurrentPageViewModelProperty);

            // Get the frames
            var newPageFrame = (d as MedicinPagesHost).NewPage;
            var oldPageFrame = (d as MedicinPagesHost).OldPage;

            // If the current page hasn't changed
            // just update the view model
            if (newPageFrame.Content is BasePage page &&
                page.ToMedicamentPage() == currentPage)
            {
                // Just update the view model
                page.ViewModelObject = currentPageViewModel;

                return value;
            }

            // Store the current page content as the old page
            var oldPageContent = newPageFrame.Content;

            // Remove current page from new page frame
            newPageFrame.Content = null;

            // Move the previous page into the old page frame
            oldPageFrame.Content = oldPageContent;

            // Animate out previous page when the Loaded event fires
            // right after this call due to moving frames
            if (oldPageContent is BasePage oldPage)
            {
                // Tell old page to animate out
                oldPage.ShouldAnimateOut = true;

                // Once it is done, remove it
                Task.Delay((int)(oldPage.SlideSeconds * 1000)).ContinueWith((t) =>
                {
                    // Remove old page
                    Application.Current.Dispatcher.Invoke(() => oldPageFrame.Content = null);
                });
            }

            // Set the new page content
            newPageFrame.Content = currentPage.ToMedicamentBasePage(currentPageViewModel);

            return value;
        }

        #endregion

    }
}
