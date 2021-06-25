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
            Base @base = Base.Bases.Where(x => strValue.StartsWith(x.Prefix)).FirstOrDefault();
            if (@base == null)
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
            string strValue = "0";

            for (int i = this.ByteArray.Length - 1; i >= 0; i--)
            {
                strValue = MultiplyString(strValue, Convert.ToString(YBigInteger.InternalBase, (int)@base.BaseName).TrimStart('0').ToUpper(), @base);
                strValue = AddStr(strValue, Convert.ToString(this.ByteArray[i], (int)@base.BaseName).TrimStart('0').ToUpper(), @base);
            }

            if (strValue.TrimStart('0') != string.Empty)
            {
                strValue = strValue.TrimStart('0');
            }
            return strValue;
        }

        public static string AddStr(string s1 ,string s2, Base @base)
        {
            string sum = "";
            s1 = new(s1.Reverse().ToArray());
            s2 = new(s2.Reverse().ToArray());

            byte carry = 0;

            for (int i = 0; i < Math.Max(s1.Length, s2.Length); i++)
            {
                byte digit1 = 0;
                if (i < s1.Length)
                {
                    if (!@base.Alphabet.Contains(s1[i]))
                    {
                        throw new Exception($"'{s1}' is not in a {@base.BaseName} number format.");
                    }

                    digit1 = (byte)@base.Alphabet.IndexOf(s1[i]);
                }

                byte digit2 = 0;
                if (i < s2.Length)
                {
                    if (!@base.Alphabet.Contains(s2[i]))
                    {
                        throw new Exception($"'{s2}' is not in a {@base.BaseName} number format.");
                    }

                    digit2 = (byte)@base.Alphabet.IndexOf(s2[i]);
                }

                carry = (byte)(digit1 + digit2 + carry);
                sum += @base.Alphabet[carry % (int)@base.BaseName];
                carry = (byte)(carry / (int)@base.BaseName);
            }

            while (carry != 0)
            {
                sum += @base.Alphabet[carry % (int)@base.BaseName];
                carry = (byte)(carry / (int)@base.BaseName);
            }

            if (@base.BaseName == BaseList.Octal)
            {
                return @base.Prefix + new string(sum.Reverse().ToArray());
            }

            return new(sum.Reverse().ToArray());
        }

        public static string MultiplyString(string s1, string s2, Base @base)
        {
            s1 = new(s1.Reverse().ToArray());
            s2 = new(s2.Reverse().ToArray());
            List<string> subResults = new();

            for (int i = 0; i < s2.Length; i++)
            {
                if (!@base.Alphabet.Contains(s2[i]))
                {
                    throw new Exception($"'{s2}' is not in a {@base.BaseName} number format.");
                }

                byte digit2 = (byte)@base.Alphabet.IndexOf(s2[i]);

                int carry = 0;
                string subResult = "";
                for (int j = 0; j < i; j++)
                {
                    subResult += "0";
                }

                for (int j = 0; j < s1.Length; j++)
                {
                    if (!@base.Alphabet.Contains(s1[j]))
                    {
                        throw new Exception($"'{s1}' is not in a {@base.BaseName} number format.");
                    }

                    byte digit1 = (byte)@base.Alphabet.IndexOf(s1[j]);

                    carry = (int)((digit1 * digit2) + carry);
                    subResult += @base.Alphabet[carry % (int)@base.BaseName];
                    carry = (int)(carry / (int)@base.BaseName);
                }

                while (carry != 0)
                {
                    subResult += @base.Alphabet[carry % (int)@base.BaseName];
                    carry = (byte)(carry / (int)@base.BaseName);
                }

                subResults.Add(new(subResult.Reverse().ToArray()));
            }

            string result = "";
            foreach (string subResult in subResults)
            {
                result = YBigInteger.AddStr(result, subResult, @base);
            }

            return result;
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
