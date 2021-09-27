using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Upsilon.Common.Library;

namespace Upsilon.Database.Library.UnitTests
{
    public class YUnitTestsClass
    {
        public static bool EnableTrace { get; set; } = true;
        public TestContext TestContext { get; set; }

        [TestInitialize()]
        public void Initialize()
        {
            if (!YUnitTestsClass.EnableTrace)
            {
                YDebugTrace.SuspendTracing();
                return;
            }

            YDebugTrace.StartTracing();
        }

        [TestCleanup()]
        public void Cleanup()
        {
            if (!YUnitTestsClass.EnableTrace)
            {
                YDebugTrace.ResumeTracing();
                return;
            }

            YDebugTrace.ComputeTraceLog();

            if (TestContext.CurrentTestOutcome == UnitTestOutcome.Passed)
            {
                YDebugTrace.StopTracing();
            }
        }
    }
}
