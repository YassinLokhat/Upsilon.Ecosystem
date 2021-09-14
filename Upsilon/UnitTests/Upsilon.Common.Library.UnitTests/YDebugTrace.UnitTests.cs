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
            this.function1();

            // Then
            1.Should().Be(0);
        }

        private void function1()
        {
            YDT.TrOn();
            // preprocessing function2
            this.function2();
            // postprocessing function2
            YDT.TrOff(); 
        }

        private void function2()
        { 
            YDT.TrOn();
            // processing function2
            var random = new Random((int)DateTime.Now.Ticks);

            if (random.Next() % 2 == 0)
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

            YDT.TrOff();
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
