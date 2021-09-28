using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Upsilon.Common.Library
{
    /// <summary>
    /// The Upsilon Debug Trace feature class
    /// </summary>
    public partial class YDebugTrace
    {
        /// <summary>
        /// An unique identifiant.
        /// </summary>
        public long TraceId { get; set; } = -1;

        /// <summary>
        /// The traced code file.
        /// </summary>
        public string FileName { get; set; } = string.Empty;

        /// <summary>
        /// The traced code start line.
        /// </summary>
        public int StartLine { get; set; } = -1;

        /// <summary>
        /// The traced code end line.
        /// </summary>
        public int EndLine { get; set; } = -1;

        /// <summary>
        /// The traced code caller method.
        /// </summary>
        public string CallerMethod { get; set; } = string.Empty;

        /// <summary>
        /// The traced code caller method line number.
        /// </summary>
        public int CallerLine { get; set; } = -1;

        /// <summary>
        /// The traced code method name.
        /// </summary>
        public string ExecutingMethodeName { get; set; } = string.Empty;

        /// <summary>
        /// The traced code parent.
        /// </summary>
        public YDebugTrace Parent { get; set; } = null;

        /// <summary>
        /// The traced code parameters.
        /// </summary>
        public object[] Parameters { get; set; } = null;
      
        /// <summary>
        /// The traced code return value.
        /// </summary>
        public object Return { get; set; } = null;

        /// <summary>
        /// Check if the current trace is closed.
        /// </summary>
        public bool IsClosed
        {
            get
            {
                return EndLine != -1;
            }
        }

        /// <summary>
        /// Get the log directory path of the current trace.
        /// </summary>
        public string LogDirectoryPath
        {
            get
            {
                var directory = string.Empty;
                var trace = this;

                while (trace != null)
                {
                    directory = Path.Combine(trace.TraceId.ToString(), directory);
                    trace = trace.Parent;
                }

                return Path.Combine(YDebugTrace.TraceLogDirectory, directory);
            }
        }


        /// <summary>
        /// The list of traced codes called by the current trace.
        /// </summary>
        public List<YDebugTrace> Traces { get; set; } = new();

        /// <summary>
        /// Add a trace to the current trace.
        /// </summary>
        /// <param name="trace"></param>
        public void AddTrace(YDebugTrace trace)
        {
            trace.Parent = this;
            this.Traces.Add(trace);
            trace.TraceId = this.Traces.Count;
        }

        /// <summary>
        /// Return the current traces as a string.
        /// </summary>
        /// <param name="level">The indent level.</param>
        /// <returns>The current traces as a string</returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new($"{this.TraceId}\n");

            stringBuilder.Append($"{this.FileName}\n");
            stringBuilder.Append($"{this.StartLine}\n");
            stringBuilder.Append($"{this.EndLine}\n");
            stringBuilder.Append($"{this.CallerMethod}\n");
            stringBuilder.Append($"{this.CallerLine}\n");
            stringBuilder.Append($"{this.ExecutingMethodeName}\n");
            stringBuilder.Append($"{(this.Parent != null ? this.Parent.TraceId : -1)}\n");
            stringBuilder.Append($"{this.Parameters.Length}\n");

            Task<string> serializeParametersTask = Task.Run<string>(_serializeParameters);
            Task<string> serializeReturnTask = Task.Run<string>(_serializeReturn);
            serializeParametersTask.Wait();
            serializeReturnTask.Wait();

            stringBuilder.Append(serializeParametersTask.Result);
            stringBuilder.Append(serializeReturnTask.Result);

            stringBuilder.Append($"{this.Traces.Count}\n");
            stringBuilder.Append(string.Join("\n", this.Traces.Select(x => x.TraceId)));
            stringBuilder.Append($"");

            return stringBuilder.ToString(); ;
        }

        private string _serializeParameters()
        {
            StringBuilder stringBuilder = new();

            foreach (var param in Parameters)
            {
                if (param == null)
                {
                    stringBuilder.Append(string.Empty);
                    continue;
                }

                try
                {
                    stringBuilder.Append(JsonSerializer.Serialize(param) + "\n");
                }
                catch (Exception)
                {
                    stringBuilder.Append(param.ToString() + "\n");
                }
            }

            return stringBuilder.ToString();
        }

        private string _serializeReturn()
        {
            if (Return == null)
            {
                return "\n";
            }

            return JsonSerializer.Serialize(Return) + "\n";
        }

        /// <summary>
        /// Log the current trace.
        /// </summary>
        public void LogTrace()
        {
            var directory = this.LogDirectoryPath;

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(Path.Combine(directory, $"{this.TraceId}.log"), this.ToString());
        }

        /// <summary>
        /// Return the computed log trace from the current trace.
        /// </summary>
        /// <param name="level">The indent level.</param>
        /// <returns>The computed log trace from the current trace.</returns>
        public string ReadLogTrace(int level = 0)
        {
            string logFile = Path.Combine(this.LogDirectoryPath, this.TraceId + ".log");

            if (!File.Exists(logFile))
            {
                this.LogTrace();
            }

            string indent = string.Empty.PadLeft(level).Replace(" ", "\t");

            StringBuilder stringBuilder = new();
            stringBuilder.AppendJoin("", File.ReadAllLines(logFile).Select(x => indent + x + "\n"));

            foreach (var trace in this.Traces)
            {
                stringBuilder.Append(trace.ReadLogTrace(level + 1));
            }

            return stringBuilder.ToString();
        }
    }

    public partial class YDebugTrace
    {
        /// <summary>
        /// The Trace log directory.
        /// </summary>
        public static string TraceLogDirectory
        {
            get { return Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "trace.log"); }
        }

        private static bool _suspendTracing = false;

        private static bool _traceIsPossible
        {
            get
            {
                return Directory.Exists(YDebugTrace.TraceLogDirectory);
            }
        }

        /// <summary>
        /// The root trace.
        /// </summary>
        public static YDebugTrace RootTrace = null;
        private static YDebugTrace _currentTrace = null;

        /// <summary>
        /// Start tracing process.
        /// </summary>
        public static void StartTracing()
        {
            Directory.CreateDirectory(YDebugTrace.TraceLogDirectory);

            YDebugTrace.RootTrace = YDebugTrace._currentTrace = null;
        }

        /// <summary>
        /// Stop tracing process.
        /// </summary>
        public static void StopTracing()
        {
            if (Directory.Exists(YDebugTrace.TraceLogDirectory))
            {
                Directory.Delete(YDebugTrace.TraceLogDirectory, true);
            }

            YDebugTrace.RootTrace = YDebugTrace._currentTrace = null;
        }

        /// <summary>
        /// Suspend tracing process.
        /// </summary>
        public static void SuspendTracing()
        {
            YDebugTrace._suspendTracing = true;
        }

        /// <summary>
        /// Resume tracing process.
        /// </summary>
        public static void ResumeTracing()
        {
            YDebugTrace._suspendTracing = false;
        }

        /// <summary>
        /// Compute the trace log directory to a single file log.
        /// </summary>
        public static void ComputeLogTrace()
        {
            if (!YDebugTrace._traceIsPossible
                || YDebugTrace.RootTrace == null)
            {
                return;
            }

            YDebugTrace.RootTrace.LogTrace();
            File.WriteAllText(Path.Combine(YDebugTrace.TraceLogDirectory, "trace.log"), YDebugTrace.RootTrace.ReadLogTrace());
        }

        /// <summary>
        /// Start tracing block.
        /// </summary>
        /// <param name="sourceMemberParameters">The method parameters.</param>
        /// <param name="sourceFilePath">Leave default.</param>
        /// <param name="sourceLineNumber">Leave default.</param>
        /// <param name="sourceMemberName">Leave default.</param>
        public static void TraceOn(object[] sourceMemberParameters = null,
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0,
            [CallerMemberName] string sourceMemberName = "")
        {
            if (!YDebugTrace._traceIsPossible
                || YDebugTrace._suspendTracing)
            {
                return;
            }

            var stackTrace = new StackTrace(true);
            var frames = stackTrace.GetFrames();
            int callerIndex = 1;

            while (frames[callerIndex - 1].GetMethod().Name != sourceMemberName
                && frames[callerIndex - 1].GetMethod().Name != "get_" + sourceMemberName)
            {
                callerIndex++;
            }

            var caller = frames[callerIndex];

            if (sourceMemberParameters == null)
            {
                sourceMemberParameters = Array.Empty<object>();
            }

            if (YDebugTrace.RootTrace == null)
            {
                YDebugTrace.RootTrace = new YDebugTrace()
                {
                    TraceId = DateTime.Now.Ticks,
                    FileName = "root",
                    StartLine = 0,
                    ExecutingMethodeName = "root",
                    Parameters = Array.Empty<object>(),
                };
                YDebugTrace._currentTrace = YDebugTrace.RootTrace;
            }

            var trace = new YDebugTrace()
            {
                //TraceId = DateTime.Now.Ticks - YDebugTrace._rootTrace.TraceId,
                FileName = YDebugTrace._getFilePath(sourceFilePath),
                StartLine = sourceLineNumber,
                ExecutingMethodeName = sourceMemberName,
                Parameters = sourceMemberParameters,
                CallerMethod = caller.GetMethod().Name,
                CallerLine = caller.GetFileLineNumber(),
            };

            YDebugTrace._currentTrace.AddTrace(trace);
            YDebugTrace._currentTrace = trace;
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
        public static T TraceOff<T>(T sourceMemberReturn,
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0,
            [CallerMemberName] string sourceMemberName = "")
        {
            if (YDebugTrace._traceIsPossible
                && !YDebugTrace._suspendTracing)
            {
                YDebugTrace._traceOff(sourceMemberReturn, sourceFilePath, sourceLineNumber, sourceMemberName);
            }

            return sourceMemberReturn;
        }

        /// <summary>
        /// Stop tracing block and return the given value.
        /// </summary>
        /// <param name="sourceMemberReturn">The method returned object.</param>
        /// <param name="sourceFilePath">Leave default.</param>
        /// <param name="sourceLineNumber">Leave default.</param>
        /// <param name="sourceMemberName">Leave default.</param>
        /// <returns>The value given as parameter.</returns>
        public static object TraceOff(object sourceMemberReturn = null,
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0,
            [CallerMemberName] string sourceMemberName = "")
        {
            if (YDebugTrace._traceIsPossible
                && !YDebugTrace._suspendTracing)
            {
                YDebugTrace._traceOff(sourceMemberReturn, sourceFilePath, sourceLineNumber, sourceMemberName);
            }

            return sourceMemberReturn;
        }

        /// <summary>
        /// Trace a getter method.
        /// </summary>
        /// <typeparam name="T">The type of the getter.</typeparam>
        /// <param name="ret">The value to return.</param>
        /// <param name="sourceMemberParameters">The method parameters.</param>
        /// <param name="sourceFilePath">Leave default.</param>
        /// <param name="sourceLineNumber">Leave default.</param>
        /// <param name="sourceMemberName">Leave default.</param>
        /// <returns>The value given as parameter.</returns>
        public static T Trace<T>(T ret,
            object[] sourceMemberParameters = null,
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0,
            [CallerMemberName] string sourceMemberName = "")
        {
            YDebugTrace.TraceOn(sourceMemberParameters, sourceFilePath, sourceLineNumber, sourceMemberName);
            return YDebugTrace.TraceOff(ret, sourceFilePath, sourceLineNumber, sourceMemberName);
        }

        private static void _traceOff(object sourceMemberReturn, string sourceFilePath, int sourceLineNumber, string sourceMemberName)
        {
            var trace = YDebugTrace._currentTrace;

            while (trace != null
                && (trace.FileName != YDebugTrace._getFilePath(sourceFilePath)
                || trace.StartLine > sourceLineNumber
                || trace.ExecutingMethodeName != sourceMemberName))
            {
                trace = trace.Parent;
            }

            if (trace == null)
            {
                throw new Exception("The current Trace is null or the current Trace off never passed to a Trace on.");
            }

            YDebugTrace._currentTrace = trace.Parent;

            trace.Return = sourceMemberReturn;
            trace.EndLine = sourceLineNumber;

            if (YDebugTrace._currentTrace == YDebugTrace.RootTrace
                && !YDebugTrace.RootTrace.Traces.Any(x => !x.IsClosed))
            {
                YDebugTrace.RootTrace.EndLine = 0;
                YDebugTrace.RootTrace.LogTrace();
            }

            trace.LogTrace();
        }

        private static string _getFilePath(string sourceFilePath)
        {
            return sourceFilePath[sourceFilePath.IndexOf(@"\UpsilonEcosystem\")..];
        }
    }
}
