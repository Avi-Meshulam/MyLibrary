using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary.BL
{
    [Serializable]
    public class Book : LibraryItem
    {
        public Book(ISBN isbn, string name, Category category, string author) 
            : base(isbn, name, category)
        {
            Author = author;
        }

        public Book(ISBN isbn, string name, Category category) 
            : this(isbn, name, category, string.Empty)
        { }

        private string _edition;
        public string Edition
        {
            get { return _edition; }
            set { SetField(ref _edition, value); }
        }

        private string _translator;
        public string Translator
        {
            get { return _translator; }
            set { SetField(ref _translator, value); }
        }

        private string _author;
        public string Author
        {
            get { return _author; }
            set { SetField(ref _author, value); }
        }
    }
}
