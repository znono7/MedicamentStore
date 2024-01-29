using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MedicamentStore
{
    public class NewSupplieViewModel : BaseViewModel
    {
        public TextEntryViewModel Nom { get; set; }
        public TextEntryViewModel Adresse { get; set; }  
        public TextEntryViewModel region { get; set; }
       

     

        public ICommand SaveCommand { get; set; }

        public event EventHandler<SelectSuppPharmaEventArgs> SelectSuppPharma;

        public ProduitPharma ProduitPharmaAded { get; set; }
        public NewSupplieViewModel()
        {

            Nom = new TextEntryViewModel { Label = "Nom" };
            Adresse = new TextEntryViewModel { Label="Adresse" };
            region = new TextEntryViewModel { Label="Région" };
            SaveCommand = new RelayCommand(Save);
        }
        public void OnProduitAded(Supplies supplies )
        {
            SelectSuppPharma?.Invoke(this, new SelectSuppPharmaEventArgs { SelectedSupplies = supplies });
        }
        private void Save()
        {
            if (string.IsNullOrEmpty(Nom.EnteredText))
            {
                 IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Warning, "Vous Devez Remplir le Nom de Fournisseur"));
                return;
            }
            ;
            var Res = IoC.SuppliesManager.InsertSupplieAsync(new Supplies
            {
                Nom = Nom.EnteredText,
                Adresse = Adresse.EnteredText,
                region = region.EnteredText
            });

            if (Res.Result.Successful)
            {
                IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Succes, "le Fournisseur Ajoutée"));

                OnProduitAded(Res.Result.Response);
            }
            else
            {
                IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Error, "ERREUR " + Res.Result.ErrorMessage));

            }

        }

       
       

       
    }
}
