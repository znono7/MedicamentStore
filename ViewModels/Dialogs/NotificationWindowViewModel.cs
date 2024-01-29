using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace MedicamentStore
{
    public class NotificationWindowViewModel : MainWindowViewModel
    {
        /// <summary>
        /// The title of this dialog window
        /// </summary> 
        public string? Title { get; set; } = "Notification";

        /// <summary>
        /// The content to host inside the dialog
        /// </summary>
        public Control? Content { get; set; }

        public Window Window { get; set; }

        private DispatcherTimer _closeTimer;

        public bool AttachmentNotifVisible { get; set; }
        public NotificationWindowViewModel(Window window) : base(window)
        {
            Window = window;
            // Make minimum size smaller
            WindowMinimumWidth = 400;
            WindowMinimumHeight = 100;

            // Make title bar smaller
            TitleHeight = 0;
            AttachmentNotifVisible = true;
            //window.Top = SystemParameters.WorkArea.Top + 100;
            //window.Left = (SystemParameters.WorkArea.Right - window.Width) / 2; // Centered horizontally

            _closeTimer = new DispatcherTimer();
            _closeTimer.Interval = TimeSpan.FromSeconds(2); // Adjust the time interval as needed
            _closeTimer.Tick += CloseTimer_Tick;
            _closeTimer.Start();
        }

        private void CloseTimer_Tick(object? sender, EventArgs e)
        {
            _closeTimer.Stop();

            // Use the application's Dispatcher to close the window on the UI thread
            Application.Current.Dispatcher.Invoke(() =>
            {
                AttachmentNotifVisible = false;
                // Close the window
                CloseDialog();
            });
        }

        private void CloseDialog()
        {
            Window.Close();
        }
    }
    }

