using MedicamentStore.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    /// <summary>
    /// A converter that takes in a <see cref="IconType"/> and returns 
    /// the FontAwesome string for that icon
    /// </summary>
    public class IconTypeToFontAwesomeConverter : BaseValueConverter<IconTypeToFontAwesomeConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((IconType)value).ToFontAwesome();
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
