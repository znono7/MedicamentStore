﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public class MovmentStockDocViewModel : BaseViewModel
    {
        public ObservableCollection<MouvementStocks> Stocks { get; set; }

        public bool HeaderVisible { get; set; }
        public bool FooterVisible { get; set; }
        public string NumPage { get; set; }
        public string TypeString { get; set; }
        public string DateTod { get; set; }
         
        public MovmentStockDocViewModel(ObservableCollection<MouvementStocks> stocks, bool headerVisible, bool footerVisible, string numPage)
        { 
            Stocks = stocks;
            HeaderVisible = headerVisible;
            FooterVisible = footerVisible;
            NumPage = numPage;
            SetDate();
        }
        private void SetDate()
        {
            DateTime maxDate = Stocks.Max(item => item.Date);
            DateTime minDate = Stocks.Min(item => item.Date);
            TypeString = $"[{minDate.ToString("dd/MM/yyyy")}] - [{maxDate.ToString("dd/MM/yyyy")}]";
            DateTod = DateTime.Today.ToString("dd/MM/yyyy");

        }
    }
}
