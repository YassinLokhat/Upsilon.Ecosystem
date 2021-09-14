using System;
using System.Collections.Generic;
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
        private static Stack<int> _startIndex = new();
        private static bool _traceIsPossible
        {
            get
            {
                return File.Exists(YDT._traceLogFile)
                    && Directory.Exists(File.ReadAllText(YDT._traceLogFile).Uncipher_Aes(YDT._key).Split('\n').FirstOrDefault());
            }
        }

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
        /// <param name="sourceLineNumber">Leave default.</param>
        public static void TrOn(
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!YDT._traceIsPossible)
            {
                return;
            }

            YDT._startIndex.Push(sourceLineNumber);
        }

        /// <summary>
        /// Stop tracing block.
        /// </summary>
        /// <param name="sourceFilePath">Leave default.</param>
        /// <param name="sourceLineNumber">Leave default.</param>
        /// <param name="sourceMemberName">Leave default.</param>
        public static void TrOff(
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0,
            [CallerMemberName] string sourceMemberName = "")
        {
            if (!YDT._traceIsPossible)
            {
                return;
            }

            var startIndex = YDT._startIndex.Pop();
            sourceFilePath = YDT._getFilePath(sourceFilePath);
            var trace = new List<string> { $"\n/// \"{sourceFilePath}\" [{startIndex + 1} - {sourceLineNumber - 1}] ({sourceMemberName})" };
            trace.AddRange(File.ReadLines(sourceFilePath).TakeElementFrom(startIndex, sourceLineNumber - startIndex - 1));

            var tmp = File.ReadLines(sourceFilePath).ToArray();

            YDT._logTrace(trace.ToArray());
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

        private static void _logTrace(string[] trace)
        {
            var traces = File.ReadAllText(YDT._traceLogFile).Uncipher_Aes(YDT._key).Split('\n').ToList();
            traces.AddRange(trace);
            
            File.WriteAllText(YDT._traceLogFile, string.Join("\n", traces).Cipher_Aes(YDT._key));
        }
    }
}
