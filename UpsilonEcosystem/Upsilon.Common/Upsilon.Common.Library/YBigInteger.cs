using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Upsilon.Common.Library
{
    public enum BaseList
    {
        Binary = 2,
        Octal = 8,
        Decimal = 10,
        Hexadecimal = 16,
    }

    public class Base
    {
        public BaseList BaseName { get; set; }
        public string Alphabet { get; set; }
        public string Prefix { get; set; }
        public int DigitGroup { get; set; }

        public static Base[] Bases { get {
                return new Base[]
                {
                Base.Binary,
                Base.Octal,
                Base.Decimal,
                Base.Hexadecimal,
                }; } }

        public static Base Binary { get {
                return new Base
                {
                    BaseName = BaseList.Binary,
                    Alphabet = "01",
                    Prefix = "0b",
                    DigitGroup = 8,
                }; } }

        public static Base Octal { get {
                return new Base
                {
                    BaseName = BaseList.Octal,
                    Alphabet = "01234567",
                    Prefix = "0o",
                    DigitGroup = 2,
                }; } }

        public static Base Decimal { get {
                return new Base
                {
                    BaseName = BaseList.Decimal,
                    Alphabet = "0123456789",
                    Prefix = "0d",
                    DigitGroup = 1,
                }; } }

        public static Base Hexadecimal { get {
                return new Base
                {
                    BaseName = BaseList.Hexadecimal,
                    Alphabet = "0123456789ABCDEF",
                    Prefix = "0x",
                    DigitGroup = 2,
                }; } }

        public override string ToString()
        {
            return $"Base name : '{this.BaseName}' ({(int)this.BaseName})\nAlphabet : '{this.Alphabet}'\nPrefix : '{this.Prefix}'\nDigit group : '{this.DigitGroup}'";
        }

        //public static bool operator ==(Base base1, Base base2)
        //{
        //    return base1.ToString() == base2.ToString();
        //}
        //
        //public static bool operator !=(Base base1, Base base2)
        //{
        //    return base1.ToString() != base2.ToString();
        //}
    }

    public sealed class YBigInteger
    {
        public byte[] ByteArray { get; private set; } = null;

        public YBigInteger(byte[] byteArray)
        {
            while (byteArray.Last() == 0 && byteArray.Length > 1)
            {
                byteArray = byteArray.Take(byteArray.Length - 1).ToArray();
            }

            this.ByteArray = byteArray;
        }

        public static readonly short InternalBase = 0x100;

        public YBigInteger(string strValue)
        {
            Base @base = Base.Bases.Where(x => strValue.StartsWith(x.Prefix)).FirstOrDefault();
            if (@base.Equals(null))
            {
                @base = Base.Decimal;
                strValue = @base.Prefix + strValue;
            }

            if (!Regex.IsMatch(strValue, $@"{@base.Prefix}[{@base.Alphabet}\s]+$", RegexOptions.IgnoreCase))
            {
                throw new Exception($"'{strValue}' is not in a {@base.BaseName}-base number format.");
            }

            strValue = Regex.Replace(strValue.Substring(2), @"\s", "").ToUpper();

            YBigInteger number = new YBigInteger(new byte[] { 0 });
            foreach (char c in strValue)
            {
                number = (number * (int)@base.BaseName) + @base.Alphabet.IndexOf(c);
            }

            this.ByteArray = number.ByteArray;
        }

        public override string ToString()
        {
            return this.ToString(Base.Decimal);
        }

        public string ToString(Base @base)
        {
            if (@base.BaseName == BaseList.Decimal)
            {
                return @base.ToString();
            }

            int digit = @base.DigitGroup;

            StringBuilder builder = new StringBuilder();

            string strNumber = string.Empty;

            for (int i = this.ByteArray.Length - 1; i >= 0; i--)
            {
                strNumber = Convert.ToString(this.ByteArray[i], (int)@base.BaseName).ToUpper();
                if (strNumber.TrimStart('0') != string.Empty)
                {
                    strNumber = strNumber.TrimStart('0');
                }

                int missingDigit = (((digit - strNumber.Length) % digit) + digit) % digit;

                for (int j = 0; j < missingDigit; j++)
                {
                    strNumber = "0" + strNumber;
                }

                builder.Append(strNumber);
            }

            strNumber = builder.ToString();

            for (int i = digit; @base != Base.Decimal && i < strNumber.Length; i += digit)
            {
                strNumber = strNumber.Insert(i, " ");
                i++;
            }

            return @base.Prefix + strNumber;
        }

        public static YBigInteger operator +(YBigInteger value1, YBigInteger value2)
        {
            byte[] result = new byte[Math.Max(value1.ByteArray.Length, value2.ByteArray.Length) + 1];
            short carry = 0;

            for (int i = 0; i < result.Length; i++)
            {
                carry = (short)((i < value1.ByteArray.Length ? value1.ByteArray[i] : 0)
                    + (i < value2.ByteArray.Length ? value2.ByteArray[i] : 0) 
                    + carry);
                result[i] = (byte)carry;
                carry = (short)(carry / YBigInteger.InternalBase);
            }

            while (result.Last() == 0 && result.Length > 1)
            {
                result = result.Take(result.Length - 1).ToArray();
            }

            return new YBigInteger(result);
        }

        public static YBigInteger operator +(YBigInteger value1, long value2)
        {
            return value1 + new YBigInteger(BitConverter.GetBytes(value2));
        }

        public static YBigInteger operator +(long value1, YBigInteger value2)
        {
            return value2 + new YBigInteger(BitConverter.GetBytes(value1));
        }

        public static YBigInteger operator -(YBigInteger value1, YBigInteger value2)
        {
            return null;
        }

        public static YBigInteger operator *(YBigInteger value1, YBigInteger value2)
        {
            List<byte[]> subResults = new();

            for (int i = 0; i < value2.ByteArray.Length; i++)
            {
                int carry = 0;
                byte[] subResult = new byte[i + 1 + value1.ByteArray.Length];
                for (int j = 0; j < value1.ByteArray.Length; j++)
                {
                    carry = (int)((value1.ByteArray[j] * value2.ByteArray[i]) + carry);
                    subResult[i + j] = (byte)carry;
                    carry = (int)(carry / YBigInteger.InternalBase);
                }
                subResult[subResult.Length - 1] = (byte)carry;

                subResults.Add(subResult);
            }

            byte[] result = new byte[] { 0 };
            foreach (byte[] subResult in subResults)
            {
                result = (new YBigInteger(result) + new YBigInteger(subResult)).ByteArray;
            }

            return new YBigInteger(result);
        }

        public static YBigInteger operator *(YBigInteger value1, long value2)
        {
            return value1 * new YBigInteger(BitConverter.GetBytes(value2));
        }

        public static YBigInteger operator *(long value1, YBigInteger value2)
        {
            return value2 * new YBigInteger(BitConverter.GetBytes(value1));
        }

        public static YBigInteger operator /(YBigInteger value1, YBigInteger value2)
        {
            return null;
        }

        public static YBigInteger operator %(YBigInteger value1, YBigInteger value2)
        {
            return null;
        }
    }
}
