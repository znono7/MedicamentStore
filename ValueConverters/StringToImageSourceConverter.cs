using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MedicamentStore
{
    public class StringToImageSourceConverter : BaseValueConverter<StringToImageSourceConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string imagePath)
            {
                try
                {
                    // Create a new BitmapImage
                    BitmapImage image = new BitmapImage();
                    // Set the image UriSource to the string path
                    image.BeginInit();
                    image.UriSource = new Uri(imagePath);
                    image.EndInit();
                    // Return the BitmapImage as ImageSource
                    return image;
                }
                catch (Exception)
                {

                    return null;
                }    
            }
            return null;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
