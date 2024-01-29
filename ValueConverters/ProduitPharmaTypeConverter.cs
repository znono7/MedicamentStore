using MedicamentStore.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public class ProduitsPharmaceutiquesTypeConverter : BaseValueConverter<ProduitsPharmaceutiquesTypeConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((ProduitsPharmaceutiquesType)value).ToProduitsPharmaceutiques();
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
