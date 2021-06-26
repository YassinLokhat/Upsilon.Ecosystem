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
        public BaseList BaseName { get { return (BaseList)this.Alphabet.Length; } }
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
                    Alphabet = "01",
                    Prefix = "0b",
                    DigitGroup = 8,
                }; } }

        public static Base Octal { get {
                return new Base
                {
                    Alphabet = "01234567",
                    Prefix = "0o",
                    DigitGroup = 2,
                }; } }

        public static Base Decimal { get {
                return new Base
                {
                    Alphabet = "0123456789",
                    Prefix = "0d",
                    DigitGroup = 1,
                }; } }

        public static Base Hexadecimal { get {
                return new Base
                {
                    Alphabet = "0123456789ABCDEF",
                    Prefix = "0x",
                    DigitGroup = 2,
                }; } }

        public override string ToString()
        {
            return $"Base name : '{this.BaseName}' ({(int)this.BaseName})\nAlphabet : '{this.Alphabet}'\nPrefix : '{this.Prefix}'\nDigit group : '{this.DigitGroup}'";
        }
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
            Base @base = _getBase(ref strValue);

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
            switch (@base.BaseName)
            {
                case BaseList.Binary:
                case BaseList.Hexadecimal:
                    return _toBinaryOrHexadecimalString(@base);
                case BaseList.Octal:
                case BaseList.Decimal:
                    return _toOctalOrDecimalString(@base);
            }

            return string.Empty;
        }

        private string _toOctalOrDecimalString(Base @base)
        {
            string strValue = @base.Prefix + "0";

            for (int i = this.ByteArray.Length - 1; i >= 0; i--)
            {
                string tmp = @base.Prefix + Convert.ToString(YBigInteger.InternalBase, (int)@base.BaseName).ToUpper();
                strValue = MultiplyString(strValue, tmp);
                tmp = @base.Prefix + Convert.ToString(this.ByteArray[i], (int)@base.BaseName).ToUpper();
                strValue = AddString(strValue, tmp);
            }

            strValue = strValue[2..];

            if (strValue.TrimStart('0') != string.Empty)
            {
                strValue = strValue.TrimStart('0');
            }

            return @base.Prefix + strValue;
        }

        private string _toBinaryOrHexadecimalString(Base @base)
        {
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

        public static string AddString(string strValue1, string strValue2)
        {
            Base base1 = _getBase(ref strValue1);
            Base base2 = _getBase(ref strValue2);

            if (base1.BaseName != base2.BaseName)
            {
                throw new Exception($"'{strValue1}({base1.BaseName})' and '{strValue2}({base2.BaseName})' should be in the same base.");
            }

            string result = string.Empty;
            strValue1 = new(strValue1.Reverse().ToArray());
            strValue2 = new(strValue2.Reverse().ToArray());

            byte carry = 0;

            for (int i = 0; i < Math.Max(strValue1.Length, strValue2.Length); i++)
            {
                byte digit1 = 0;
                if (i < strValue1.Length)
                {
                    digit1 = (byte)base1.Alphabet.IndexOf(strValue1[i]);
                }

                byte digit2 = 0;
                if (i < strValue2.Length)
                {
                    digit2 = (byte)base1.Alphabet.IndexOf(strValue2[i]);
                }

                carry = (byte)(digit1 + digit2 + carry);
                result += base1.Alphabet[carry % (int)base1.BaseName];
                carry = (byte)(carry / (int)base1.BaseName);
            }

            while (carry != 0)
            {
                result += base1.Alphabet[carry % (int)base1.BaseName];
                carry = (byte)(carry / (int)base1.BaseName);
            }

            return base1.Prefix + new string(result.Reverse().ToArray());
        }

        public static string MultiplyString(string strValue1, string strValue2)
        {
            Base base1 = _getBase(ref strValue1);
            Base base2 = _getBase(ref strValue2);

            if (base1.BaseName != base2.BaseName)
            {
                throw new Exception($"'{strValue1}({base1.BaseName})' and '{strValue2}({base2.BaseName})' should be in the same base.");
            }

            strValue1 = new(strValue1.Reverse().ToArray());
            strValue2 = new(strValue2.Reverse().ToArray());
            List<string> subResults = new();

            for (int i = 0; i < strValue2.Length; i++)
            {
                byte digit2 = (byte)base1.Alphabet.IndexOf(strValue2[i]);

                int carry = 0;
                string subResult = string.Empty;
                for (int j = 0; j < i; j++)
                {
                    subResult += "0";
                }

                for (int j = 0; j < strValue1.Length; j++)
                {
                    byte digit1 = (byte)base1.Alphabet.IndexOf(strValue1[j]);

                    carry = (int)((digit1 * digit2) + carry);
                    subResult += base1.Alphabet[carry % (int)base1.BaseName];
                    carry = (int)(carry / (int)base1.BaseName);
                }

                while (carry != 0)
                {
                    subResult += base1.Alphabet[carry % (int)base1.BaseName];
                    carry = (byte)(carry / (int)base1.BaseName);
                }

                subResults.Add(base1.Prefix + new string(subResult.Reverse().ToArray()));
            }

            string result = base1.Prefix + "0";
            foreach (string subResult in subResults)
            {
                result = YBigInteger.AddString(result, subResult);
            }

            return result;
        }

        private static Base _getBase(ref string strValue)
        {
            string value = strValue;

            Base @base = Base.Bases.Where(x => value.StartsWith(x.Prefix)).FirstOrDefault();
            if (@base == null)
            {
                @base = Base.Decimal;
                value = @base.Prefix + value;
            }

            if (!Regex.IsMatch(value, $@"{@base.Prefix}[{@base.Alphabet}\s]+$", RegexOptions.IgnoreCase))
            {
                throw new Exception($"'{value}' is not in a {@base.BaseName}-base number format.");
            }

            strValue = Regex.Replace(value.Substring(2), @"\s", "").ToUpper();

            return @base;
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
    }
}
