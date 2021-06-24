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
    }
}
