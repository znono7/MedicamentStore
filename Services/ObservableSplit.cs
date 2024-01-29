using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public class ObservableSplit
    {
        protected int itemsPerCollection1 = 14;
        protected int itemsPerCollection2 = 20;
        protected int totalCollections1;
        protected int totalCollections2;

        public ObservableSplit(ObservableCollection<MedicamentStock> stocks)
        {
            Stocks = stocks;
           
            
        }

        public ObservableCollection<MedicamentStock> Stocks { get; }
        public ObservableCollection<MedicamentStock> collections1 {  get; set; }
        public ObservableCollection<MedicamentStock> collections2 {  get; set; }
        public List<ObservableCollection<MedicamentStock>> Listcollections {  get; set; }

        public void FirstCollection()
        {
            collections1 = new ObservableCollection<MedicamentStock>(Stocks.Take(itemsPerCollection1));

        }
        public void SecondCollection()
        {
            collections2 = new ObservableCollection<MedicamentStock>(Stocks.Skip(itemsPerCollection1).Take(itemsPerCollection2));

        }

        public void ListCollection()
        {

        }
        public void SeparateList()
        {
            int totalCollections2 = (int)Math.Ceiling((double)(Stocks.Count - itemsPerCollection1) / itemsPerCollection2);

            Listcollections = new List<ObservableCollection<MedicamentStock>>();
            for (int i = 0; i < totalCollections2; i++)
            {
                ObservableCollection<MedicamentStock> collection = new ObservableCollection<MedicamentStock>(
                    Stocks.Skip((i* itemsPerCollection2) +
                    itemsPerCollection2).Take(itemsPerCollection2)
                );
                Listcollections.Add(collection);
            }

        }
    }
}
