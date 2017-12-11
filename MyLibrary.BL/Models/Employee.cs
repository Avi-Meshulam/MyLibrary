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
    public class Employee : Entity<Employee>
    {
        private static uint _nextID = 1;

        public Employee(string firstName, string lastName)
        {
            Validate(new KeyValuePair<string, object>("First Name", firstName)
                /*, new KeyValuePair<string, object>("Last Name", lastName)*/);

            _employeeID = _nextID++;
            FirstName = firstName;
            LastName = lastName;
        }

        private readonly uint _employeeID;
        public uint EmployeeID { get { return _employeeID; } }

        public override object EntityId() { return EmployeeID; }

        private string _firstName;
        public string FirstName
        {
            get { return _firstName; }
            set { SetField(ref _firstName, value); }
        }

        private string _lastName;
        public string LastName
        {
            get { return _lastName; }
            set { SetField(ref _lastName, value); }
        }

        private Address _address;
        public Address Address
        {
            get { return _address; }
            set { SetField(ref _address, value); }
        }

        private string _homePhone;
        public string HomePhone
        {
            get { return _homePhone; }
            set { SetField(ref _homePhone, value); }
        }

        private string _cellPhone;
        public string CellPhone
        {
            get { return _cellPhone; }
            set { SetField(ref _cellPhone, value); }
        }

        public override bool Equals(Employee other)
        {
            return other != null && _employeeID == other._employeeID;
        }

        public override bool Equals(object obj)
        {
            if (obj is Employee)
                return Equals((Employee)obj);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return _employeeID.GetHashCode();
        }

        public override string ToString()
        {
            return $"{_firstName} {_lastName}";
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (EmployeeID >= _nextID)
                _nextID = EmployeeID + 1;
        }
    }
}
