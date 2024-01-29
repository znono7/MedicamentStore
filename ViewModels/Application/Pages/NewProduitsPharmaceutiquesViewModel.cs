using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text; 
using System.Threading.Tasks;
using System.Windows.Input;

namespace MedicamentStore
{
    public class NewProduitsPharmaceutiquesViewModel : BaseViewModel
    {
        public TextEntryViewModel ProductName { get; set; } 
        public TextEntryViewModel Forme { get; set; }  
        public TextEntryViewModel Dosage { get; set; }
        public TextEntryViewModel Conditionnement { get; set; }

        public bool IsMedicament { get; set; }
        public bool IsAutre { get; set; } 
       
        private TypeProduct _selectedItem { get; set; }
        public TypeProduct SelectedProduit
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                SetType(value.type.ToProduiyPharmaType());
            }
        }

        public ICommand SaveCommand { get; set; }

        public event EventHandler<SelectProduitPharmaEventArgs> ProduitAded;
        public ProduitPharma ProduitPharmaAded { get; set; }
        public NewProduitsPharmaceutiquesViewModel(ProduitsPharmaceutiquesType type = ProduitsPharmaceutiquesType.Medicaments)
        {
            _ = GetTypes();
            SetType(type);
            SelectedProduit = type == ProduitsPharmaceutiquesType.Medicaments ? items[0] : items[(int)type-1];
            SaveCommand = new RelayCommand(Save);
        }
        public void OnProduitAded(ProduitPharma produitPharma)
        {
            ProduitAded?.Invoke(this, new SelectProduitPharmaEventArgs { SelectedProductPharma = produitPharma });
        }
        private void Save()
        {
           
            if (string.IsNullOrEmpty(ProductName.EnteredText))
            {
                 IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Warning, "Vous Devez Remplir le Nom de Produit Pharmaceutique"));
                return;
            }
            ;
           var res = IoC.ProduitManager.InsertProduit(new ProduitPharma 
            {
                Nom_Commercial = ProductName.EnteredText.ToUpper(),Forme = Forme.EnteredText , Dosage = Dosage.EnteredText,Conditionnement = Conditionnement.EnteredText
                , Img = "Lp.jpg"
            });
            if (res.Result.Successful)
            {
                IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Succes, "le Produit Pharmaceutique Ajoutée"));

                OnProduitAded(res.Result.Response);
            }
            else
            {
                IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Error, "ERREUR "+res.Result.ErrorMessage));

            }

        }

        public void SetNewMedicament()
        {
            IsMedicament = true;
            IsAutre = false;
            ProductName = new TextEntryViewModel { Label = "Nom de Produit" };
            Forme = new TextEntryViewModel { Label = "Forme" };
            Dosage = new TextEntryViewModel { Label = "Dosage" };
            Conditionnement = new TextEntryViewModel { Label = "Conditionnement" };
        }
        public void SetNewAutre()
        {
            IsMedicament = false;
            IsAutre = true;
            ProductName = new TextEntryViewModel { Label = "Nom de Produit" };
            Forme = new TextEntryViewModel { Label = "Forme" };
            Conditionnement = new TextEntryViewModel { Label = "Conditionnement" };
        }
        
        public ObservableCollection<TypeProduct> items { get; set; }
        public async Task GetTypes()
        {
            items = new ObservableCollection<TypeProduct>();
            foreach (ProduitsPharmaceutiquesType type in Enum.GetValues(typeof(ProduitsPharmaceutiquesType)))
            {
                if (type == ProduitsPharmaceutiquesType.None)
                    continue;
                string convertedValue = type.ToProduitsPharmaceutiques();
                int i = (int)type;
                if (convertedValue != null)
                {
                    items.Add(new TypeProduct
                    {
                        Id = i,
                        type = convertedValue
                    });
                }
            }
            
        }
        //public void SetProduitsPharmaceutiquesTypes()
        //{
        //    items = new List<string>();
        //    foreach (ProduitsPharmaceutiquesType type in Enum.GetValues(typeof(ProduitsPharmaceutiquesType)))
        //    {
        //        string convertedValue = type.ToProduitsPharmaceutiques();
        //        if (convertedValue != null)
        //        {
        //            items.Add(convertedValue);
        //        }
        //    }
        //}

        public void SetType(ProduitsPharmaceutiquesType type)
        {
            switch (type)
            {
                case ProduitsPharmaceutiquesType.Medicaments:
                case ProduitsPharmaceutiquesType.MedicamentsPourPrevention:
                case ProduitsPharmaceutiquesType.MedicamentsPourDAT:
                case ProduitsPharmaceutiquesType.MedicamentsPourGenevologie:
                case ProduitsPharmaceutiquesType.VaccinsEtSerums:
                case ProduitsPharmaceutiquesType.AntiSeptiques:
                case ProduitsPharmaceutiquesType.SolutesMassifs:
                case ProduitsPharmaceutiquesType.Psychotropes:
                case ProduitsPharmaceutiquesType.ContaceptifsOraux:
                case ProduitsPharmaceutiquesType.Reactifs:
                    SetNewMedicament();
                    break;
                case ProduitsPharmaceutiquesType.ArticleDePansements:
                case ProduitsPharmaceutiquesType.Consommables:
                case ProduitsPharmaceutiquesType.ProduitsDentaires:
                case ProduitsPharmaceutiquesType.FilmsRadiologiques:
                case ProduitsPharmaceutiquesType.MaterielsDeLaboratoires:
                case ProduitsPharmaceutiquesType.Appareils:
                case ProduitsPharmaceutiquesType.ProduitsPourDiagnostic:
                case ProduitsPharmaceutiquesType.MoyensDeProtection:
                case ProduitsPharmaceutiquesType.Sondes:
                case ProduitsPharmaceutiquesType.Masques:
                case ProduitsPharmaceutiquesType.Papier:
                case ProduitsPharmaceutiquesType.Gel:
                    SetNewAutre();
                    break;
                default:
                    SetNewMedicament();
                    break;
            }
        }
    }
}
