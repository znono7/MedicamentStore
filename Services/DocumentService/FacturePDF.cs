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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MedicamentStore
{
    public class FactureDocument : IDocument
    {
        public FactureDocument(List<InvoiceItemDto> transactions , Invoice invoice)
        {
            InvoiceItems = transactions;
            Invoice = invoice;
        }

        public List<InvoiceItemDto> InvoiceItems { get; }
        public Invoice Invoice { get; }

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
                column.Item().AlignLeft().Text($"Fournisseur: {Invoice.NomSupplie}").FontSize(10);

                column.Item().Element(ComposeTable);

                var totalPrice = InvoiceItems.Sum(x => x.PrixTotal);
                column.Item().AlignRight().Text($"Montant Total: {totalPrice.ToString("N2", CultureInfo.CurrentCulture)} DA").FontSize(10);
                column.Item().AlignRight().Text($"Nombre de Produit: {Invoice.ProduitTotal}").FontSize(10);

            });
        }

        private void ComposeTable(IContainer container)
        {

            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(20);
                    columns.ConstantColumn(160);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("#");
                    header.Cell().Element(CellStyle).Text("Produit Pharmaceutique");
                    header.Cell().Element(CellStyle).Text("Type");
                    header.Cell().Element(CellStyle).Text("Quantité");
                    header.Cell().Element(CellStyle).Text("Unité");
                    header.Cell().Element(CellStyle).Text("Prix");
                    header.Cell().Element(CellStyle).Text("Prix Total");
                    

                    static IContainer CellStyle(IContainer container)
                    {
                        var headerStyle = TextStyle.Default.FontSize(10).SemiBold().FontFamily(Fonts.Tahoma).FontColor(Colors.Black);

                        return container.DefaultTextStyle(headerStyle).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });

                foreach (var item in InvoiceItems)
                {
                    table.Cell().Element(CellStyle).Text($"{InvoiceItems.IndexOf(item) + 1}");
                    table.Cell().Element(CellStyle).Text($"{item.Nom_Commercial} {item.Dosage}{Environment.NewLine}{item.Forme}");
                    table.Cell().Element(CellStyle).Text($"{item.TypeProduct}");
                    table.Cell().Element(CellStyle).AlignCenter().Text($"{item.Quantite}");
                    table.Cell().Element(CellStyle).AlignLeft().Text($"{item.Unite}");
                    table.Cell().Element(CellStyle).Text($"{item.Prix.ToString("N2", CultureInfo.CurrentCulture)} DA");
                    table.Cell().Element(CellStyle).Text($"{(item.Prix * item.Quantite).ToString("N2", CultureInfo.CurrentCulture)} DA");
                    

                    static IContainer CellStyle(IContainer container)
                    {
                        var cellStyle = TextStyle.Default.FontSize(9).NormalWeight().FontFamily(Fonts.Arial).FontColor(Colors.Black);

                        return container.DefaultTextStyle(cellStyle).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                    }
                   
                }



            });
           
        }

        private void ComposeHeader(IContainer container)
        {
          
            string s1 ="Facturation des Entrées de Stockage";
            string s2 = "Facturation des Sortie de Stockage";
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


                    column.Item().AlignCenter().Text(  Invoice.InvoiceType == 1 ? s1 : s2 ).Style(titleStyle);

                });
            });
        }

       
    }
}
