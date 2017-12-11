using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary.BL
{
    /// <summary>
    /// 13 digit ISBN consists of 5 parts:
    ///     1. Prefix – a GS1 prefix: so far 978 or 979 have been made available by GS1.
    ///     2. Group Identifier (language-sharing country group, individual country or territory).
    ///     3. Publisher code
    ///     4. Catalogue number.
    ///     5. Check digit.
    /// <see cref="https://en.wikipedia.org/wiki/International_Standard_Book_Number#Overview"/>
    /// </summary>
    [Serializable]
    public class ISBN : IEquatable<ISBN>
    {
        public const int ISBN_LENGTH = 13;

        public ISBN(ISBN_978_GroupIdentifier groupIdentifier, string publisherCode, string catalogueNumber)
            : this(ISBN_Prefix.ISBN_978, (int)groupIdentifier, publisherCode, catalogueNumber)
        { }

        public ISBN(ISBN_979_GroupIdentifier groupIdentifier, string publisherCode, string catalogueNumber)
            : this(ISBN_Prefix.ISBN_979, (int)groupIdentifier, publisherCode, catalogueNumber)
        { }

        private ISBN(ISBN_Prefix prefix, int groupIdentifier, string publisherCode, string catalogueNumber)
        {
            _prefix = prefix;
            _groupIdentifier = groupIdentifier;
            _publisherCode = publisherCode.PadLeft(2, '0');

            int catalogueNumberLength = 
                ISBN_LENGTH 
                - ((short)_prefix).ToString().Length
                - _groupIdentifier.ToString().Length
                - _publisherCode.Length
                - 1; // Check Digit

            _catalogueNumber = catalogueNumberLength > 0 ?
                catalogueNumber.PadLeft(catalogueNumberLength, '0') :
                catalogueNumber;

            StringBuilder sb;
            try
            {
                sb = new StringBuilder($"{(short)_prefix}{_groupIdentifier}{_publisherCode}{_catalogueNumber}X", 
                    ISBN_LENGTH);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ArgumentException(
                    $"Length of ISBN must be exactly {ISBN_LENGTH - 1} (without check digit). ISBN: {ToString()}");
            }

            _checkDigit = CalcCheckDigit(sb);
            sb[sb.Length - 1] = char.Parse(_checkDigit.ToString());
            _value = ulong.Parse(sb.ToString());
        }

        private readonly ISBN_Prefix _prefix;
        public ISBN_Prefix Prefix { get { return _prefix; } }

        private readonly int _groupIdentifier;
        public int GroupIdentifier { get { return _groupIdentifier; } }

        private readonly string _publisherCode;
        public string PublisherCode { get { return _publisherCode; } }

        private readonly string _catalogueNumber;
        public string CatalogueNumber { get { return _catalogueNumber; } }

        private readonly byte _checkDigit;
        public byte CheckDigit { get { return _checkDigit; } }
        
        // Saved in numeric format in order to allow fast comparisons
        private readonly ulong _value;
        public ulong Value { get { return _value; } }

        public override string ToString()
        {
            return
                $"{(short)Prefix}-{GroupIdentifier}-{PublisherCode}-{CatalogueNumber}-{CheckDigit}";
        }

        private static byte CalcCheckDigit(StringBuilder isbn)
        {
            int sum = 0;
            byte digit = 0;

            for (int index = 1; index <= isbn.Length - 1; index++)
            {
                digit = Convert.ToByte(isbn[index - 1]);

                if (index % 2 == 0)
                    sum += digit * 3;
                else
                    sum += digit;
            }

            return (byte)((10 - (sum % 10)) % 10);
        }

        public bool Equals(ISBN other)
        {
            return _value == other?._value;
        }

        public override bool Equals(object obj)
        {
            if (obj is ISBN)
                return Equals((ISBN)obj);
            else
                return false;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public static bool operator ==(ISBN isbn1, ISBN isbn2)
        {
            return isbn1.Equals(isbn2);
        }

        public static bool operator !=(ISBN isbn1, ISBN isbn2)
        {
            return !isbn1.Equals(isbn2);
        }
    }
}
