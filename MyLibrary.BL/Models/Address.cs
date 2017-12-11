using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary.BL
{
    [Serializable]
    public class Address
    {
        public Address(string streetName, string streetNumber,
            string city, string state, string zipCode, string country)
        {
            if (!IsValid(streetName, streetNumber, city))
                throw new ArgumentException(ComposeInvalidDataMsg(streetName, streetNumber, city));

            StreetName = streetName;
            StreetNumber = streetNumber;
            City = city;
            State = state;
            ZipCode = zipCode;
            Country = country;
        }

        public Address(string streetName, string streetNumber, string city)
            : this(streetName, streetNumber, city, string.Empty, string.Empty, string.Empty)
        { }

        private string _streetName;
        public string StreetName
        {
            get { return _streetName; }
            set { _streetName = value; }
        }

        private string _streetNumber;
        public string StreetNumber
        {
            get { return _streetNumber; }
            set { _streetNumber = value; }
        }

        private string _city;
        public string City
        {
            get { return _city; }
            set { _city = value; }
        }

        private string _state;
        public string State
        {
            get { return _state; }
            set { _state = value; }
        }

        private string _zipCode;
        public string ZipCode
        {
            get { return _zipCode; }
            set { _zipCode = value; }
        }

        private string _country;
        public string Country
        {
            get { return _country; }
            set { _country = value; }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"{StreetNumber} {StreetName}, {City}");
            sb.Append($"{(string.IsNullOrEmpty(State) ? string.Empty : " " + State + " ")}");
            sb.Append($"{(string.IsNullOrEmpty(ZipCode) ? string.Empty : " " + ZipCode + " ")}");
            sb.Append($"{(string.IsNullOrEmpty(Country) ? string.Empty : ", " + Country + " ")}");
            return sb.ToString();
        }

        public static bool IsValid(string streetName, string streetNumber, string city)
        {
            return
                !string.IsNullOrEmpty(streetName) &&
                !string.IsNullOrEmpty(streetNumber) &&
                !string.IsNullOrEmpty(city);
        }

        private static string ComposeInvalidDataMsg(string streetName, string streetNumber, string city)
        {
            var sb = new StringBuilder();

            if (string.IsNullOrEmpty(streetName))
                Utils.AppendValidationMessage(sb, "Street Name");

            if (string.IsNullOrEmpty(streetNumber))
                Utils.AppendValidationMessage(sb, "Street Number");

            if (string.IsNullOrEmpty(city))
                Utils.AppendValidationMessage(sb, "City");

            return sb.ToString();
        }
    }
}
