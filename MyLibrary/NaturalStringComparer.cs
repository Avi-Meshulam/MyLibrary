using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary
{
    class NaturalStringComparer : IComparer<string>, IComparer<object>, IComparer
    {
        public int Compare(string x, string y)
        {
            if (x == null || y == null)
                return CompareNullArguments(x, y);

            return GUIUtils.CompareNatural(x, y);
        }

        public int Compare(object x, object y)
        {
            return Compare(x?.ToString(), y?.ToString());
        }

        private int CompareNullArguments(object x, object y)
        {
            if (x == null && y == null)
                return 0;

            if (x == null)
                return -1;
            else
                return 1;
        }
    }
}
