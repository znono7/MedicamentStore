using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Documents;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MedicamentStore
{
    public class MovmentListeDocument : IDocument
    {
        public MovmentListeDocument(ObservableCollection<MouvementStocks> transactions)
        {
            Transactions = transactions;
           
            
        }

        public ObservableCollection<MouvementStocks> Transactions { get; }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;

       
        public void Compose(IDocumentContainer container)
        {
            var footerStyle = TextStyle.Default.FontSize(10).NormalWeight().FontFamily(Fonts.TimesNewRoman).FontColor(Colors.Black);

            container
                .Page(page =>
                {
                    
                    page.Size(PageSizes.A4);
                    
                    page.Margin(1f , Unit.Centimetre);
                    page.PageColor(Colors.White);

                    page.Header().ShowOnce().Element(ComposeHeader);
                    page.Content().Element(ComposeContent);

                    page.Footer().Height(20).AlignLeft().AlignMiddle().DefaultTextStyle(footerStyle).Text(x =>
                    {
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages(); 
                        
                    });
                }
                );
        }

        private void ComposeContent(IContainer container)
        {
            container.PaddingVertical(40).Column(column =>
            {
                column.Spacing(5);

                column.Item().Element(ComposeTable);

                
            });
        }

        private void ComposeTable(IContainer container)
        {

            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(15);
                    columns.ConstantColumn(150);
                    columns.RelativeColumn();
                    columns.ConstantColumn(50);
                    columns.ConstantColumn(50);
                    columns.ConstantColumn(50);
                    columns.ConstantColumn(32);
                    columns.ConstantColumn(50);
                    columns.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("#");
                    header.Cell().Element(CellStyle).Text($"Produit{Environment.NewLine}Pharmaceutique");
                    header.Cell().Element(CellStyle).Text("Type");

                    header.Cell().Element(CellStyle).Text("Entrée");
                    header.Cell().Element(CellStyle).Text("Sortie");
                    header.Cell().Element(CellStyle).Text($"Stock{Environment.NewLine}Initial");
                    header.Cell().Element(CellStyle).Text("Unité");
                   // header.Cell().Element(CellStyle).Text("Prix");
                   // header.Cell().Element(CellStyle).Text("Prix Total");
                    header.Cell().Element(CellStyle).Text("à la Date");
                    header.Cell().Element(CellStyle).Text("Fournisseur");

                    static IContainer CellStyle(IContainer container)
                    {
                        var headerStyle = TextStyle.Default.FontSize(7).SemiBold().FontFamily(Fonts.Lato).FontColor(Colors.Black);

                        return container.DefaultTextStyle(headerStyle).PaddingVertical(2).Border(0.5f).BorderColor(Colors.Black).AlignCenter().AlignMiddle();
                    }
                });

                foreach (var item in Transactions)
                {
                    table.Cell().Element(CellStyle).Text($"{Transactions.IndexOf(item) + 1}");
                    table.Cell().Element(CellStyleNomC).Text($"{item.Nom_Commercial} {item.Dosage}{Environment.NewLine}{item.Forme}");
                    table.Cell().Element(CellStyle).Text($"{item.TypeMed}");

                    table.Cell().Element(CellStyle).AlignCenter().Text($"{item.StockIn}");
                    table.Cell().Element(CellStyle).AlignCenter().Text($"{item.StockOut}");
                    table.Cell().Element(CellStyle).AlignCenter().Text($"{item.PreviousQuantity}");
                    table.Cell().Element(CellStyle).AlignLeft().Text($"{item.Unite}");
                   // table.Cell().Element(CellStyle).Text($"{(item.Prix * item.QuantiteTransaction).ToString("N2", CultureInfo.CurrentCulture)} DA");
                    table.Cell().Element(CellStyle).Text($"{item.Date.ToString("dd/MM/yyyy")}");
                    table.Cell().Element(CellStyle).Text($"{item.Supplie}");

                    static IContainer CellStyle(IContainer container)
                    {
                        var cellStyle = TextStyle.Default.FontSize(7).NormalWeight().FontFamily(Fonts.Calibri).FontColor(Colors.Black);

                        return container.DefaultTextStyle(cellStyle).Border(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).AlignCenter().AlignMiddle();
                    }
                    static IContainer CellStyleNomC(IContainer container)
                    {
                        var cellStyle = TextStyle.Default.FontSize(7).NormalWeight().FontFamily(Fonts.Calibri).FontColor(Colors.Black);

                        return container.DefaultTextStyle(cellStyle).Border(0.5f).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).AlignLeft().AlignMiddle();
                    }
                }



            });
           
        }

        private void ComposeHeader(IContainer container)
        {


            var titleStyle = TextStyle.Default.FontSize(12).SemiBold().FontFamily(Fonts.TimesNewRoman).FontColor(Colors.Black).Medium();
            var titleStyle2 = TextStyle.Default.FontSize(14).SemiBold().FontFamily(Fonts.TimesNewRoman).FontColor(Colors.Black).Medium().LineHeight(1f);

            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    
                    column.Item().AlignCenter().Text("République Algérienne Démocratique et Populaire").Style(titleStyle);
                    column.Item().AlignCenter().Text("Ministère de la Santé").Style(titleStyle);
                    column.Item().AlignCenter().Text("Direction de la Santé et de la Population DE LAGHOUAT").Style(titleStyle);
                    column.Item().AlignCenter().Text("").Style(titleStyle);
                    
                    column.Item().AlignLeft().Text("Etablissement Public de Santé de  Proximité à Aflou").Style(titleStyle);
                    column.Item().AlignLeft().Text("Service de Pharmacie").Style(titleStyle);
                    column.Item().AlignRight().Text($"Dans: {DateTime.Today.ToString("dd/MM/yyyy")}").Style(titleStyle);


                    column.Item().AlignCenter().Text("Fiche de Stock").Style(titleStyle);
                    column.Item().AlignCenter().Text(SetDate()).Style(titleStyle);

                });
            });
        }

        private string SetDate()
        {
            DateTime maxDate = Transactions.Max(item => item.Date);
            DateTime minDate = Transactions.Min(item => item.Date);
            return $"[{minDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}] - [{maxDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)}]";
        }
    }
}
