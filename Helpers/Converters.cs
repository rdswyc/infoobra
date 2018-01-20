using System;
using System.Globalization;
using System.Windows.Data;

using InfoObra.Services;

namespace InfoObra.Helpers
{
    public class ToUpperValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as string).ToUpper();
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class ToLowerValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as string).ToLower();
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class FieldValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return SharePoint.GetField(value as string, parameter as string).DisplayName;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class FichaValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int id;

            if (int.TryParse(value as string, out id))
            {
                return SharePoint.GetVerificacao(id.ToString(), new spList("ficha_verificacao"), parameter as string);
            }
            else
            {
                return string.Empty;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
