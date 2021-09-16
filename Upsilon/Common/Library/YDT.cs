using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
                return File.Exists(YDT._traceLogFile)
                    && Directory.Exists(File.ReadAllText(YDT._traceLogFile).Uncipher_Aes(YDT._key).Split('\n').FirstOrDefault());
            }
        }

        private static _Trace _rootTrace = null;
        private static _Trace _currentTrace = null;

        /// <summary>
        /// Initialize tracing with the solution file directory.
        /// </summary>
        /// <param name="solutionDirectory">The solution file directory.</param>
        public static void InitTrace(string solutionDirectory)
        {
            File.WriteAllText(YDT._traceLogFile, solutionDirectory.Cipher_Aes(YDT._key));
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
                YDT._rootTrace = new _Trace() 
                { 
                    FileName = YDT._getFilePath(sourceFilePath), 
                    StartLine = sourceLineNumber, 
                    ExecutingMethodeName = sourceMemberName,
                    Parameters = sourceMemberParameters,
                };
                YDT._currentTrace = YDT._rootTrace;
            }

            StackTrace stackTrace = new StackTrace(true);
            var frames = stackTrace.GetFrames();
            int callerIndex = 1;

            while (frames[callerIndex - 1].GetMethod().Name != sourceMemberName)
            {
                callerIndex++;
            }

            var caller = frames[callerIndex];

            var trace = new _Trace()
            { 
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

            YDT._logTrace(trace);
        }

        private static string _getFilePath(string sourceFilePath)
        {
            var absolutePath = File.ReadAllText(YDT._traceLogFile).Uncipher_Aes(YDT._key).Split('\n').FirstOrDefault();
            do
            {
                sourceFilePath = sourceFilePath.Substring(sourceFilePath.IndexOf(@"\") + 1);
            }
            while (!File.Exists(Path.Combine(absolutePath, sourceFilePath)));

            return Path.Combine(absolutePath, sourceFilePath);
        }

        private static void _logTrace(_Trace trace)
        {
            var header = File.ReadAllText(YDT._traceLogFile).Uncipher_Aes(YDT._key).Split('\n').FirstOrDefault();
            
            File.WriteAllText(YDT._traceLogFile, header + "\n" + trace.ToString().Cipher_Aes(YDT._key));
        }
    }

    internal class _Trace
    {
        public string FileName { get; set; } = string.Empty;
        public int StartLine { get; set; } = -1;
        public int EndLine { get; set; } = -1;
        public string CallerMethod { get; set; } = string.Empty;
        public int CallerLine { get; set; } = -1;
        public string ExecutingMethodeName { get; set; } = string.Empty;
        public object[] Parameters { get; set; } = null;
        public object Return { get; set; } = null;

        public bool IsClosed
        {
            get
            {
                return EndLine != -1;
            }
        }

        public List<_Trace> Traces { get; set; } = new();
        public _Trace Parent { get; set; } = null;

        public void AddTrace(_Trace trace)
        {
            trace.Parent = this;
            this.Traces.Add(trace);
        }

        public override string ToString()
        {
            if (!this.IsClosed)
            {
                return string.Empty;
            }

            var trace = new List<string> { $"\n\"{FileName}\" [{StartLine + 1} - {EndLine - 1}]" };
            trace.Add($"Called in {CallerMethod} line {CallerLine} :");
            trace.Add($"{ExecutingMethodeName}");
            trace.Add("(");
            trace.AddRange(Parameters.Select((x, i) => $"\t{x.SerializeObject()}"));
            trace.Add(")");
            trace.Add("{");
            trace.AddRange(File.ReadLines(FileName).TakeElementFrom(StartLine, EndLine - StartLine - 1).Select((x, i) => $"{StartLine + i + 1}\t{x}"));
            trace.Add($"\tReturned {(Return != null ? Return.SerializeObject() : "null")}");
            trace.Add("}");
            
            trace.AddRange(this.Traces.Select(x => x.ToString()));

            return string.Join("\n", trace);
        }
    }
}
