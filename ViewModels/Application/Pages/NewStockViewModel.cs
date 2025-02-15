﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System;

namespace MedicamentStore
{
    public class NewStockViewModel : BaseViewModel  
    {
        public int ProductCount => StockProducts.Count;
        public List<PharmaceuticalProduct> Products { get; set; }
        public ObservableCollection<NewProduitPharmaStock> StockProducts { get; set; }
        public ObservableCollection<MedicamentUpdateStock> UpdateStockProducts { get; set; }
        public CustomerCmbSuppViewModel SuppCmb { set; get; }
        public TextEntryInvoiceDateViewModel dateViewModel { set; get; }
        public InvoiceInfoViewModel invoice { get; set; } 
         
        public bool AnyPopupVisible => SuppCmb.AttachmentMenuVisible || 
                                       dateViewModel.AttachmentMenuVisible;
                                         
        public ICommand ReturnCommand { get; set; }   
        public ICommand SaveCommand { get; set; } 
        public ICommand DeleteCommand { get; set; } 
        public ICommand OpenWindowCommand { get; set; }
        public ICommand PopupClickawayCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public string TitlePage {  get; set; }
        public bool IsForUpdate {  get; set; }
        public NewStockViewModel(ObservableCollection<MedicamentUpdateStock> stocks)
        {
            UpdateStockProducts = stocks;
            invoice = new InvoiceInfoViewModel();
            ReturnCommand = new RelayCommand(async () => await BackPage());
            OpenWindowCommand = new RelayCommand(OpenWindow);
            SaveCommand = new RelayCommand(async () => await Save());
            PopupClickawayCommand = new RelayCommand(PopupClickaway);
            ClearCommand = new RelayCommand(ClearList);
            DeleteCommand = new RelayParameterizedCommand((p) => DeleteProduit(p));
            SuppCmb = new CustomerCmbSuppViewModel();
            dateViewModel = new TextEntryInvoiceDateViewModel();
            TitlePage = "la Quantité de Mise à Jour";
            IsForUpdate = true;
           
        }
        public NewStockViewModel()
        {
            invoice = new InvoiceInfoViewModel();

            StockProducts = new ObservableCollection<NewProduitPharmaStock>();
            foreach (var item in StockProducts)
            {
                item.IsStock = false;
            }
            ReturnCommand = new RelayCommand(async () => await BackPage());
            OpenWindowCommand = new RelayCommand(OpenWindow);
            SaveCommand = new RelayCommand(async () => await Save());
            PopupClickawayCommand = new RelayCommand(PopupClickaway);
            ClearCommand = new RelayCommand(ClearList);
            DeleteCommand = new RelayParameterizedCommand((p) =>DeleteProduit(p));
            SuppCmb = new CustomerCmbSuppViewModel();
            dateViewModel = new TextEntryInvoiceDateViewModel();
            TitlePage = "Nouvelle Entrée de Stock";
            IsForUpdate = false;
            
        }

        private void ClearList()
        {
            if (StockProducts != null)
            {
                StockProducts.Clear();
               
            }
            if (UpdateStockProducts != null)
            {
                UpdateStockProducts.Clear();
            }
            dateViewModel = new TextEntryInvoiceDateViewModel();
            SuppCmb = new CustomerCmbSuppViewModel();
            invoice = new InvoiceInfoViewModel();
        }

        private void DeleteProduit(object p)
        {
            if (p is NewProduitPharmaStock newProduit)
            {
                StockProducts.Remove(newProduit);
            }
        }

        public void PopupClickaway()
        {
            // Hide attachment menu
            SuppCmb.AttachmentMenuVisible = false;
            dateViewModel.AttachmentMenuVisible = false;
        }

        private async Task Save()
        { 
            if (SuppCmb.SelectedId == 0 )   
            {
                await IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Warning, $"Voulez Choisez le Fournisseur ..."));
                return;
            }
             
           
          
