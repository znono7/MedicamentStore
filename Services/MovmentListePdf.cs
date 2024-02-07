using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MedicamentStore
{
    public class MovmentListeDocument : IDocument
    {
        public MovmentListeDocument(ObservableCollection<TransactionDto> transactions)
        {
            Transactions = transactions;
           
            
        }

        public ObservableCollection<TransactionDto> Transactions { get; }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;

       
        public void Compose(IDocumentContainer container)
        {
            var footerStyle = TextStyle.Default.FontSize(10).NormalWeight().FontFamily(Fonts.TimesNewRoman).FontColor(Colors.Black);

            container
                .Page(page =>
                {
                    
                    page.Size(PageSizes.A4);
                    page.Margin(1.5f , Unit.Centimetre);
                    page.PageColor(Colors.White);

                    page.Header().Element(ComposeHeader);
                   

                    page.Footer().Height(20).AlignLeft().AlignMiddle().DefaultTextStyle(footerStyle).Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages(); 
                        
                    });
                }
                );
        }

        private void ComposeHeader(IContainer container)
        {
          

            var titleStyle = TextStyle.Default.FontSize(14).SemiBold().FontFamily(Fonts.TimesNewRoman).FontColor(Colors.Black).Medium().LineHeight(1f);
            var titleStyle2 = TextStyle.Default.FontSize(14).SemiBold().FontFamily(Fonts.TimesNewRoman).FontColor(Colors.Black).Medium().LineHeight(1f);

            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    
                    column.Item().AlignCenter().Text("République Algérienne Démocratique et Populaire").Style(titleStyle);
                    column.Item().AlignCenter().Text("Ministère de la Santé").Style(titleStyle);
                    column.Item().AlignCenter().Text("Direction de la Santé et de la Population DE LAGHOUAT").Style(titleStyle);
                    column.Spacing(0.4f , Unit.Centimetre);
                    column.Item().AlignLeft().Text("Etablissement Public de Santé de  Proximité à Aflou").Style(titleStyle);
                    column.Item().AlignLeft().Text("Service de Pharmacie").Style(titleStyle);
                    column.Item().AlignCenter().Text("Liste des mouvements de Produits Pharmaceutiques").Style(titleStyle);
                    column.Item().AlignCenter().Text(SetDate()).Style(titleStyle);

                });
            });
        }

        private string SetDate()
        {
            DateTime maxDate = Transactions.Max(item => item.Date);
            DateTime minDate = Transactions.Min(item => item.Date);
            return $"[{minDate.ToShortDateString()}] - [{maxDate.ToShortDateString()}]";
        }
    }
}
