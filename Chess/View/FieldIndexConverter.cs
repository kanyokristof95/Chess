using System;
using System.Globalization;
using System.Windows.Data;

namespace Chess.View
{
    public class FieldIndexConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Tuple<int, char> tuple = new Tuple<int, char>((int) values[0], (char) values[1]);
            return tuple;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            var obj = value as Tuple<int, char>;
            return new object[] { obj.Item1, obj.Item2 };
        }
    }
}
