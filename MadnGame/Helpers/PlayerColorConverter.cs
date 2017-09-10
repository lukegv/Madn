using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace MadnGame.Helpers
{
    public class PlayerColorConverter : IValueConverter
    {
        private readonly Color[] PlayerColors = new Color[] {
            Colors.Red, Colors.Green, Colors.Blue, Colors.Yellow, Colors.Pink, Colors.Orange, Colors.Lime
        };

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int && ((int)value).InBounds(0, this.PlayerColors.Length - 1))
            {
                return new SolidColorBrush(this.PlayerColors[(int)value]);
            }
            else
            {
                return new SolidColorBrush(Colors.Transparent);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public static class IntExtensions
    {
        public static bool InBounds(this int value, int lower, int upper)
        {
            return (value >= lower) && (value <= upper);
        }
    }
}
