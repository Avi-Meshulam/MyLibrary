using MyLibrary.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary.BL
{
    [Serializable]
    public class Publisher : Entity<Publisher>
    {
        private static uint _nextID = 1;
        private static Publisher _empty;

        private Publisher() {
            _publisherID = 0;
            Name = string.Empty;
        }

        public Publisher(string name)
        {
            Validate(new KeyValuePair<string, object>("Name", name));

            _publisherID = _nextID++;
            Name = name;
        }

        public static Publisher Empty {
            get {
                if (_empty == null)
                    _empty = new Publisher();
                return _empty;
            }
        }

        private readonly uint _publisherID;
        public uint PublisherID { get { return _publisherID; } }

        public override object EntityId() { return PublisherID; }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetField(ref _name, value); }
        }

        private Address _address;
        public Address Address
        {
            get { return _address; }
            set { SetField(ref _address, value); }
        }

        private string _phoneNumber;
        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set { SetField(ref _phoneNumber, value); }
        }

        private string _email;
        public string Email
        {
            get { return _email; }
            set { SetField(ref _email, value); }
        }

        private string _webSite;
        public string WebSite
        {
            get { return _webSite; }
            set { SetField(ref _webSite, value); }
        }

        // Publishers are considered equal if their IDs -OR- Names are equal
        public override bool Equals(Publisher other)
        {
            return other != null && (_publisherID == other._publisherID || _name == other._name);
        }

        public override bool Equals(object obj)
        {
            if(obj is Publisher)
                return Equals((Publisher)obj);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return _publisherID.GetHashCode();
        }

        public override string ToString()
        {
            return _name;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (PublisherID >= _nextID)
                _nextID = PublisherID + 1;
        }
    }
}