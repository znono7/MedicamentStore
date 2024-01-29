using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public class IntegerConverter : BaseValueConverter<IntegerConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
           

            if (value is int intValue)
            {
                return intValue.ToString();
            }
            return value;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string input)
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    return 0; // Default value when TextBox is empty
                }
                if (int.TryParse(input, out int result))
                {
                    return result;
                }
            }
            return 0;
        }
    }
}
