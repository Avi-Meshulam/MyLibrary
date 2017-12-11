using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary.BL
{
    static class Extensions
    {
        public static IEnumerable<Enum> GetValues(this Type enumType)
        {
            foreach (var name in Enum.GetNames(enumType))
            {
                yield return (Enum)Enum.Parse(enumType, name);
            }
        }
    }
}
