using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MedicamentStore
{
   public class DateFilterViewModel : BaseViewModel
    {
        public ICommand FromDateButtonCommand { get; set; }
        public ICommand ToDateButtonCommand { get; set; }
        public ICommand FilerTypeCommand { get; set; }

        public DateFilterType CurrentDateFilterType = DateFilterType.None;
        public bool AttachmentDateVisible { get;  set; }
        public bool AttachmentToDateVisible { get;  set; }

        public string DateStat { get; set; } = "N'importe Quelle Date";
        private DateTime selectedFromDate;
        public DateTime SelectedFromDate
        {
            get { return selectedFromDate; }
            set
            {
                if (selectedFromDate != value)
                {
                    selectedFromDate = value;
                    OnPropertyChanged(nameof(SelectedFromDate));
                    HandleDateChange();
                }
            }
        }
        private DateTime selectedToDate;
        public DateTime SelectedToDate
        {
            get { return selectedToDate; }
            set
            {
                if (selectedToDate != value)
                {
                    selectedToDate = value;
                    OnPropertyChanged(nameof(SelectedToDate));
                    HandleDateChange();
                }
            }
        }


        public DateFilterViewModel()
        {
           // SelectedFromDate = SelectedToDate = DateTime.Now;
            FromDateButtonCommand = new RelayCommand(AttachmentButton);
            ToDateButtonCommand = new RelayCommand(AttachmentToDateButton);
            FilerTypeCommand = new RelayParameterizedCommand(async (p) => await SetFilterType(p));
        }

        private async Task SetFilterType(object p)
        {
            if (p is DateFilterType type)
            {
                CurrentDateFilterType = type;
                DateStat = CurrentDateFilterType.ToDateFilterType();
            }
            await Task.Delay(1);
        }

        private void AttachmentToDateButton()
        {
            AttachmentToDateVisible ^= true;

        }

        public void AttachmentButton()
        {
            // Toggle menu visibility
            AttachmentDateVisible ^= true;
        }

        private void HandleDateChange()
        {
            // Handle the date change here
            CurrentDateFilterType = DateFilterType.WithDate;
            DateStat = CurrentDateFilterType.ToDateFilterType();
        }
    }

    public enum DateFilterType
    {
        None = 0,
        Today = 1,
        Yesterday = 2,
        ThisMonth = 3,
        PastMonth = 4,
        Past3Month = 5,
        WithDate = 6,
    }
    public static class DateFilterTypeExtensions
    {
        public static string ToDateFilterType(this DateFilterType type)
        {
            switch (type)
            {
                case DateFilterType.None:
                    return "N'importe Quelle Date";
                case DateFilterType.Today:
                    return "Aujourd'hui";
                case DateFilterType.Yesterday:
                    return "Hier";
                case DateFilterType.ThisMonth:
                    return "Ce Mois-ci";
                case DateFilterType.PastMonth:
                    return "Mois Passé";
                case DateFilterType.Past3Month:
                    return "les 3 Derniers Mois";
                case DateFilterType.WithDate:
                    return "Date Spécifique";
                default:
                    return string.Empty;
            }
        }
    }
            }
