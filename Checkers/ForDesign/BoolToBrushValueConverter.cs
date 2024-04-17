using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Checkers.ForDesign
{
    class BoolToBrushValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && (bool)value)
            {
                
                LinearGradientBrush brush = new LinearGradientBrush();
                brush.StartPoint = new System.Windows.Point(0, 0);
                brush.EndPoint = new System.Windows.Point(1, 1);
                brush.GradientStops.Add(new GradientStop(Colors.Black, 0.5));
                brush.GradientStops.Add(new GradientStop(Colors.White, 1.0));
                return brush;
            }
            else
            {

                LinearGradientBrush brush = new LinearGradientBrush();
                brush.StartPoint = new System.Windows.Point(0, 0);
                brush.EndPoint = new System.Windows.Point(1, 1);
                brush.GradientStops.Add(new GradientStop(Colors.LightGray, 0.0));
                brush.GradientStops.Add(new GradientStop(Colors.WhiteSmoke, 1.0)); 
                return brush;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
