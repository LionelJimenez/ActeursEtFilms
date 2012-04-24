using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows.Media.Imaging;
using System.Windows.Data;

namespace ActeursETFilms
{
    class ClassAfficheConverter: IValueConverter
    {

        public static readonly string dir = "C:/Users/Xrdt/Documents/Visual Studio 2010/Projects/ActeursETFilms/ActeursETFilms/Affiches/";

        public object Convert(object value, Type targetType,
                          object parameter, CultureInfo culture)
        {
            try
            {
                string dirimages = dir + value.ToString()+".jpg";
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
