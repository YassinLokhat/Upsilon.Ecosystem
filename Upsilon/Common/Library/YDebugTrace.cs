using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Upsilon.Common.Library
{
    /// <summary>
    /// Represent a debug trace.
    /// </summary>
    public class YDebugTrace
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
        }

        /// <summary>
        /// Return the current traces as a string.
        /// </summary>
        /// <param name="level">The indent level.</param>
        /// <returns>The current traces as a string</returns>
        public string ToString(int level = 0)
        {
            string indent = string.Empty.PadLeft(level).Replace(" ", "\t");
            string ret = $"{indent}{this.TraceId}\n" +
                $"{indent}{this.FileName}\n" +
                $"{indent}{this.StartLine}\n" +
                $"{indent}{this.EndLine}\n" +
                $"{indent}{this.CallerMethod}\n" +
                $"{indent}{this.CallerLine}\n" +
                $"{indent}{this.ExecutingMethodeName}\n" +
                $"{indent}{(this.Parent != null ? this.Parent.TraceId : -1)}\n" +
                $"{indent}{this.Parameters.Length}\n" +
                $"{string.Join("", this.Parameters.Select(x => indent + (x != null ? x.SerializeObject() + "\n" : "\n")))}" +
                $"{indent}{(this.Return != null ? this.Return.SerializeObject() : string.Empty)}\n" +
                $"{indent}{this.Traces.Count}\n" +
                $"{string.Join("\n", this.Traces.Select(x => x.ToString(level + 1)))}" +
                $"";

            return ret;
        }
    }
}
