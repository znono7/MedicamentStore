using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MedicamentStore
{
    public class NewStockViewModel : BaseViewModel  
    {
        public ObservableCollection<NewProduitPharmaStock> StockProducts { get; set; }
        public CustomerCmbSuppViewModel SuppCmb { set; get; }
        public TextEntryInvoiceDateViewModel dateViewModel { set; get; } 

        public bool AnyPopupVisible => SuppCmb.AttachmentMenuVisible ||
                                       dateViewModel.AttachmentMenuVisible;
                                        
        public ICommand ReturnCommand { get; set; }  
        public ICommand SaveCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand OpenWindowCommand { get; set; }
        public ICommand PopupClickawayCommand { get; set; }
        public ICommand ClearCommand { get; set; }


        public NewStockViewModel()
        {
            StockProducts = new ObservableCollection<NewProduitPharmaStock>();
            ReturnCommand = new RelayCommand(async () => await BackPage());
            OpenWindowCommand = new RelayCommand(OpenWindow);
            SaveCommand = new RelayCommand(async () => await Save());
            PopupClickawayCommand = new RelayCommand(PopupClickaway);
            ClearCommand = new RelayCommand(ClearList);
            DeleteCommand = new RelayParameterizedCommand((p) =>DeleteProduit(p));
            SuppCmb = new CustomerCmbSuppViewModel();
            dateViewModel = new TextEntryInvoiceDateViewModel();

            
        }

        private void ClearList()
        {
            if (StockProducts != null)
            {
                StockProducts.Clear();
            }
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
            foreach(var product in StockProducts)
            {
                product.IdSupplie = SuppCmb.SelectedId;
                product.Date = dateViewModel.SelectedDate.ToShortDateString();
               
            }
            var Res = await IoC.StockManager.AddNewStockAsync(StockProducts);
            if (Res.Successful)
            {
                await IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Succes, $"Succeé Ajouter "));

            }
            else
            {
                await IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Error, $"Erreur Pour Ajoute {Res.ErrorMessage}"));
                return;       
            }
           

        }

        private void  OpenWindow()
        {
            StockItemsWindowViewModel viewModel = new StockItemsWindowViewModel();
            viewModel.ProduitSelected += (sender, e) =>
            {
                if (e.SelectedProductStock != null)
                {
                    NewProduitPharmaStock newProduit = new NewProduitPharmaStock
                    {
                        Id = e.SelectedProductStock.Id,
                        Nom_Commercial = e.SelectedProductStock.Nom_Commercial,
                        Dosage = e.SelectedProductStock.Dosage,
                        Forme = e.SelectedProductStock.Forme,
                        Conditionnement = e.SelectedProductStock.Conditionnement,
                        Type = e.SelectedProductStock.Type
                        
                      

                    };
                    bool idExists = StockProducts.Any(p => p.Id == newProduit.Id && p.Type == newProduit.Type);
                    if (idExists)
                    {
                        return;
                    }
                    StockProducts.Add(newProduit);
                }
            };
            AddStock newWindow = new AddStock(viewModel);
            newWindow.Show();
            
        }

        private async Task BackPage()
        {
            IoC.Application.GoToPage(ApplicationPage.StockHostPage);
            await Task.Delay(1);
        }
         
      

    }
}
