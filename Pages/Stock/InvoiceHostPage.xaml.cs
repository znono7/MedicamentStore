using System;
using System.Collections;
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
    /// Interaction logic for StockHostPage.xaml
    /// </summary>
    public partial class InvoiceHostPage : BasePage<InvoiceHostViewModel> 
    {
        public InvoiceHostPage()
        {
            InitializeComponent();
        } 
        public InvoiceHostPage(InvoiceHostViewModel viewModel) : base(viewModel) 
        {
            InitializeComponent();
        }

       
       

       

       
    }
}
