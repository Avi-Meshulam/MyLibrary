using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary.BL
{
    [Serializable]
    public class Journal : LibraryItem
    {
        public Journal(ISBN isbn, string name, Category category, string editor) 
            : base(isbn, name, category)
        {
            Editor = editor;
        }

        public Journal(ISBN isbn, string name, Category category) 
            : this(isbn, name, category, string.Empty)
        { }

        private string _editor;
        public string Editor
        {
            get { return _editor; }
            set { SetField(ref _editor, value); }
        }
    }
}
