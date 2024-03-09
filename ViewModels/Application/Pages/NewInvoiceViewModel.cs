using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using System.Windows.Input;

namespace MedicamentStore
{ 
   public class NewInvoiceViewModel : BaseViewModel
    {

        public ObservableCollection<InvoiceProductDataGrid> InvoiceProducts { get; set; }
        public ObservableCollection<InvoiceItem> InvoiceProductsSets { get; set; }


        public InvoiceInfoViewModel invoice {  get; set; }    
          
        public NotificationBoxViewModel notificationBoxViewModel { get; set; }
        public bool AttachmentNotifVisible { get; set; } 
         
        /// <summary>
        /// True if any popup menus are visible
        /// </summary>
        public bool AnyPopupVisible => 
                                        invoice.AttachmentDateVisible  ; 

        /// <summary>
        /// The command for when the area outside of any popup is clicked
        /// </summary>
        public ICommand PopupClickawayCommand { get; set; }
        public ICommand ReturnCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand NeeItemCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public string QuantityValidationMessage { get; private set; }

        public NewInvoiceViewModel()
        {
           
            invoice = new InvoiceInfoViewModel();
            PopupClickawayCommand = new RelayCommand(PopupClickaway);
            ReturnCommand = new RelayCommand(BackPage);
            SaveCommand = new RelayCommand(async()=>await Save());
            DeleteCommand = new RelayParameterizedCommand((p)=> DeleteItem(p));
            ClearCommand = new RelayCommand(() => InvoiceProducts = new ObservableCollection<InvoiceProductDataGrid>());
            NeeItemCommand = new RelayCommand(ToNewItemWindow);
            InvoiceProductsSets = new ObservableCollection<InvoiceItem>();
            InvoiceProducts = new ObservableCollection<InvoiceProductDataGrid>();
        }

        private void BackPage()
        {
            IoC.Application.GoToPage(ApplicationPage.MainSorteStockPage);       
        }

        public bool ValidateQuantity(double quantity, double restQuantity)
        {
            if (quantity > restQuantity)
            {

                QuantityValidationMessage = "La Quantité ne peut pas être Supérieure à la Quantité Restante.";
                notificationBoxViewModel = new NotificationBoxViewModel(NotificationType.Error, QuantityValidationMessage);
                return false;
            }
            else
            {
                QuantityValidationMessage = string.Empty;
                return true;
            }
        }

        public async Task Save()
        {
            if(InvoiceProducts.Count == 0) 
            {
                await IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Warning, $"Voulez Choisez les Produits..."));
                return;
            }
            InvoiceProductsSets = new ObservableCollection<InvoiceItem>();

            var InvoiceInfo = new Invoice
            {
                Number = invoice.EnteredNumText,
                Date = invoice.SelectedDate,
                ProduitTotal = InvoiceProducts.Count,
                MontantTotal = InvoiceProducts.Sum(x => x.PrixTotal),
                InvoiceType = 2,
                IdSupplie = null,
            };
            foreach (var item in InvoiceProducts)
            {
                var i = new InvoiceItem
                {
                    IdStock = item.IdS,
                    IdMedicament = item.ProductId,
                    InvoiceNumber = invoice.EnteredNumText,
                    Quantite = item.Quantite,
                    Prix = item.Prix,
                    IdTypeProduct = item.TypeId,
                    IdUnite = item.IdUnite
                    
                };
                InvoiceProductsSets.Add(i);
            }
            var ResFinal = await IoC.InvoiceManager.InsertInvoice(InvoiceInfo, InvoiceProductsSets);
            if (ResFinal.Successful)
            {
                AttachmentNotifVisible = true;
                notificationBoxViewModel = new NotificationBoxViewModel(NotificationType.Succes, "Succeé Ajouter");
                await Task.Delay(3000);
                AttachmentNotifVisible = false;
                IoC.Application.GoToPage(ApplicationPage.InvoiceHostPage);
                // await IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Succes, $"Succeé Ajouter "));

            }
            else
            {
                AttachmentNotifVisible = true;

                notificationBoxViewModel = new NotificationBoxViewModel(NotificationType.Error, $"Erreur Pour Ajoute {ResFinal.ErrorMessage}");
                await Task.Delay(3000);
                AttachmentNotifVisible = false;
                // await IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Error, $"Erreur Pour Ajoute {ResFinal.ErrorMessage}"));
                
            }
        }

        private void DeleteItem(object p)
        {
            if (p is InvoiceProductDataGrid item)
            {
                InvoiceProducts.Remove(item);
            }
        }

        private void ToNewItemWindow()
        {
            InvoiceItemsWindowViewModel viewModel = new InvoiceItemsWindowViewModel();
            viewModel.ItemSelected += (sender, e) =>
            {
                
                if(e.SelectedItem != null)
                {
                    if (InvoiceProducts.Where(x => x.IdS == e.SelectedItem.Ids).Any())
                    {
                        return;
                    }
                    
                    InvoiceProducts.Add(ToInvoiceProductDataGrid(e.SelectedItem));
                    

                }
                    
            };
            InvoiceItemsWindow window = new InvoiceItemsWindow(viewModel);
            window.Show();
        }

        public void PopupClickaway()
        {
            // Hide attachment menu
           
            invoice.AttachmentDateVisible = false;
           
        }

        public  InvoiceProductDataGrid ToInvoiceProductDataGrid(MedicamentStock invoiceProduct)
        {
            return new InvoiceProductDataGrid
            {
                IdS = invoiceProduct.Ids,
                Nom_Commercial = invoiceProduct.Nom_Commercial,
                Forme = invoiceProduct.Forme,
                Dosage = invoiceProduct.Dosage,
                QuantiteRest = invoiceProduct.Quantite,
                Prix = invoiceProduct.Prix,
                ProductId = invoiceProduct.IdMedicament,
                Unite = invoiceProduct.Unite,
                TypeId = invoiceProduct.Type,
                IdUnite = invoiceProduct.IdUnite,
            };
        }

        public InvoiceProduct ToInvoiceProduct(InvoiceProductDataGrid invoiceProduct)
        {
            return new InvoiceProduct
            {
                IdS = invoiceProduct.IdS,
                Nom_Commercial = invoiceProduct.Nom_Commercial,
                Forme = invoiceProduct.Forme,
                Dosage = invoiceProduct.Dosage,
                QuantiteRest = invoiceProduct.QuantiteRest,
                Prix = invoiceProduct.Prix,
                ProductId = invoiceProduct.ProductId,
                Quantite = invoiceProduct.Quantite,
                PrixTotal = invoiceProduct.PrixTotal,
                Unite = invoiceProduct.Unite,
                InvoiceNumber = invoiceProduct.InvoiceNumber,
            };
        }
    }
}
