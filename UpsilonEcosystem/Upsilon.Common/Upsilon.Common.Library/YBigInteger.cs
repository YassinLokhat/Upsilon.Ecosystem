using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Upsilon.Common.Library
{
    /// <summary>
    /// Enumerate numeric bases
    /// </summary>
    public enum YBase
    {
        /// <summary>
        /// Unknown base
        /// </summary>
        None = 0,
        /// <summary>
        /// Binary base (2)
        /// </summary>
        Binary = 2,
        /// <summary>
        /// Octal base (8)
        /// </summary>
        Octal = 8,
        /// <summary>
        /// Decimal base (10)
        /// </summary>
        Decimal = 10,
        /// <summary>
        /// Hexadecimal base (16)
        /// </summary>
        Hexadecimal = 16,
    }

    public static class YBaseExtensions
    {
        public static short GetBaseNumber(this YBase @base)
        {
            return (short)@base;
        }

        public static string GetAlphabet(this YBase @base)
        {
            return @base switch
            {
                YBase.Binary => "01",
                YBase.Octal => "01234567",
                YBase.Decimal => "0123456789",
                YBase.Hexadecimal => "0123456789ABCDEF",
                _ => string.Empty,
            };
        }

        public static string GetPrefix(this YBase @base)
        {
            return @base switch
            {
                YBase.Binary => "0b",
                YBase.Octal => "0o",
                YBase.Decimal => "0d",
                YBase.Hexadecimal => "0x",
                _ => string.Empty,
            };
        }

        public static byte GetDigitGroup(this YBase @base)
        {
            return @base switch
            {
                YBase.Binary => 8,
                YBase.Octal or YBase.Hexadecimal => 2,
                YBase.Decimal => 1,
                _ => 0,
            };
        }

        public static YBase[] GetBases()
        {
            return YStaticMethods.GetEnumValues<YBase>().Except(new[] { YBase.None }).ToArray();
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
            YBase @base = _getBase(ref strValue);

            YBigInteger number = new(new byte[] { 0 });
            foreach (char c in strValue)
            {
                number = (number * @base.GetBaseNumber()) + @base.GetAlphabet().IndexOf(c);
            }

            this.ByteArray = number.ByteArray;
        }

        public override string ToString()
        {
            return this.ToString(YBase.Decimal);
        }

        public string ToString(YBase @base)
        {
            return @base switch
            {
                YBase.Binary or YBase.Hexadecimal => _toBinaryOrHexadecimalString(@base),
                YBase.Octal or YBase.Decimal => _toOctalOrDecimalString(@base),
                _ => string.Empty,
            };
        }

        private string _toOctalOrDecimalString(YBase @base)
        {
            string strValue = @base.GetPrefix() + "0";

            for (int i = this.ByteArray.Length - 1; i >= 0; i--)
            {
                string tmp = @base.GetPrefix() + Convert.ToString(YBigInteger.InternalBase, @base.GetBaseNumber()).ToUpper();
                strValue = MultiplyString(strValue, tmp);
                tmp = @base.GetPrefix() + Convert.ToString(this.ByteArray[i], @base.GetBaseNumber()).ToUpper();
                strValue = AddString(strValue, tmp);
            }

            strValue = strValue[2..];

            if (strValue.TrimStart('0') != string.Empty)
            {
                strValue = strValue.TrimStart('0');
            }

            return @base.GetPrefix() + strValue;
        }

        private string _toBinaryOrHexadecimalString(YBase @base)
        {
            int digit = @base.GetDigitGroup();

            StringBuilder builder = new();

            string strNumber;

            for (int i = this.ByteArray.Length - 1; i >= 0; i--)
            {
                strNumber = Convert.ToString(this.ByteArray[i], @base.GetBaseNumber()).ToUpper();
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

            for (int i = digit; @base != YBase.Decimal && i < strNumber.Length; i += digit)
            {
                strNumber = strNumber.Insert(i, " ");
                i++;
            }

            return @base.GetPrefix() + strNumber;
        }

        public static string AddString(string strValue1, string strValue2)
        {
            YBase base1 = _getBase(ref strValue1);
            YBase base2 = _getBase(ref strValue2);

            if (base1 != base2)
            {
                throw new Exception($"'{base1.GetPrefix()}{strValue1}' and '{base2.GetPrefix()}{strValue2}' should be in the same base.");
            }

            strValue1 = new(strValue1.Reverse().ToArray());
            strValue2 = new(strValue2.Reverse().ToArray());

            byte[] result = YBigInteger.AddBytes(strValue1.Select(x => (byte)base1.GetAlphabet().IndexOf(x)).ToArray(),
                strValue2.Select(x => (byte)base1.GetAlphabet().IndexOf(x)).ToArray(),
                base1.GetBaseNumber());

            return base1.GetPrefix() + new string(result.Select(x => base1.GetAlphabet()[x]).Reverse().ToArray());
        }

        public static string MultiplyString(string strValue1, string strValue2)
        {
            YBase base1 = _getBase(ref strValue1);
            YBase base2 = _getBase(ref strValue2);

            if (base1 != base2)
            {
                throw new Exception($"'{base1.GetPrefix()}{strValue1}' and '{base2.GetPrefix()}{strValue2}' should be in the same base.");
            }

            strValue1 = new(strValue1.Reverse().ToArray());
            strValue2 = new(strValue2.Reverse().ToArray());

            byte[] result = YBigInteger.MultiplyBytes(strValue1.Select(x => (byte)base1.GetAlphabet().IndexOf(x)).ToArray(),
                strValue2.Select(x => (byte)base1.GetAlphabet().IndexOf(x)).ToArray(),
                base1.GetBaseNumber());

            return base1.GetPrefix() + new string(result.Select(x => base1.GetAlphabet()[x]).Reverse().ToArray());
        }

        private static YBase _getBase(ref string strValue)
        {
            string value = strValue;
            YBase @base = YBaseExtensions.GetBases().Where(x => value.StartsWith(x.GetPrefix())).FirstOrDefault();
            if (@base == YBase.None)
            {
                @base = YBase.Decimal;
                value = @base.GetPrefix() + value;
            }

            if (!Regex.IsMatch(value, $@"{@base.GetPrefix()}[{@base.GetAlphabet()}\s]+$", RegexOptions.IgnoreCase))
            {
                throw new Exception($"'{value}' is not a {@base} number format.");
            }

            strValue = Regex.Replace(value[2..], @"\s", "").ToUpper();

            return @base;
        }

        public static byte[] AddBytes(byte[] value1, byte[] value2, short @base)
        {
            List<byte> result = new();

            short carry = 0;
            for (int i = 0; i < Math.Max(value1.Length, value2.Length); i++)
            {
                byte digit1 = 0;
                if (i < value1.Length)
                {
                    digit1 = value1[i];
                }

                byte digit2 = 0;
                if (i < value2.Length)
                {
                    digit2 = value2[i];
                }

                carry = (short)(digit1 + digit2 + carry);
                result.Add((byte)(carry % @base));
                carry = (short)(carry / @base);
            }

            while (carry != 0)
            {
                result.Add((byte)(carry % @base));
                carry = (short)(carry / @base);
            }

            while (result.Last() == 0 && result.Count > 1)
            {
                result.RemoveAt(result.Count - 1);
            }

            return result.ToArray();
        }

        public static byte[] MultiplyBytes(byte[] value1, byte[] value2, short @base)
        {
            List<byte[]> subResults = new();

            for (int i = 0; i < value2.Length; i++)
            {
                byte digit2 = value2[i];

                int carry = 0;
                List<byte> subResult = new();
                for (int j = 0; j < i; j++)
                {
                    subResult.Add(0);
                }

                for (int j = 0; j < value1.Length; j++)
                {
                    byte digit1 = value1[j];

                    carry = ((digit1 * digit2) + carry);
                    subResult.Add((byte)(carry % @base));
                    carry /= @base;
                }

                while (carry != 0)
                {
                    subResult.Add((byte)(carry % @base));
                    carry /= @base;
                }

                subResults.Add(subResult.ToArray());
            }

            byte[] result = new byte[] { 0 };
            foreach (byte[] subResult in subResults)
            {
                result = YBigInteger.AddBytes(result, subResult, @base);
            }

            return result;
        }

        public static YBigInteger operator +(YBigInteger value1, YBigInteger value2)
        {
            return new YBigInteger(YBigInteger.AddBytes(value1.ByteArray, value2.ByteArray, 0x100));
        }

        public static YBigInteger operator +(YBigInteger value1, long value2)
        {
            return value1 + new YBigInteger(BitConverter.GetBytes(value2));
        }

        public static YBigInteger operator +(long value1, YBigInteger value2)
        {
            return value2 + value1;
        }

        public static YBigInteger operator *(YBigInteger value1, YBigInteger value2)
        {
            return new YBigInteger(YBigInteger.MultiplyBytes(value1.ByteArray, value2.ByteArray, 0x100));
        }

        public static YBigInteger operator *(YBigInteger value1, long value2)
        {
            return value1 * new YBigInteger(BitConverter.GetBytes(value2));
        }

        public static YBigInteger operator *(long value1, YBigInteger value2)
        {
            return value2 * value1;
        }
    }
}
