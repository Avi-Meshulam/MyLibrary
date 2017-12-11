using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Security;
using System.ComponentModel;
using MyLibrary.DAL;

namespace MyLibrary.BL
{
    [Serializable]
    public class User : Entity<User>
    {
        private static uint _nextID = 1;

        public User(uint employeeID, UserType userType)
        {
            _userID = _nextID++;
            _employeeID = employeeID;
            _userType = userType;
        }

        private readonly uint _userID;
        public uint UserID { get { return _userID; } }

        public override object EntityId() { return UserID; }

        [NonSerialized]
        private bool _isLoggedIn;
        public bool IsLoggedIn {
            get { return _isLoggedIn; }
            internal set { _isLoggedIn = value; }
        }

        private readonly uint _employeeID;
        public uint EmployeeID { get { return _employeeID; } }

        private readonly UserType _userType;
        public UserType UserType { get { return _userType; } }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { SetField(ref _userName, value); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { SetField(ref _password, value); }
        }

        public override bool Equals(User other)
        {
            return other != null && _userID == other._userID;
        }

        public override bool Equals(object obj)
        {
            if (obj is User)
                return Equals((User)obj);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return _userID.GetHashCode();
        }

        public override string ToString()
        {
            return _userName;
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            Password = Cryptography.Encrypt(Password);
        }

        [OnSerialized]
        private void OnSerialized(StreamingContext context)
        {
            Password = Cryptography.Decrypt(Password);
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (UserID >= _nextID)
                _nextID = UserID + 1;
            Password = Cryptography.Decrypt(Password);
        }
    }
}
