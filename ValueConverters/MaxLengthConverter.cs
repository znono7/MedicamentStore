using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public class MaxLengthConverter : BaseValueConverter<MaxLengthConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int maxLength = System.Convert.ToInt32(parameter);
            string input = value as string;

            if (input.Length > maxLength)
            {
                return input.Substring(0, maxLength);
            }

            return input;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value; // One-way binding, no need to convert back
        }
    }
}