            if (IsForUpdate)
            {
                foreach (var product in UpdateStockProducts)
                {
                    if (product.QuantiteAdded == 0 )
                    {
                        await IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Warning, $"Quantite  de {product.Nom_Commercial} est Vide"));
                        return;
                    }
                }
                foreach (var product in UpdateStockProducts)
                {
                    product.IdSupplie = SuppCmb.SelectedId;
                    product.Date = dateViewModel.SelectedDate.ToShortDateString();

                }
                Invoice invoiceP = new Invoice
                {
                    Date = dateViewModel.SelectedDate,
                    IdSupplie = SuppCmb.SelectedId,
                    InvoiceType = 1,
                    Number = invoice.EnteredNumText,
                    ProduitTotal = UpdateStockProducts.Count,
                    MontantTotal = UpdateStockProducts.Sum(x => x.PrixTotal),
                };
                System.Collections.Generic.List<InvoiceItem> listItems = new System.Collections.Generic.List<InvoiceItem>();
                foreach (var item in UpdateStockProducts)
                {
                    InvoiceItem invoiceItem = new InvoiceItem
                    {
                        IdStock  = item.IdStock,
                        IdMedicament = item.IdMedicament,
                        IdTypeProduct = item.Type,
                        IdUnite = item.IdUnite,
                        InvoiceNumber = invoice.EnteredNumText,
                        Prix = item.Prix,
                        Quantite = item.QuantiteAdded,
                        IdProduct = item.IdProduct,
                    };
                    listItems.Add(invoiceItem);
                }

                var Result = await IoC.StockManager.UpdateStock(UpdateStockProducts.ToList(), invoiceP, listItems); 
                if (Result.Successful) 
                {
                    await IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Succes, $"La Quantité a été Mise à Jour "));
                    dateViewModel = new TextEntryInvoiceDateViewModel();
                    SuppCmb = new CustomerCmbSuppViewModel();
                    UpdateStockProducts.Clear();
                    invoice = new InvoiceInfoViewModel();
                }
                else
                {
                    await IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Error, "Erreur lors de la mise à jour de la quantité"));

                }
            }
            else
            {
                if (StockProducts.Count == 0)
                {
                    return;
                }
                foreach (var product in StockProducts)
                {
                    if (product.Quantite == 0 || product.Prix == 0)
                    {
                        await IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Warning, $"Quantite OU Prix de {product.Nom_Commercial} est Vide"));
                        return;
                    }
                }
                foreach (var product in StockProducts)
                {
                    product.IdSupplie = SuppCmb.SelectedId;
                    product.Date = dateViewModel.SelectedDate.ToShortDateString();

                }
                Invoice invoiceP = new Invoice
                {
                    Date = dateViewModel.SelectedDate,
                    IdSupplie = SuppCmb.SelectedId,
                    InvoiceType = 1,
                    Number = invoice.EnteredNumText,
                    ProduitTotal = StockProducts.Count,
                    MontantTotal = StockProducts.Sum(x => x.PrixTotal),
                };
                System.Collections.Generic.List<InvoiceItem> listItems = new System.Collections.Generic.List<InvoiceItem>();
                foreach (var item in StockProducts)
                {
                    InvoiceItem invoiceItem = new InvoiceItem
                    {
                        IdProduct = item.IdProduct,
                        IdTypeProduct = item.Type,
                        IdUnite = item.SelectedUnite.Id,
                        InvoiceNumber = invoice.EnteredNumText,
                        Prix = item.Prix,
                        Quantite = item.Quantite,
                    };
                    listItems.Add(invoiceItem);
                }

                var Res = await IoC.StockManager.NewStock(StockProducts.ToList(), invoiceP, listItems); 
                if (Res.Successful)
                {
                    await IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Succes, $"Succeé Ajouter "));
                    dateViewModel = new TextEntryInvoiceDateViewModel();
                    SuppCmb = new CustomerCmbSuppViewModel();
                    StockProducts.Clear();
                    invoice = new InvoiceInfoViewModel();
                }
                else
                {
                    await IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Error, $"Erreur Pour Ajoute {Res.ErrorMessage}"));
                    return;
                }
            }

            
        }
        
        private void  OpenWindow()
        {
            NewProduitsPharmaceutiquesViewModel viewModel = new NewProduitsPharmaceutiquesViewModel(ProductCount);
            viewModel.ProduitAded += (sender, e) =>
            {
                if(e.SelectedProductPharma != null)
                {
                    NewProduitPharmaStock newProduit = new NewProduitPharmaStock
                    {
                        Nom_Commercial = e.SelectedProductPharma.Nom_Commercial,
                        Dosage = e.SelectedProductPharma.Dosage,
                        Forme = e.SelectedProductPharma.Forme,
                        Conditionnement = e.SelectedProductPharma.Conditionnement,
                        Type = e.SelectedProductPharma.Type,
                        IdProduct = e.SelectedProductPharma.IdProduct,
                        Img = e.SelectedProductPharma.Img,
                        imageSource = e.SelectedProductPharma.imageSource,

                    };
                   
                    StockProducts.Add(newProduit);
                }
            };
            NewProduitsPharmaceutiques newProduits = new NewProduitsPharmaceutiques(viewModel);
            newProduits.Show();
          
        }
        private async Task BackPage()
        {
          
                
            IoC.Application.GoToPage(ApplicationPage.MainEntreeStockPage);

            
            await Task.Delay(1);
        }
         
      

    }
}
