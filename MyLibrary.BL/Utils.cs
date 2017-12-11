using MyLibrary.BL;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyLibrary
{
    public static class Utils
    {
        public static void AppendValidationMessage(
            StringBuilder sb, string field, ValidationType validationType = ValidationType.RequiredField)
        {
            if (sb == null || string.IsNullOrEmpty(field))
                return;

            if (sb.Length > 0)
                sb.AppendLine();

            switch (validationType)
            {
                case ValidationType.RequiredField:
                    sb.Append($"{field} is required");
                    break;
                case ValidationType.WrongNumberFormat:
                    sb.Append($"{field} has a wrong number format");
                    break;
                default:
                    break;
            }
        }
    }
}
