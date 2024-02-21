using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MedicamentStore
{
    public class AddStockWindowViewModel : BaseViewModel
    {
     
        public CustomerCmbSuppViewModel SuppCmb {  get; set; }
        public TextEntryInvoiceDateViewModel dateViewModel { set; get; }

        public event EventHandler<UpdateQuantiteProduitEventArgs> UpdateQuantiteProduit;
        public ICommand UpdateQuantiteCommand { get; set; }
        public int Id { get; set; }           
        public string? Img { get; set; }
        public string? Nom_Commercial { get; set; }
        public string? Forme { get; set; }
        public string? Dosage { get; set; }
        public string? Conditionnement { get; set; }

        public int Quantite { get; set; }

        public ICommand PlusCommand { get; set; }
        public ICommand MinusCommand { get; set; }
        public ICommand SaveCommand { get; set; }

        
        public MedicamentStock MedicamentStock { get; set; }
        public NotificationBoxViewModel NotificationBoxViewModel { get; set; }

        public bool AttachmentNotifVisible {  get; set; }
        public AddStockWindowViewModel(MedicamentStock medicamentStock )
        {
            SuppCmb = new CustomerCmbSuppViewModel();
            dateViewModel = new TextEntryInvoiceDateViewModel
            {
                width = 200
            };
            PlusCommand = new RelayCommand(PlusQuantite);
            MinusCommand = new RelayCommand(MinusQuantite);
            UpdateQuantiteCommand = new RelayCommand(async()=> await UpdateQ());
            MedicamentStock = new MedicamentStock();
            MedicamentStock = medicamentStock;
           
            Img = medicamentStock.Img;
            Nom_Commercial = medicamentStock.Nom_Commercial;
            Forme = medicamentStock.Forme;
            Dosage = medicamentStock.Dosage;
            Conditionnement = medicamentStock.Conditionnement;

          
        }

        public async Task UpdateQ()
        {
            if (SuppCmb.SelectedId == 0)
            { 
                AttachmentNotifVisible = true;
                NotificationBoxViewModel = new NotificationBoxViewModel(NotificationType.Warning," Voulez Choisez le Fournisseur...");
                await Task.Delay(1500);
                AttachmentNotifVisible = false;
                return;
            }
            var Result = await IoC.StockManager.AddStockEnterAsync(new StockEnter
            {
                IdStock = MedicamentStock.Ids,
                IdSupplie = SuppCmb.SelectedId,
                Quantite = MedicamentStock.Quantite,
                QuantiteAdded = Quantite,
                Date = dateViewModel.SelectedDate
            });
            if (Result.Successful)
            {
                AttachmentNotifVisible = true;
                
                NotificationBoxViewModel = new NotificationBoxViewModel(NotificationType.Succes, "La Quantité a été Mise à Jour");
                await Task.Delay(1500);
                AttachmentNotifVisible = false;
                UpdateQuantiteProduit.Invoke(this, new UpdateQuantiteProduitEventArgs { UpdateQuantiteStock = MedicamentStock });
                
            }
            else
            {
                AttachmentNotifVisible = true;
                await Task.Delay(1500);
                NotificationBoxViewModel = new NotificationBoxViewModel(NotificationType.Error, "Erreur lors de la mise à jour de la quantité");
                AttachmentNotifVisible = false;
            }


        }

        private void MinusQuantite()
        {
            if (Quantite == 0)
            {
                return;
            }
            Quantite--;
        }

        private void PlusQuantite()
        {
            Quantite++;
        }

    }
}
