using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using ESAPIScript;

namespace ESAPIScript.Converters
{
   
     public class SortStructuresConverter : IMultiValueConverter
    {
        // Multivalue converter example sorting structures in a list according to how closesly they match a comparator, using Levenshtein Distance
        public object Convert(object[] value, Type targetType,
               object parameter, System.Globalization.CultureInfo culture)
        {
            string init = (value[0] as string);
            ObservableCollection<string> AvailableStructures = value[1] as ObservableCollection<string>;
            if (init == null)
            {
                return AvailableStructures;
            }
            if (AvailableStructures.Count != 0)
            {
                double[] LD = new double[AvailableStructures.Count];
                for (int i = 0; i < AvailableStructures.Count; i++)
                {
                    LD[i] = double.PositiveInfinity;
                }
                int c = 0;
                foreach (string S in AvailableStructures)
                {
                    var CurrentId = init.ToUpper();
                    var stripString = S.Replace(@"B_", @"").Replace(@"_", @"").ToUpper();
                    var CompString = CurrentId.Replace(@"B_", @"").Replace(@"_", @"").ToUpper();
                    double LDist = Helpers.LevenshteinDistance.Compute(stripString, CompString);
                    if (stripString.ToUpper().Contains(CompString) && stripString != "" && CompString != "")
                        LDist = Math.Min(LDist, 1.5);
                    LD[c] = LDist;
                    c++;
                }
                var temp = new ObservableCollection<string>(AvailableStructures.Zip(LD, (s, l) => new { key = s, LD = l }).OrderBy(x => x.LD).Select(x => x.key).ToList());
                return temp;
            }
            else
                return AvailableStructures;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class VisibilityConverter : IValueConverter
    {
        // Convert a boolean into a visibility setting
        public object Convert(object value, Type targetType,
               object parameter, System.Globalization.CultureInfo culture)
        {
            bool? V = value as bool?;
            if (V == null)
                return Visibility.Collapsed;
            if (V == true)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetTypes,
               object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility? V = value as Visibility?;
            if (V == Visibility.Hidden)
                return false;
            else
                return true;
        }
    }
    public class VisibilityInverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
               object parameter, System.Globalization.CultureInfo culture)
        {
            bool? V = value as bool?;
            if (V == null)
                return Visibility.Collapsed;
            if (V == false)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetTypes,
               object parameter, System.Globalization.CultureInfo culture)
        {
            Visibility? V = value as Visibility?;
            if (V == Visibility.Hidden)
                return false;
            else
                return true;
        }
    }
    public class VisibilityMultiConverter : IMultiValueConverter
    {
        // Example of how to set visibility based on multiple inputs
        public object Convert(object[] value, Type targetType,
              object parameter, System.Globalization.CultureInfo culture)
        {
            foreach (bool v in value)
            {
                if (v)
                    return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
        
}
