using System;
using System.Globalization;
using System.Windows.Data;

namespace ForgeBoard.Converters
{
    public class PNLToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((double)value > 0)
                return System.Windows.Media.Brushes.ForestGreen;

            return System.Windows.Media.Brushes.OrangeRed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
             throw new NotImplementedException(); 
        }
    }
}
