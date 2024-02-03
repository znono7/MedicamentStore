using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace MedicamentStore
{
    public class MouvementViewModel : BaseViewModel
    {
        protected ObservableCollection<TransactionDto> stocks;
        public ObservableCollection<TransactionDto> Stocks
        {
            get => stocks;
            set
            {
                if (stocks == value)
                    return;
                stocks = value;

                FilteredStocks = new ObservableCollection<TransactionDto>(stocks);
            }
        }

        public ObservableCollection<TransactionDto> FilteredStocks { get; set; }
        public DateFilterViewModel DateFilterViewModel { get; set; }
        public ObservableCollection<TypeProduct> TypeItems { get; set; }
        public string DateStat => DateFilterViewModel.DateStat;

        public bool isExpanded {  get; set; }

        public ICommand ExpandCommand { get; set; }
        public ICommand FilterDataCommand { get; set; }
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
            FilterDataCommand = new RelayCommand(async () => await FilterData());
            _ = LoadData();
        }

        public async Task LoadData()
        {
            var Result = await IoC.TransactionManager.GetAll();
            Stocks = new ObservableCollection<TransactionDto>(Result);
            foreach (var item in Stocks)
            {
                if(item.TypeTransaction == 1)
                {
                    item.SymbleType = "+";
                    item.PrimaryBackground = "349432";
                }else if (item.TypeTransaction == 2)
                {
                    item.SymbleType = "-";
                    item.PrimaryBackground = "EF4444";
                }
                item.PrixTotal = double.Parse(string.Format("{0:0.00}", item.Prix * item.Quantite));

            }
        }
        private async Task FilterData()
        {
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            switch (DateFilterViewModel.CurrentDateFilterType)
            {
                case DateFilterType.None:
                    FilteredStocks = new ObservableCollection<TransactionDto>(Stocks);
                    return;
                case DateFilterType.Today:
                    startDate = DateTime.Today;
                    endDate = DateTime.Today.AddDays(1).AddTicks(-1);
                    break;
                case DateFilterType.Yesterday:
                    startDate = DateTime.Today.AddDays(-1);
                    endDate = DateTime.Today.AddTicks(-1);
                    break;
                case DateFilterType.ThisMonth:
                    startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                    endDate = startDate.AddMonths(1).AddTicks(-1);
                    break;
                case DateFilterType.PastMonth:
                    startDate = new DateTime(DateTime.Today.AddMonths(-1).Year, DateTime.Today.AddMonths(-1).Month, 1);
                    endDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddTicks(-1);
                    break;
                case DateFilterType.Past3Month:
                    startDate = new DateTime(DateTime.Today.AddMonths(-3).Year, DateTime.Today.AddMonths(-3).Month, 1);
                    endDate = DateTime.Today.AddTicks(-1);
                    break;
                case DateFilterType.WithDate:
                    startDate = DateFilterViewModel.SelectedFromDate;
                    endDate = DateFilterViewModel.SelectedToDate;
                    break;
                
                default:
                    startDate = DateTime.Today;
                    endDate = DateTime.Today.AddDays(1).AddTicks(-1);
                    break;
            }
             
            FilteredStocks = new ObservableCollection<TransactionDto>( Stocks.Where(item => item.Date >= startDate.Date &&
                                                                                            item.Date <= endDate.Date));
            
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
