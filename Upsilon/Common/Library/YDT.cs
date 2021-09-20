using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Upsilon.Common.Library
{
    /// <summary>
    /// The Upsilon Debug Trace feature class
    /// </summary>
    public static class YDT
    {
        private static readonly string _traceLogFile = "trace.log";
        //private static readonly string _key = "UpsilonEcosystem";
        private static readonly string _key = "";
        private static bool _traceIsPossible
        {
            get
            {
                return File.Exists(YDT._traceLogFile);
            }
        }

        private static YDebugTrace _rootTrace = null;
        private static YDebugTrace _currentTrace = null;

        /// <summary>
        /// Initialize tracing with the solution file directory.
        /// </summary>
        /// <param name="solutionDirectory">The solution file directory.</param>
        public static void InitTrace(string solutionDirectory)
        {
            File.WriteAllText(YDT._traceLogFile, string.Empty);
        }

        /// <summary>
        /// Start tracing block.
        /// </summary>
        /// <param name="sourceMemberParameters">The method parameters.</param>
        /// <param name="sourceFilePath">Leave default.</param>
        /// <param name="sourceLineNumber">Leave default.</param>
        /// <param name="sourceMemberName">Leave default.</param>
        public static void TrOn(object[] sourceMemberParameters = null,
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0,
            [CallerMemberName] string sourceMemberName = "")
        {
            if (!YDT._traceIsPossible)
            {
                return;
            }

            if (sourceMemberParameters == null)
            {
                sourceMemberParameters = Array.Empty<object>();
            }

            if (YDT._rootTrace == null)
            {
                YDT._rootTrace = new YDebugTrace() 
                {
                    TraceId = DateTime.Now.Ticks,
                    FileName = "root", 
                    StartLine = 0,
                    ExecutingMethodeName = "root",
                    Parameters = Array.Empty<object>(),
                };
                YDT._currentTrace = YDT._rootTrace;
            }

            StackTrace stackTrace = new StackTrace(true);
            var frames = stackTrace.GetFrames();
            int callerIndex = 1;

            while (frames[callerIndex - 1].GetMethod().Name != sourceMemberName
                && frames[callerIndex - 1].GetMethod().Name != "get_" + sourceMemberName)
            {
                callerIndex++;
            }

            var caller = frames[callerIndex];

            var trace = new YDebugTrace()
            { 
                TraceId = DateTime.Now.Ticks,
                FileName = YDT._getFilePath(sourceFilePath), 
                StartLine = sourceLineNumber, 
                ExecutingMethodeName = sourceMemberName,
                Parameters = sourceMemberParameters,
                CallerMethod = caller.GetMethod().Name, 
                CallerLine = caller.GetFileLineNumber(), 
            };

            YDT._currentTrace.AddTrace(trace);
            YDT._currentTrace = trace;
        }

        /// <summary>
        /// Stop tracing block.
        /// </summary>
        /// <param name="sourceMemberReturn">The method returned object.</param>
        /// <param name="sourceFilePath">Leave default.</param>
        /// <param name="sourceLineNumber">Leave default.</param>
        /// <param name="sourceMemberName">Leave default.</param>
        public static void TrOff(object sourceMemberReturn = null,
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0,
            [CallerMemberName] string sourceMemberName = "")
        {
            if (!YDT._traceIsPossible)
            {
                return;
            }

            var trace = YDT._currentTrace;
            YDT._currentTrace = YDT._currentTrace.Parent;

            if (trace == null
                || trace.FileName != YDT._getFilePath(sourceFilePath)
                || trace.StartLine > sourceLineNumber
                || trace.ExecutingMethodeName != sourceMemberName)
            {
                throw new Exception("Current Trace off call does not match to a previous Trace on call.");
            }

            trace.Return = sourceMemberReturn;
            trace.EndLine = sourceLineNumber;

            if (YDT._currentTrace == YDT._rootTrace
                && !YDT._rootTrace.Traces.Any(x => !x.IsClosed))
            {
                YDT._rootTrace.EndLine = 0;
            }

            YDT._logTrace();
        }

        /// <summary>
        /// Stop tracing block and return the given value.
        /// </summary>
        /// <typeparam name="T">The type of the value to return.</typeparam>
        /// <param name="sourceMemberReturn">The method returned object.</param>
        /// <param name="sourceFilePath">Leave default.</param>
        /// <param name="sourceLineNumber">Leave default.</param>
        /// <param name="sourceMemberName">Leave default.</param>
        /// <returns>The value given as parameter.</returns>
        public static T RetTrOff<T>(T sourceMemberReturn,
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0,
            [CallerMemberName] string sourceMemberName = "")
        {
            YDT.TrOff(sourceMemberReturn, sourceFilePath, sourceLineNumber + 1, sourceMemberName);

            return sourceMemberReturn;
        }

        /// <summary>
        /// Trace a getter method.
        /// </summary>
        /// <typeparam name="T">The type of the getter.</typeparam>
        /// <param name="ret">The value to return.</param>
        /// <param name="sourceFilePath">Leave default.</param>
        /// <param name="sourceLineNumber">Leave default.</param>
        /// <param name="sourceMemberName">Leave default.</param>
        /// <returns>The value given as parameter.</returns>
        public static T Ret<T>(T ret,
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0,
            [CallerMemberName] string sourceMemberName = "")
        {
            YDT.TrOn(new object[] { ret }, sourceFilePath, sourceLineNumber - 1, sourceMemberName);
            return YDT.RetTrOff(ret, sourceFilePath, sourceLineNumber, sourceMemberName);
        }

        private static string _getFilePath(string sourceFilePath)
        {
            return sourceFilePath[sourceFilePath.IndexOf(@"\UpsilonEcosystem\")..];
        }

        private static void _logTrace()
        {
            File.WriteAllText(YDT._traceLogFile, YDT._rootTrace.ToString().Cipher_Aes(YDT._key));
        }
    }
}
