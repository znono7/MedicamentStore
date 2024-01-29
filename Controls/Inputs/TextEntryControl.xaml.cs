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
    /// Interaction logic for TextEntryControl.xaml
    /// </summary>
    public partial class TextEntryControl : UserControl
    {
        public TextEntryControl()
        {
            InitializeComponent();
            DataContext = new TextEntryViewModel();
        }

        private void EntryData_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Define the maximum number of characters allowed in the TextBox
            int maxLength = 250; // Change this value to your desired maximum length

            // Check if the length of the text in the TextBox is greater than or equal to the maximum length
            if (textBox.Text.Length >= maxLength)
            {
                // If it exceeds the maximum length, set e.Handled to true to prevent further input
                e.Handled = true;
            }
        }
    }
}
