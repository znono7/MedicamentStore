using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MedicamentStore
{
    public class MouvementViewModel : BaseViewModel
    {
        public DateFilterViewModel DateFilterViewModel { get; set; }
        public ObservableCollection<TypeProduct> TypeItems { get; set; }

        public bool isExpanded {  get; set; }

        public ICommand ExpandCommand { get; set; }
        public ICommand PopupClickawayCommand { get; set; }
        public bool DimmableOverlayVisible { get; set; }
        private TypeProduct _selectedType { get; set; }

        public TypeProduct SelectedType
        {
            get { return _selectedType; }
            set
            {
                _selectedType = value;
                //_ = GetProducts(_selectedType.Id);
                //Unit = _selectedUnite?.Id ?? 0;
            }
        }
        public MouvementViewModel()
        {
            DateFilterViewModel = new DateFilterViewModel();
            GetTypes();
            ExpandCommand = new RelayCommand(AttachmentMenuButton);
            PopupClickawayCommand = new RelayCommand(ClickawayMenuButton);
        }

        private void ClickawayMenuButton()
        {
            isExpanded = false;
        }

        private void AttachmentMenuButton()
        {
            isExpanded ^= true;
        }

        public void GetTypes()
        {
            TypeItems = new ObservableCollection<TypeProduct>();
            foreach (ProduitsPharmaceutiquesType type in Enum.GetValues(typeof(ProduitsPharmaceutiquesType)))
            {
                if (type == ProduitsPharmaceutiquesType.None)
                    continue;
                string convertedValue = type.ToProduitsPharmaceutiques();
                int i = (int)type;
                if (convertedValue != null)
                {
                    TypeItems.Add(new TypeProduct
                    {
                        Id = i,
                        type = convertedValue
                    });
                }
            }
        }
    }
}
