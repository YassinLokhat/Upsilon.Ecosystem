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
    public class YDebugTrace_UnitTests
    {
        [TestMethod]
        public void Test_01_()
        {
            // Given
            YDT.InitTrace(MetaHelper.YHelper.GetSolutionDirectory());

            // When
            this.function1(15);

            // Then
            1.Should().Be(0);
        }

        private void function1(int param)
        {
            YDT.TrOn(new object[] { param });
            int local = 10;
            // preprocessing function2
            this.function2(local);
            // postprocessing function2
            YDT.TrOff(); 
        }

        private int function2(int param)
        {
            YDT.TrOn(new object[] { param });
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

            YDT.TrOff(n);
            return n;
        }

        private void function3()
        {
            YDT.TrOn();
            // processing function3
            YDT.TrOff();
        }

        private void function4()
        {
            YDT.TrOn();
            // processing function4
            YDT.TrOff();
        }
    }
}
