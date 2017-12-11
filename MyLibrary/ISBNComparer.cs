using MyLibrary.BL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary
{
    class ISBNComparer : IComparer<ISBN>, IComparer
    {
        public int Compare(object x, object y)
        {
            return Compare((ISBN) x, (ISBN) y);
        }

        public int Compare(ISBN x, ISBN y)
        {
            return x.Value > y.Value ? 1 : x.Value < y.Value ? -1 : 0;
        }
    }
}
