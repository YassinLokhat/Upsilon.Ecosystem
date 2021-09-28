using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Upsilon.Common.Library
{
    /// <summary>
    /// This class represents a text block.
    /// </summary>
    public class YTextBlock
    {
        /// <summary>
        /// The <c><see cref="YTextBlockSearchConfiguration"/></c> used for the search.
        /// </summary>
        public YTextBlockSearchConfiguration Configuration { get; set; } = null;

        /// <summary>
        /// The index of the begining of the block, including the <c><see cref="YTextBlockSearchConfiguration.BlockStart"/></c> string.
        /// </summary>
        public int StartIndex { get; set; } = -1;

        /// <summary>
        /// The index of the end of the block, including the <c><see cref="YTextBlockSearchConfiguration.BlockEnd"/></c> string.
        /// </summary>
        public int EndIndex { get; set; } = -1;

        /// <summary>
        /// The full text given to the search.
        /// </summary>
        public string Text { get; set; } = null;

        /// <summary>
        /// Get the found text block including the <c><see cref="YTextBlockSearchConfiguration.BlockStart"/></c> string and the <c><see cref="YTextBlockSearchConfiguration.BlockEnd"/></c> string.
        /// </summary>
        public string OuterText
        {
            get
            {
                YDebugTrace.TraceOn();
                
                if (this.Text == null
                   || this.Configuration == null
                   || this.StartIndex == -1
                   || this.EndIndex == -1)
                {
                    return YDebugTrace.TraceOff<string>(null);
                }

                return YDebugTrace.TraceOff(this.Text[this.StartIndex..(this.EndIndex + this.Configuration.BlockEnd.Length)]);
            }
        }

        /// <summary>
        /// Get the found text block without the <c><see cref="YTextBlockSearchConfiguration.BlockStart"/></c> string and the <c><see cref="YTextBlockSearchConfiguration.BlockEnd"/></c> string.
        /// </summary>
        public string InnerText
        {
            get
            {
                YDebugTrace.TraceOn();

                if (this.Text == null
                   || this.Configuration == null
                   || this.StartIndex == -1
                   || this.EndIndex == -1)
                {
                    return YDebugTrace.TraceOff<string>(null);
                }

                return YDebugTrace.TraceOff(this.Text[(this.StartIndex + this.Configuration.BlockStart.Length)..this.EndIndex]);
            }
        }

        internal static string _GetIgnoreStringKey(string[] keys, int length)
        {
            YDebugTrace.TraceOn(new object[] { keys, length });

            if (length < 3)
            {
                return "¤¤";
            }

            string key;
            var random = new Random((int)DateTime.Now.Ticks);

            do
            {
                key = string.Empty;

                for (int i = 0; i < length - 2; i++)
                {
                    key += random.Next(10);
                }
            }
            while (keys.Contains(key));

            return YDebugTrace.TraceOff($"¤{key}¤");
        }
    }

    /// <summary>
    /// Configure the search using <c><see cref="YStringManagement.GetNextTextBlock(string, YTextBlockSearchConfiguration)"/></c> method.
    /// </summary>
    public class YTextBlockSearchConfiguration
    {
        /// <summary>
        /// Indicate the string delimiting the begining of the block.
        /// </summary>
        public string BlockStart { get; set; } = null;

        /// <summary>
        /// Indicate the string delimiting the end of the block.
        /// </summary>
        public string BlockEnd { get; set; } = null;

        /// <summary>
        /// The inline ignore string.
        /// </summary>
        public string InlineIgnore { get; set; } = null;

        /// <summary>
        /// The block ignore starting string.
        /// </summary>
        public string BlockIgnoreStart { get; set; } = null;

        /// <summary>
        /// The block ignore ending string.
        /// </summary>
        public string BlockIgnoreEnd { get; set; } = null;

        /// <summary>
        /// The escape string.
        /// </summary>
        public string Escape { get; set; } = null;

        /// <summary>
        /// The index where the search will start.
        /// </summary>
        public int StartIndex { get; set; } = 0;
    }
}
