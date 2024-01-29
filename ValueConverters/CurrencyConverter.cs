using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedicamentStore
{
    public class CurrencyConverter : BaseValueConverter<CurrencyConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double amount)
            {
                return $"{amount.ToString("N2", CultureInfo.CurrentCulture)} DA";
            }
            return value;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string input)
            {
                // Remove currency symbols, commas, etc., and convert back to double
                if (double.TryParse(input.Replace("DA", "").Trim(), NumberStyles.Currency, CultureInfo.CurrentCulture, out double result))
                {
                    return result;
                }
            }
            return value;
        }
    }
}
