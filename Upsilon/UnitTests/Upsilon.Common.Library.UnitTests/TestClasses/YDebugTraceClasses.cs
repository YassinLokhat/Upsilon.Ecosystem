using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Upsilon.Common.Library.UnitTests.TestClasses
{
    public static class YDebugTraceClasses
    {
        public static void Function1(int param, bool riseException = false)
        {
            YDebugTrace.TraceOn(new object[] { param });
            int local = 10;
            // preprocessing Function2
            YDebugTraceClasses.Function2(param , riseException);
            // postprocessing Function2
            YDebugTrace.TraceOff();
        }

        public static int Function2(int param, bool riseException)
        {
            YDebugTrace.TraceOn(new object[] { param, riseException });
            // processing Function2
            var random = new Random((int)DateTime.Now.Ticks);
            //var n = random.Next(100);

            if (param % 2 == 0)
            {
                // preprocessing Function3
                YDebugTraceClasses.Function3();
                // postprocessing Function3
            }
            else
            {
                // preprocessing Function4
                YDebugTraceClasses.Function4();
                // postprocessing Function4
            }

            if (riseException)
            {
                throw new Exception();
            }

            
            return YDebugTrace.TraceOff(param + 1);
        }

        public static void Function3()
        {
            YDebugTrace.TraceOn();
            // processing Function3
            YDebugTrace.TraceOff();
        }

        public static bool Function4()
        {
            return YDebugTrace.Trace(true);
        }
    }
}
