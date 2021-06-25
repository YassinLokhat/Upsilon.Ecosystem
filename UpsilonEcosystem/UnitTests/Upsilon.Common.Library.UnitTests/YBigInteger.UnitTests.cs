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
        public void Test_07_AddString()
        {
            Random random = new((int)DateTime.Now.Ticks);
            Base[] bases = new[] { Base.Octal, Base.Decimal };

            foreach (Base @base in bases)
            {
                for (int i = 0; i < LoopCount; i++)
                {
                    // Given
                    long val1 = random.Next();
                    long val2 = random.Next();
                    long sum = val1 + val2;

                    // When
                    string strSum = YBigInteger.AddStr(Convert.ToString(val1, (int)@base.BaseName).TrimStart('0').ToUpper(),
                        Convert.ToString(val2, (int)@base.BaseName).TrimStart('0').ToUpper(), @base);

                    // Then
                    strSum.Should().Be(Convert.ToString(sum, (int)@base.BaseName).TrimStart('0').ToUpper());
                }
            }
        }
     
        [TestMethod]
        public void Test_08_MultiplyString()
        {
            Random random = new((int)DateTime.Now.Ticks);
            Base[] bases = new[] { Base.Octal, Base.Decimal };

            foreach (Base @base in bases)
            {
                for (int i = 0; i < LoopCount; i++)
                {
                    // Given
                    long val1 = random.Next();
                    long val2 = random.Next();
                    long result = val1 * val2;

                    // When
                    string strResult = YBigInteger.MultiplyString(Convert.ToString(val1, (int)@base.BaseName).TrimStart('0').ToUpper(),
                        Convert.ToString(val2, (int)@base.BaseName).TrimStart('0').ToUpper(), @base);

                    // Then
                    strResult.Should().Be(Convert.ToString((long)result, (int)@base.BaseName).TrimStart('0').ToUpper());
                }
            }
        }

        [TestMethod]
        public void Test_09_DecimalBase()
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
                strValue.Should().Be(value.ToString());
            }
        }

        [TestMethod]
        public void Test_10_OctalBase()
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
