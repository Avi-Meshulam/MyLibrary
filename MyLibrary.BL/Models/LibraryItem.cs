using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MyLibrary.DAL;
using System.Collections.Concurrent;
using System.Collections.Specialized;

namespace MyLibrary.BL
{
    [Serializable]
    public abstract class LibraryItem : Entity<LibraryItem>
    {
        public LibraryItem(ISBN isbn, string title, Category category)
        {
            Validate(new KeyValuePair<string, object>("Title", title));

            _isbn = isbn;
            _title = title;
            _category = category;
            _copies = new ObservableConcurrentDictionary<uint, ItemCopy>();
            _copies.CollectionChanged += _copies_CollectionChanged;
            AddCopy();
        }

        private void _copies_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Update();
        }

        private readonly ISBN _isbn;
        public ISBN ISBN { get { return _isbn; } }

        public override object EntityId() { return ISBN; }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetField(ref _title, value); }
        }

        // In order to save storage size, only PublisherID and PublisherName (which is used for display) are serialized 
        [NonSerialized]
        private Publisher _publisher;
        public Publisher Publisher
        {
            set
            {
                SetField(ref _publisher, value);
                _publisherID = _publisher?.PublisherID;
                _publisherName = _publisher?.Name;
            }
        }

        //[NonSerialized]   // Unmark when serializing with SoapFormatter
        // PublisherID is set only through Publisher property
        private uint? _publisherID;
        public uint? PublisherID { get { return _publisherID; } }

        private string _publisherName;
        public string PublisherName { get { return _publisherName; } }

        private string _graphicDesigner;
        public string GraphicDesigner
        {
            get { return _graphicDesigner; }
            set { SetField(ref _graphicDesigner, value); }
        }

        //[NonSerialized]   // Unmark when serializing with SoapFormatter
        private DateTime? _printDate;
        public DateTime? PrintDate
        {
            get { return _printDate; }
            set { SetField(ref _printDate, value); }
        }

        //[NonSerialized]   // Unmark when serializing with SoapFormatter
        private ObservableConcurrentDictionary<uint, ItemCopy> _copies
            = new ObservableConcurrentDictionary<uint, ItemCopy>();
        public ObservableConcurrentDictionary<uint, ItemCopy> Copies { get { return _copies; } }

        private Category _category;
        public Category Category
        {
            get { return _category; }
            set
            {
                SetField(ref _category, value);
                if (!(CategoriesRepository.Categories[value].Contains(SubCategory)))
                    SubCategory = default(Enum);
            }
        }

        private Enum _subCategory;
        public Enum SubCategory
        {
            get { return _subCategory; }
            set
            {
                if (value != default(Enum))
                {
                    if (!(CategoriesRepository.Categories[Category]?.Contains(value) ?? false))
                        throw new ArgumentException(
                            $"Sub Category <{Enum.GetName(value.GetType(), value)}> " +
                            $"does not match Categoty <{Enum.GetName(Category.GetType(), Category)}>");
                }

                SetField(ref _subCategory, value);
            }
        }

        [NonSerialized]
        private bool _isAvailable;
        public bool IsAvailable { get { return _isAvailable = _copies.Values.Any(i => i.IsBorrowed == false); } }

        public ItemCopy AddCopy()
        {
            var itemCopy = new ItemCopy { CopyNo = _copies.Keys.LastOrDefault() + 1 };
            _copies.Add(itemCopy.CopyNo, itemCopy);
            return itemCopy;
        }

        public bool DeleteCopy(uint CopyNo)
        {
            ItemCopy itemCopy;
            if (!_copies.TryGetValue(CopyNo, out itemCopy))
                return false;

            if (_copies.Remove(itemCopy.CopyNo))
            {
                return true;
            }

            return false;
        }

        public bool BorrowCopy(uint CopyNo)
        {
            return BorrowReturnCopy(CopyNo, isBorrowing: true);
        }

        public bool ReturnCopy(uint CopyNo)
        {
            return BorrowReturnCopy(CopyNo, isBorrowing: false);
        }

        private bool BorrowReturnCopy(uint CopyNo, bool isBorrowing)
        {
            ItemCopy itemCopy;
            if (!_copies.TryGetValue(CopyNo, out itemCopy))
                return false;

            itemCopy.IsBorrowed = isBorrowing;

            _copies[CopyNo] = itemCopy;

            return true;
        }

        public override bool Equals(LibraryItem other)
        {
            return other != null && _isbn == other._isbn;
        }

        public override bool Equals(object obj)
        {
            if (obj is LibraryItem)
                return Equals((LibraryItem)obj);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return _isbn.GetHashCode();
        }

        public override string ToString()
        {
            return $"'{_title}' ISBN: {_isbn}";
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            _copies = new ObservableConcurrentDictionary<uint, ItemCopy>();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            _copies.CollectionChanged += _copies_CollectionChanged;
        }
    }
}
