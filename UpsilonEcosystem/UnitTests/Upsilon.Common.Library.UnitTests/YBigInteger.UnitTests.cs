using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Upsilon.Common.Library.UnitTests
{
    [TestClass]
    public class YBigInteger_UnitTests
    {
        private readonly int LoopCount = 1000;

        [TestMethod]
        public void Test_01_Sum()
        {
            Random random = new((int)DateTime.Now.Ticks);

            for (int i = 0; i < LoopCount; i++)
            {
                // Given
                long val1 = random.Next();
                long val2 = random.Next();
                YBigInteger number1 = new(BitConverter.GetBytes(val1));
                YBigInteger number2 = new(BitConverter.GetBytes(val2));
                YBigInteger number3 = new(BitConverter.GetBytes(val1 + val2));

                // When
                YBigInteger sum = number1 + number2;

                // Then
                sum.ByteArray.Should().BeEquivalentTo(number3.ByteArray);
            }
        }

        [TestMethod]
        public void Test_02_Multiply()
        {
            Random random = new((int)DateTime.Now.Ticks);

            for (int i = 0; i < LoopCount; i++)
            {
                // Given
                long val1 = random.Next();
                long val2 = random.Next();
                YBigInteger number1 = new(BitConverter.GetBytes(val1));
                YBigInteger number2 = new(BitConverter.GetBytes(val2));
                YBigInteger number3 = new(BitConverter.GetBytes(val1 * val2));
                
                // When
                YBigInteger sum = number1 * number2;
                
                // Then
                sum.ByteArray.Should().BeEquivalentTo(number3.ByteArray);
            }
        }

        [TestMethod]
        public void Test_03_ToString_Binary()
        {
            Random random = new((int)DateTime.Now.Ticks);

            for (int i = 0; i < LoopCount; i++)
            {
                // Given
                int count = random.Next(20) + 1;
                byte[] bytes = new byte[count];
                random.NextBytes(bytes);

                // When
                YBigInteger number1 = new(bytes);
                string strNumber = number1.ToString(Base.Binary);
                YBigInteger number2 = new(strNumber);

                // Then
                number1.ByteArray.Should().BeEquivalentTo(number2.ByteArray);
            }
        }

        [TestMethod]
        public void Test_04_ToString_Octal()
        {
            Random random = new((int)DateTime.Now.Ticks);

            for (int i = 0; i < LoopCount; i++)
            {
                // Given
                int count = random.Next(20) + 1;
                byte[] bytes = new byte[count];
                random.NextBytes(bytes);

                // When
                YBigInteger number1 = new(bytes);
                string strNumber = number1.ToString(Base.Octal);
                YBigInteger number2 = new(strNumber);

                // Then
                number1.ByteArray.Should().BeEquivalentTo(number2.ByteArray);
            }
        }

        [TestMethod]
        public void Test_05_ToString_Decimal()
        {
            Random random = new((int)DateTime.Now.Ticks);

            for (int i = 0; i < LoopCount; i++)
            {
                // Given
                int count = random.Next(20) + 1;
                byte[] bytes = new byte[count];
                random.NextBytes(bytes);

                // When
                YBigInteger number1 = new(bytes);
                string strNumber = number1.ToString(Base.Decimal);
                YBigInteger number2 = new(strNumber);

                // Then
                number1.ByteArray.Should().BeEquivalentTo(number2.ByteArray);
            }
        }

        [TestMethod]
        public void Test_06_ToString_Hexadecimal()
        {
            Random random = new((int)DateTime.Now.Ticks);

            for (int i = 0; i < LoopCount; i++)
            {
                // Given
                int count = random.Next(20) + 1;
                byte[] bytes = new byte[count];
                random.NextBytes(bytes);

                // When
                YBigInteger number1 = new(bytes);
                string strNumber = number1.ToString(Base.Hexadecimal);
                YBigInteger number2 = new(strNumber);

                // Then
                number1.ByteArray.Should().BeEquivalentTo(number2.ByteArray);
            }
        }

        [TestMethod]
        public void Test_07_AddString_OK()
        {
            Random random = new((int)DateTime.Now.Ticks);

            foreach (Base @base in Base.Bases)
            {
                for (int i = 0; i < LoopCount; i++)
                {
                    // Given
                    long value1 = random.Next();
                    long value2 = random.Next();
                    long result = value1 + value2;
                    string strValue1 = @base.Prefix + Convert.ToString(value1, (int)@base.BaseName).ToUpper();
                    string strValue2 = @base.Prefix + Convert.ToString(value2, (int)@base.BaseName).ToUpper();
                    string strResult1 = @base.Prefix + Convert.ToString(result, (int)@base.BaseName).ToUpper();

                    // When
                    string strResult2 = YBigInteger.AddString(strValue1, strValue2);

                    // Then
                    strResult2.Should().Be(strResult1);
                }
            }
        }
     
        [TestMethod]
        public void Test_08_AddString_KO_AddingDifferentBases()
        {
            Random random = new((int)DateTime.Now.Ticks);

            // Given
            Base base1 = Base.Octal;
            Base base2 = Base.Decimal;
            long value1 = random.Next();
            long value2 = random.Next();
            string strValue1 = base1.Prefix + Convert.ToString(value1, (int)base1.BaseName).ToUpper();
            string strValue2 = base2.Prefix + Convert.ToString(value2, (int)base2.BaseName).ToUpper();

            // When
            Action act = new(() => 
            {
                string strResult = YBigInteger.AddString(strValue1, strValue2);
            });

            // Then
            act.Should().Throw<Exception>().WithMessage($"'{strValue1[2..]}({base1.BaseName})' and '{strValue2[2..]}({base2.BaseName})' should be in the same base.");
        }
     
        [TestMethod]
        public void Test_09_AddString_KO_ErrorInStringValue()
        {
            Random random = new((int)DateTime.Now.Ticks);

            // Given
            Base base1 = Base.Octal;
            Base base2 = Base.Decimal;
            long value1 = random.Next();
            long value2 = random.Next();
            string strValue1 = base1.Prefix + Convert.ToString(value1, (int)base1.BaseName).ToUpper() + "9";
            string strValue2 = base1.Prefix + Convert.ToString(value2, (int)base2.BaseName).ToUpper();

            // When
            Action act = new(() => 
            {
                string strResult = YBigInteger.AddString(strValue1, strValue2);
            });

            // Then
            act.Should().Throw<Exception>().WithMessage($"'{strValue1}' is not in a {base1.BaseName}-base number format.");
        }
     
        [TestMethod]
        public void Test_10_MultiplyString()
        {
            Random random = new((int)DateTime.Now.Ticks);
            Base[] bases = new[] { Base.Octal, Base.Decimal };

            foreach (Base @base in bases)
            {
                for (int i = 0; i < LoopCount; i++)
                {
                    // Given
                    long value1 = random.Next();
                    long value2 = random.Next();
                    long result = value1 * value2;
                    string strVal1 = @base.Prefix + Convert.ToString(value1, (int)@base.BaseName).ToUpper();
                    string strVal2 = @base.Prefix + Convert.ToString(value2, (int)@base.BaseName).ToUpper();
                    string strResult1 = @base.Prefix + Convert.ToString(result, (int)@base.BaseName).ToUpper();

                    // When
                    string strResult2 = YBigInteger.MultiplyString(strVal1, strVal2);

                    // Then
                    strResult2.Should().Be(strResult1);
                }
            }
        }

        [TestMethod]
        public void Test_11_DecimalBase()
        {
            Random random = new((int)DateTime.Now.Ticks);

            for (int i = 0; i < LoopCount; i++)
            {
                // Given
                long value = random.Next();
                YBigInteger bigInteger = new(value.ToString());

                // When
                string strValue = bigInteger.ToString(Base.Decimal);

                // Then
                strValue.Should().Be(Base.Decimal.Prefix + value.ToString());
            }
        }

        [TestMethod]
        public void Test_12_OctalBase()
        {
            Random random = new((int)DateTime.Now.Ticks);

            for (int i = 0; i < LoopCount; i++)
            {
                // Given
                long value = random.Next();
                YBigInteger bigInteger = new(value.ToString());

                // When
                string strValue = bigInteger.ToString(Base.Octal);

                // Then
                strValue.Should().Be(Base.Octal.Prefix + Convert.ToString(value, (int)Base.Octal.BaseName).TrimStart('0').ToUpper());
            }
        }
    }
}
