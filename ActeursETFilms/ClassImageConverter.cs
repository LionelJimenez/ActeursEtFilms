using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media.Imaging;

namespace ActeursETFilms
{
    class ClassImageConverter: IValueConverter

    {
        public static readonly string dir = "C:/Users/Xrdt/Documents/Visual Studio 2010/Projects/ActeursETFilms/ActeursETFilms/Photos/";
            
        public object Convert(object value, Type targetType,
                          object parameter, CultureInfo culture)
        {
            try
            {
                //return new BitmapImage(new Uri((string)value));
                string dirimages = dir + value.ToString(); 
                BitmapImage bi3 = new BitmapImage();
                bi3.BeginInit();
                bi3.UriSource = new Uri(dirimages, UriKind.RelativeOrAbsolute);
                bi3.EndInit();
                return bi3;
            }
            catch
            {
                return new BitmapImage();
            }
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
