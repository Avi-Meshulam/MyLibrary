using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary.DAL
{
    public static class Extensions
    {
        public static string PluralForm(this string name)
        {
            var ps = PluralizationService.CreateService(new CultureInfo("en-us"));
            return ps.Pluralize(name);
        }
    }
}
