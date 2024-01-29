using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MedicamentStore
{
    public class FallbackImageConverter : BaseValueConverter<FallbackImageConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string imagePath && !string.IsNullOrEmpty(imagePath))
            {
                try
                {
                    return new BitmapImage(new Uri(imagePath, UriKind.RelativeOrAbsolute));
                }
                catch (Exception)
                {
                    return new BitmapImage(new Uri("pack://application:,,,/Pictures/Lp.jpg", UriKind.RelativeOrAbsolute));

                }
            }

            // Return a default image source if the conversion fails
            return new BitmapImage(new Uri("pack://application:,,,/Pictures/Lp.jpg", UriKind.RelativeOrAbsolute));

          
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
