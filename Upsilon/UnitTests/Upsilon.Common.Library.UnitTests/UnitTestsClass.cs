using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Upsilon.Common.Library.UnitTests
{
    public class UnitTestsClass
    {
        public static bool EnableTrace { get; set; } = true;
        public TestContext TestContext { get; set; }

        [TestInitialize()]
        public void Initialize()
        {
            if (!UnitTestsClass.EnableTrace)
            {
                YDebugTrace.SuspendTracing();
                return;
            }

            YDebugTrace.StartTracing();
        }

        [TestCleanup()]
        public void Cleanup()
        {
            if (!UnitTestsClass.EnableTrace)
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
