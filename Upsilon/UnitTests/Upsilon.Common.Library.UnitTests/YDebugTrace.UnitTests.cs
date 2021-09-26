using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Upsilon.Common.Library.UnitTests
{
    [TestClass]
    public class YDebugTrace_UnitTests : UnitTestsClass
    {
        [TestMethod]
        public void Test_01_()
        {
            // Given

            // When
            this.function1(15);

            // Then
            1.Should().Be(0);
        }

        [TestMethod]
        public void Test_02_()
        {
            // Given

            // When
            this.function1(15);

            // Then
            1.Should().Be(1);
        }

        private void function1(int param)
        {
            YDebugTrace.TraceOn(new object[] { param });
            int local = 10;
            // preprocessing function2
            this.function2(local);
            // postprocessing function2
            YDebugTrace.TraceOff(); 
        }

        private int function2(int param)
        {
            YDebugTrace.TraceOn(new object[] { param });
            // processing function2
            var random = new Random((int)DateTime.Now.Ticks);
            var n = random.Next(100);

            if (n % 2 == 0)
            {
                // preprocessing function3
                this.function3();
                // postprocessing function3
            }
            else
            {
                // preprocessing function4
                this.function4();
                // postprocessing function4
            }

            YDebugTrace.TraceOff((object)n);
            return n;
        }

        private void function3()
        {
            YDebugTrace.TraceOn();
            // processing function3
            YDebugTrace.TraceOff();
        }

        private void function4()
        {
            YDebugTrace.TraceOn();
            // processing function4
            YDebugTrace.TraceOff();
        }
    }
}
