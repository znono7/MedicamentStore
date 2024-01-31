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
    /// Interaction logic for DateFilterControl.xaml
    /// </summary>
    public partial class DateFilterControl : UserControl
    {
        public DateFilterControl()
        {
            InitializeComponent();
            
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true; // Ignore non-numeric input
            }

            TextBox textBox = (TextBox)sender;

            // Check if the length of the text is already 2
            if (textBox.Text.Length >= 2 && !char.IsControl(e.Text[0]))
            {
                e.Handled = true; // Limit the length to 2 numeric characters
            }
        }

        private void TextBox_PreviewTextInput_1(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true; // Ignore non-numeric input
            }

            TextBox textBox = (TextBox)sender;

            // Check if the length of the text is already 4
            if (textBox.Text.Length >= 4 && !char.IsControl(e.Text[0]))
            {
                e.Handled = true; // Limit the length to 4 numeric characters
            }
        }
    }
}
