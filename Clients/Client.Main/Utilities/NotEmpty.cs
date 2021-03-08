using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Controls;

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
}
