using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace Client.Main.Utilities
{
    class NotEmpty : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
            {
            string toValidate = value as string;
            if (String.IsNullOrEmpty(toValidate))
            {
                return new ValidationResult(false, "Debe llenar este campo.");
            }
            else
            {
                return new ValidationResult(true, null);
            }
        }
    }
    public class InverseAndBooleansToBooleanConverter : IMultiValueConverter
    {   
        int firstTime = 0;
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (firstTime > 0)
            {
                if (values.LongLength > 0)
                    {
                        foreach (var value in values)
                        {
                            if (value is bool && (bool)value)
                            {
                                return false;
                            }
                        }
                    }
                    return true;
            }
                else 
                { 
                    firstTime += 1;
                    return false;                 
                }

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


}
