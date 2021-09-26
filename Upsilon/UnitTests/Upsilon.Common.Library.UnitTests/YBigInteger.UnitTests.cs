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
    public class YBigInteger_UnitTests : UnitTestsClass
    {
        private readonly int LoopCount = 100;

        [ClassInitialize]
        public static void Inilialize(TestContext testContext)
        {
            UnitTestsClass.EnableTrace = false;
        }

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
                string strNumber = number1.ToString(YBase.Binary);
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
                string strNumber = number1.ToString(YBase.Octal);
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
                string strNumber = number1.ToString(YBase.Decimal);
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
                string strNumber = number1.ToString(YBase.Hexadecimal);
                YBigInteger number2 = new(strNumber);

                // Then
                number1.ByteArray.Should().BeEquivalentTo(number2.ByteArray);
            }
        }

        [TestMethod]
        public void Test_07_AddString_OK()
        {
            Random random = new((int)DateTime.Now.Ticks);

            foreach (YBase @base in YBaseExtensions.GetBases())
            {
                for (int i = 0; i < LoopCount; i++)
                {
                    // Given
                    long value1 = random.Next();
                    long value2 = random.Next();
                    long result = value1 + value2;
                    string strValue1 = @base.GetPrefix() + Convert.ToString(value1, @base.GetBaseNumber()).ToUpper();
                    string strValue2 = @base.GetPrefix() + Convert.ToString(value2, @base.GetBaseNumber()).ToUpper();
                    string strResult1 = @base.GetPrefix() + Convert.ToString(result, @base.GetBaseNumber()).ToUpper();

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
            YBase base1 = YBase.Octal;
            YBase base2 = YBase.Decimal;
            long value1 = random.Next();
            long value2 = random.Next();
            string strValue1 = base1.GetPrefix() + Convert.ToString(value1, base1.GetBaseNumber()).ToUpper();
            string strValue2 = base2.GetPrefix() + Convert.ToString(value2, base2.GetBaseNumber()).ToUpper();

            // When
            Action act = new(() => 
            {
                string strResult = YBigInteger.AddString(strValue1, strValue2);
            });

            // Then
            act.Should().Throw<Exception>().WithMessage($"'{strValue1}' and '{strValue2}' should be in the same base.");
        }
     
        [TestMethod]
        public void Test_09_AddString_KO_ErrorInStringValue()
        {
            Random random = new((int)DateTime.Now.Ticks);

            // Given
            YBase base1 = YBase.Octal;
            YBase base2 = YBase.Decimal;
            long value1 = random.Next();
            long value2 = random.Next();
            string strValue1 = base1.GetPrefix() + Convert.ToString(value1, (int)base1.GetBaseNumber()).ToUpper() + "9";
            string strValue2 = base1.GetPrefix() + Convert.ToString(value2, (int)base2.GetBaseNumber()).ToUpper();

            // When
            Action act = new(() => 
            {
                string strResult = YBigInteger.AddString(strValue1, strValue2);
            });

            // Then
            act.Should().Throw<Exception>().WithMessage($"'{strValue1}' is not a {base1} number format.");
        }
     
        [TestMethod]
        public void Test_10_MultiplyString_OK()
        {
            Random random = new((int)DateTime.Now.Ticks);

            foreach (YBase @base in YBaseExtensions.GetBases())
            {
                for (int i = 0; i < LoopCount; i++)
                {
                    // Given
                    long value1 = random.Next();
                    long value2 = random.Next();
                    long result = value1 * value2;
                    string strVal1 = @base.GetPrefix() + Convert.ToString(value1, @base.GetBaseNumber()).ToUpper();
                    string strVal2 = @base.GetPrefix() + Convert.ToString(value2, @base.GetBaseNumber()).ToUpper();
                    string strResult1 = @base.GetPrefix() + Convert.ToString(result, @base.GetBaseNumber()).ToUpper();

                    // When
                    string strResult2 = YBigInteger.MultiplyString(strVal1, strVal2);

                    // Then
                    strResult2.Should().Be(strResult1);
                }
            }
        }

        [TestMethod]
        public void Test_11_MultiplyString_KO_AddingDifferentBases()
        {
            Random random = new((int)DateTime.Now.Ticks);

            // Given
            YBase base1 = YBase.Octal;
            YBase base2 = YBase.Decimal;
            long value1 = random.Next();
            long value2 = random.Next();
            string strValue1 = base1.GetPrefix() + Convert.ToString(value1, base1.GetBaseNumber()).ToUpper();
            string strValue2 = base2.GetPrefix() + Convert.ToString(value2, base2.GetBaseNumber()).ToUpper();

            // When
            Action act = new(() =>
            {
                string strResult = YBigInteger.MultiplyString(strValue1, strValue2);
            });

            // Then
            act.Should().Throw<Exception>().WithMessage($"'{strValue1}' and '{strValue2}' should be in the same base.");
        }

        [TestMethod]
        public void Test_12_MultiplyString__KO_ErrorInStringValue()
        {
            Random random = new((int)DateTime.Now.Ticks);

            // Given
            YBase base1 = YBase.Octal;
            YBase base2 = YBase.Decimal;
            long value1 = random.Next();
            long value2 = random.Next();
            string strValue1 = base1.GetPrefix() + Convert.ToString(value1, (int)base1.GetBaseNumber()).ToUpper() + "9";
            string strValue2 = base1.GetPrefix() + Convert.ToString(value2, (int)base2.GetBaseNumber()).ToUpper();

            // When
            Action act = new(() =>
            {
                string strResult = YBigInteger.MultiplyString(strValue1, strValue2);
            });

            // Then
            act.Should().Throw<Exception>().WithMessage($"'{strValue1}' is not a {base1} number format.");
        }
    }
}
