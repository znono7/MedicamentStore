using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MedicamentStore
{
    public class InvoiceInfoViewModel : BaseViewModel
    {

        /// <summary>
        /// The current Entered text
        /// </summary>
        public string? EnteredNumText { get; set; }

        public double height { get; set; } = 32;

        public DateTime SelectedDate { get; set; }

        public int LastFactNum { get; set; }

        /// <summary>
        /// True to show the attachment menu, false to hide it
        /// </summary>
        public bool AttachmentDateVisible { get; set; }
       // public bool AttachmentDueDateVisible { get; set; }
        public bool DueDateVisible { get; set; } = false;

     //   public List<string> PaymentTerms { get; set; }

        //private string _selectedPaymentTerm;
        //public string SelectedPaymentTerm 
        //{ 
        //    get { return _selectedPaymentTerm; }
        //    set 
        //    {
        //        _selectedPaymentTerm = value;
        //        OnPropertyChanged(nameof(SelectedPaymentTerm));
        //        if (value == "Dû à Réception")
        //        {
        //            SelectedDueDate = DateTime.Today;
        //            DueDateVisible = true;

        //        }
        //        else if (value == "Net X Jours")
        //        {
        //            // Remplacez 30 par le nombre de jours approprié pour ce terme de paiement
        //            SelectedDueDate = DateTime.Today.AddDays(30);
        //            DueDateVisible = true;

        //        }
        //        else if (value == "Fin de Mois")
        //        {
        //            SelectedDueDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month));
        //            DueDateVisible = true;

        //        }
        //        else if (value == "Conditions Personnalisées")
        //        {
        //            // Remplacez 30 par le nombre de jours approprié pour ce terme de paiement
        //            SelectedDueDate = DateTime.Today;
        //            DueDateVisible = true;

        //        }
        //        else
        //        {
        //            DueDateVisible = false;
        //        }
              
        //    } }

        //public DateTime SelectedDueDate { get; set; }
        /// <summary>
        /// The command for when the attachment button is clicked
        /// </summary>
        public ICommand AttachmentButtonCommand { get; set; }
        public ICommand SetFNumberCommand { get; set; }
        // public ICommand AttachmentDButtonCommand { get; set; }

        public InvoiceInfoViewModel()
        {  
            SelectedDate = DateTime.Today;
            AttachmentButtonCommand = new RelayCommand(AttachmentButton);
            SetFNumberCommand = new RelayCommand(SetFactNumButton);
            _ = GetFactNumber();
            SetFactNum(LastFactNum);
      //  AttachmentDButtonCommand = new RelayCommand(AttachmentDButton);

        //    PaymentTerms = new List<string>
        //{
        //        "---",
        //    "Net X Jours",
        //    "Dû à Réception",
        //    "Paiement à l'Avance",
        //    "Paiement à la Livraison",
                
        //        "Fin de Mois",
        //        "Paiement Partiel",
        //        "Conditions Personnalisées"

        //};
        }

        private void SetFactNumButton()
        {
            int s = LastFactNum++;
            SetFactNum(s);
        }

        /// <summary>
        /// When the attachment button is clicked show/hide the attachment popup
        /// </summary>
        public void AttachmentButton()
        {

            // Toggle menu visibility
            AttachmentDateVisible ^= true;
          

        }
        //public void AttachmentDButton()
        //{

        //    // Toggle menu visibility
        //    AttachmentDueDateVisible ^= true;

        //}

        public async Task GetFactNumber()
        {
            LastFactNum = await IoC.InvoiceManager.GetLastInvoiceNumber();           
        }

        public void SetFactNum(int x)
        {
            EnteredNumText = $"Fact-{SelectedDate.ToShortDateString().Replace("/","")}{x}";
        }
    }
    }
