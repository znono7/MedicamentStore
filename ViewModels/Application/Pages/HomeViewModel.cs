using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public class HomeViewModel : BaseViewModel
    {
        public InfoCardViewModel? InfoCard1 { get; set; }
        public InfoCardViewModel? InfoCard2 { get; set; }
        public InfoCardViewModel? InfoCard3 { get; set; }

        public HomeViewModel()
        {
         
        }

        public async Task SetProp()
        {
            InfoCard1 = new InfoCardViewModel
            {
                Title = "Statistiques du magasin",
                IconType = IconType.InfoStock,
                PrimaryBackground = "827BFF",
                PrimaryBackground2 = "D9B5FF",
                EllipseBackground1 = "B298FD",
                EllipseBackground2 = "E4BBFF"
            };
            InfoCard2 = new InfoCardViewModel
            {
                Title = "Statistiques des produits pharmaceutiques",
                IconType = IconType.InfoPrix,
                PrimaryBackground = "FD8A87",
                PrimaryBackground2 = "F3AB92",
                EllipseBackground1 = "FBD5A8",
                EllipseBackground2 = "FDB89B"
            };
            InfoCard3 = new InfoCardViewModel
            {
                Title = "Statistiques d'exportation",
                IconType = IconType.InfoExport,
                PrimaryBackground = "FC84CB",
                PrimaryBackground2 = "FC8FAE",
                EllipseBackground1 = "FC8FAE",
                EllipseBackground2 = "FFABE6"
            };
            await Task.Delay(1);
        }
    }
}
 