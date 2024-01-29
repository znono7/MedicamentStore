﻿using System;
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
    /// Interaction logic for ProduitStockDoc.xaml
    /// </summary>
    public partial class ProduitStockDoc : BaseDocumentUserControl
    {
        public ProduitStockDoc()
        {
            InitializeComponent();
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WpfPrinting printing = new WpfPrinting();
            PrintDialog printDialog = new PrintDialog();
            DataGrid dataGrid = new DataGrid();
            dataGrid = MyGridData;
            if (printDialog.ShowDialog() == true)
            {
                printing.PrintDataGrid(new TextBlock { Text ="République Algérienne Démocratique et Populaire" }, dataGrid, new TextBlock { Text = "1/1" }, printDialog);
            }
        }
    }
}
