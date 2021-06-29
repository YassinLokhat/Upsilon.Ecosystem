using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Upsilon.Common.Library
{
    /// <summary>
    /// The Upsilon Ecosystem argument parsing engine
    /// </summary>
    public class YArgumentParser
    {
        /// <summary>
        /// Get the list of <c>YArguments</c> parsed by the engine
        /// </summary>
        public List<YArgument> Arguments { get; private set; }
        
        /// <summary>
        /// Get the raw arguments given to the engine
        /// </summary>
        public string[] Args { get; private set; }

        /// <summary>
        /// Get the list of argument modifiers
        /// </summary>
        public static readonly char[] ArgModifiers = new[] { '-', '/' };

        /// <summary>
        /// Create a new argument parser engine from an arguments list
        /// </summary>
        /// <param name="args">The arguments list</param>
        public YArgumentParser(string[] args)
        {
            this.Args = args;
            this.Arguments = new List<YArgument>();

            for (int i = 0; i < this.Args.Length; i++)
            {
                YArgument argument = new()
                {
                    Name = string.Empty,
                };

                int j = i;

                if (_isArgumentModifier(this.Args[i]))
                {
                    j++;
                    argument.Name = this.Args[i].TrimStart(this.Args[i][0]);
                }

                while (j < this.Args.Length
                    && !_isArgumentModifier(this.Args[j]))
                {
                    if (argument.Values == null)
                    {
                        argument.Values = new List<string>();
                    }

                    argument.Values.Add(this.Args[j]);

                    j++;
                }

                this.Arguments.Add(argument);

                i = j - 1;
            }
        }

        /// <summary>
        /// Get a <c>YArguments</c> from the <paramref name="argName"/> or <c>null</c> if the given param is not found.
        /// </summary>
        /// <remarks>Giving string.Empty to the <paramref name="argName"/> param will return the main <c>YArguments</c></remarks>
        /// <param name="argName"></param>
        /// <returns>Returns the <c>YArguments</c> or <c>null</c></returns>
        public YArgument GetArgument(string argName)
        {
            return this.Arguments.Find(x => x.Name == argName);
        }

        /// <summary>
        /// Check if the <paramref name="argName"/> is set.
        /// </summary>
        /// <param name="argName"></param>
        /// <returns>Returns <c>true</c> or <c>false</c></returns>
        public bool HasArgument(string argName)
        {
            return GetArgument(argName) != null;
        }

        /// <summary>
        /// Check if the <paramref name="argName"/> is set as a boolean.
        /// </summary>
        /// <param name="argName"></param>
        /// <returns>Returns <c>true</c> or <c>false</c></returns>
        public bool ArgumentIsSet(string argName)
        {
            YArgument arg = GetArgument(argName);
            return arg != null && arg.IsBoolean;
        }

        /// <summary>
        /// Get the main <c>YArguments</c> or <c>null</c> if not found.
        /// </summary>
        /// <returns>Returns the <c>YArguments</c> or <c>null</c></returns>
        public YArgument GetMainArgument()
        {
            return GetArgument(string.Empty);
        }

        private static bool _isArgumentModifier(string arg)
        {
            return (!string.IsNullOrEmpty(arg) && ArgModifiers.Contains(arg[0]));
        }
    }

    /// <summary>
    /// This class represent an argument
    /// </summary>
    public class YArgument
    {
        /// <summary>
        /// The name of the argument without its modifier
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The values of the argument
        /// </summary>
        public List<string> Values { get; set; } = null;

        /// <summary>
        /// Check if the argument is set as a boolean
        /// </summary>
        public bool IsBoolean
        {
            get
            {
                return Values == null;
            }
        }
    }
}
