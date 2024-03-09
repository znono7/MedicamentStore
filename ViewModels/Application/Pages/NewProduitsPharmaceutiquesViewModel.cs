using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text; 
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MedicamentStore
{
    public class NewProduitsPharmaceutiquesViewModel : BaseViewModel
    {
        private ImageSource _imageSource;


        public ImageSource MyImageSource
        {
            get { return _imageSource; }
            set
            {
                _imageSource = value;
                OnPropertyChanged("MyImageSource");
            }
        }
        public TextEntryViewModel ProductName { get; set; } 
        public TextEntryViewModel Forme { get; set; }  
        public TextEntryViewModel Dosage { get; set; }
        public TextEntryViewModel Conditionnement { get; set; }

        public bool IsMedicament { get; set; } 
        public bool IsAutre { get; set; } 
       
        public ICommand ChooseImgCommand { get; set; }
        private TypeProduct _selectedItem { get; set; }
        public TypeProduct SelectedProduit
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                //SetType(value.type.ToProduiyPharmaType());
            }
        }

        public ICommand SaveCommand { get; set; }

        public event EventHandler<SelectProduitPharmaEventArgs> ProduitAded;
        public ProduitPharma ProduitPharmaAded { get; set; }
        public NewProduitsPharmaceutiquesViewModel(ProduitsPharmaceutiquesType type = ProduitsPharmaceutiquesType.Medicaments)
        {
            _ = GetTypes();
            ProductName = new TextEntryViewModel { Label = "Nom de Produit" };
            Forme = new TextEntryViewModel { Label = "Forme" };
            Dosage = new TextEntryViewModel { Label = "Dosage/Taille " };
            Conditionnement = new TextEntryViewModel { Label = "Conditionnement" };
            //SetType(type);
            SelectedProduit = type == ProduitsPharmaceutiquesType.Medicaments ? items[0] : items[(int)type-1];
            SaveCommand = new RelayCommand(Save);
            ChooseImgCommand = new RelayCommand(async () => await ChooseImg());
        }

        private async Task ChooseImg()
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg;*.gif;*.bmp)|*.png;*.jpeg;*.jpg;*.gif;*.bmp|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                string selectedFilePath = openFileDialog.FileName;
                ImageName = Path.GetFileName(selectedFilePath);
                LoadImageFromDisk(selectedFilePath);
            }
        }

        private void LoadImageFromDisk(string filePath)
        {
            try
            {
                // Create a BitmapImage and set its URI source
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(filePath);
                bitmapImage.EndInit();

                // Set the BitmapImage as the source for the Image control
                MyImageSource = bitmapImage;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}");
            }
        }

        public void OnProduitAded(PharmaceuticalProduct produitPharma)
        {
            ProduitAded?.Invoke(this, new SelectProduitPharmaEventArgs { SelectedProductPharma = produitPharma });
        }
        private void SaveImageToDisk()
        {
            

            // Convert the image source to a BitmapSource
            BitmapSource bitmapSource = MyImageSource as BitmapSource;
           
            if (bitmapSource != null)
            {
                // Create a BitmapEncoder based on the desired image format (e.g., JPEG)
                BitmapEncoder encoder = new JpegBitmapEncoder(); // Change this to the desired format if needed

                // Create a FileStream to write the image data to a file on disk
                string filePath = $".\\Pictures\\{ImageName}"; // Change this to your desired file path and name
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    // Encode the bitmap source and write it to the file stream
                    encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                    encoder.Save(fileStream);
                }

                MessageBox.Show("Image saved successfully!");
            }
            else
            {
                MessageBox.Show("Failed to save image. Image source is invalid.");
            }
        }

        private void Save()
        {
            //SaveImageToDisk();
            //return;
            if (string.IsNullOrEmpty(ProductName.EnteredText))
            {
                 IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Warning, "Vous Devez Remplir le Nom de Produit Pharmaceutique"));
                return;
            }
            ;
            var produit = new PharmaceuticalProduct
            {
                Nom_Commercial = ProductName.EnteredText.ToUpper(),
                Forme = Forme.EnteredText,
                Dosage = Dosage.EnteredText,
                Conditionnement = Conditionnement.EnteredText,
                Img = ImageName,
                Type = SelectedProduit.Id,
                imageSource = MyImageSource
            };
            OnProduitAded(produit);
           //var res = IoC.ProduitManager.InsertProduit(produit);
           // if (res.Result.Successful)
           // {
           //     IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Succes, "le Produit Pharmaceutique Ajoutée"));
           //     SaveImageToDisk();
           //     OnProduitAded(res.Result.Response);
           // }
           // else
           // {
           //     IoC.NotificationBox.ShowMessage(new NotificationBoxViewModel(NotificationType.Error, "ERREUR "+res.Result.ErrorMessage));

           // }

        }

        public void SetNewMedicament()
        {
            IsMedicament = true;
            IsAutre = false;
            ProductName = new TextEntryViewModel { Label = "Nom de Produit" };
            Forme = new TextEntryViewModel { Label = "Forme" };
            Dosage = new TextEntryViewModel { Label = "Dosage/Taille " };
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
        public string? ImageName { get; private set; }

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
