using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary.BL
{
    [Serializable]
    public struct ItemCopy
    {
        private uint _copyNo;
        public uint CopyNo
        {
            get { return _copyNo; }
            internal set { _copyNo = value; }
        }

        private bool _isBorrowed;
        public bool IsBorrowed
        {
            get { return _isBorrowed; }
            set
            {
                _isBorrowed = value;
                BorrowingDate = _isBorrowed ? (DateTime?)DateTime.Now : null;
            }
        }

        private DateTime? _borrowingDate;
        public DateTime? BorrowingDate
        {
            get { return _borrowingDate; }
            private set { _borrowingDate = value; }
        }

        [NonSerialized]
        private double? _borrowingPeriod;
        public double? BorrowingPeriod
        {
            get {
                return _borrowingPeriod =_borrowingDate == null ?
                    null : (double?)Math.Round((DateTime.Now - (DateTime)_borrowingDate).TotalDays, 1);
            }
        }
    }
}
