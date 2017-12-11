using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary
{
    class EnumKeyValueComparer : IComparer<KeyValuePair<Enum, Enum>>, IComparer
    {
        private static NaturalStringComparer _stringComparer = new NaturalStringComparer();

        public int Compare(KeyValuePair<Enum, Enum> x, KeyValuePair<Enum, Enum> y)
        {
            return _stringComparer.Compare($"{x.Value} {x.Key}", $"{y.Value} {y.Key}");
        }

        public int Compare(object x, object y)
        {
            return Compare((KeyValuePair<Enum, Enum>)x, (KeyValuePair<Enum, Enum>)y);
        }
    }
}
