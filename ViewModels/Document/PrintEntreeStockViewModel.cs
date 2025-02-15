﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MedicamentStore
{
    public class PrintEntreeStockViewModel : BaseViewModel
    {
        public ICommand ReturnCommand { get; set; }
        public ObservableCollection<EnterTransaction> Stocks { get; }

        public PrintEntreeStockViewModel(ObservableCollection<EnterTransaction> stocks) 
        {
            ReturnCommand = new RelayCommand(async () => await ToBackPage());
            Stocks = stocks;
        }

        private async Task ToBackPage()
        {
            IoC.Application.GoToPage(ApplicationPage.EntreeStockPage);
            await Task.Delay(1);
        }
    }
}
