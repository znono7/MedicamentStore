
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;


namespace MedicamentStore
{
    public class DocumentsHostReportViewModel : MainWindowViewModel
    {
        /// <summary>
        /// The title of this dialog window
        /// </summary> 
        public string? Title { get; set; } 

        /// <summary>
        /// The content to host inside the dialog 
        /// </summary>
        public Control? Content { get; set; }

        public ICommand PrintCommand { get; set; }
        public DocumentsHostReportViewModel(Window window) : base(window)
        {
            // Make minimum size smaller
            WindowMinimumWidth = 250;
            WindowMinimumHeight = 100;

            // Make title bar smaller
            TitleHeight = 30;

            PrintCommand = new RelayCommand(PrintDocument);
        }

        private void PrintDocument()
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                IDocumentPaginatorSource document = (Content as ProduitStockFlowDoc).MyFlowDoc;
                printDialog.PrintDocument(document.DocumentPaginator, "StockEtat");
            }
        }
    }
}
