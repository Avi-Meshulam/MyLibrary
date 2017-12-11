using System;
using System.Collections.Generic;

namespace MyLibrary.BL
{
    internal class EnumStringComparer : IEqualityComparer<Enum>
    {
        public bool Equals(Enum x, Enum y)
        {
            return StringComparer.CurrentCulture.Equals(x.ToString(), y.ToString());
        }

        public int GetHashCode(Enum obj)
        {
            return obj.GetHashCode();
        }
    }
}