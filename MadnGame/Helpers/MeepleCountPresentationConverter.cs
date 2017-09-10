using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MadnGame.Helpers
{
    public class MeepleCountPresentationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            List<Object> meeples = new List<Object>();
            if (value is int)
            {
                for (int i = 0; i < (int)value; i++)
                {
                    meeples.Add(new Object());
                }
            }
            return meeples;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
