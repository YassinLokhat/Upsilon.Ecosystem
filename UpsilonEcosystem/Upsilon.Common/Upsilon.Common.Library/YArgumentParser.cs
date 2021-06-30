using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Upsilon.Common.Library
{
    /// <summary>
    /// The Upsilon Ecosystem argument parsing engine.
    /// <seealso cref="YArgument"/>
    /// </summary>
    public class YArgumentParser
    {
        /// <summary>
        /// Get the list of <c><see cref="YArgument"/></c> parsed by the engine.
        /// </summary>
        public List<YArgument> Arguments { get; private set; }
        
        /// <summary>
        /// Get the raw arguments given to the engine.
        /// </summary>
        public string[] Args { get; private set; }

        /// <summary>
        /// Get the list of argument modifiers :
        /// <list type="bullet">
        /// <item><description>the dash '-'</description></item>
        /// <item><description>the slash '/'</description></item>
        /// </list>
        /// </summary>
        public static readonly char[] ArgModifiers = new[] { '-', '/' };

        /// <summary>
        /// Create a new argument parser engine from an argument list.
        /// <example>
        /// <code>
        ///    YArgumentParser argumentParser = new YArgumentParser(new string[]
        ///        { 
        ///            "First value of the main argument",
        ///            "Second value of the main argument",
        ///            "-s",
        ///            "source",
        ///            "/d",
        ///            "destination",
        ///            "-readonly"
        ///        });
        /// </code>
        /// In results, <c>argumentParser</c> will have 4 arguments : 
        /// <list type="bullet">
        /// <item><description>the main argument which has 2 values</description></item>
        /// <item><description><c>s</c> argument which has <c>source</c> as value</description></item>
        /// <item><description><c>d</c> argument which has <c>destination</c> as value</description></item>
        /// <item><description><c>readonly</c> argument which is set as boolean</description></item>
        /// </list>
        /// </example>
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
        /// Get a <c><see cref="YArgument"/></c> from the <paramref name="argName"/> or <c>null</c> if the given param is not found.
        /// <example>
        /// <code>
        ///    YArgumentParser argumentParser = new YArgumentParser(new string[]
        ///        { 
        ///            "First value of the main argument",
        ///            "Second value of the main argument",
        ///            "-s",
        ///            "source",
        ///            "/d",
        ///            "destination",
        ///            "-readonly"
        ///        });
        ///    YArgument argument = argumentParser.GetArgument("s");
        ///    YArgument mainArgument = argumentParser.GetArgument(string.Empty);
        ///    YArgument nullArgument = argumentParser.GetArgument("m");
        /// </code>
        /// In results :
        /// <list type="bullet">
        ///     <item><description><c>argument.Values</c> will be <c>{ "source" }</c></description></item>
        ///     <item><description><c>mainArgument.Values</c> will be <c>{ "First value of the main argument", "Second value of the main argument"}</c></description></item>
        ///     <item><description><c>nullArgument</c> will be <c>null</c></description></item>
        /// </list>
        /// </example>
        /// </summary>
        /// <remarks>Giving string.Empty to the <paramref name="argName"/> param will return the main <c>YArguments</c></remarks>
        /// <param name="argName"></param>
        /// <returns>Returns the <c><see cref="YArgument"/></c> or <c>null</c></returns>
        public YArgument GetArgument(string argName)
        {
            return this.Arguments.Find(x => x.Name == argName);
        }

        /// <summary>
        /// Check if the <paramref name="argName"/> is set.
        /// <example>
        /// <code>
        ///    YArgumentParser argumentParser = new YArgumentParser(new string[]
        ///        { 
        ///            "First value of the main argument",
        ///            "Second value of the main argument",
        ///            "-s",
        ///            "source",
        ///            "/d",
        ///            "destination",
        ///            "-readonly"
        ///        });
        /// </code>
        /// In results :
        /// <list type="bullet">
        ///     <item><description><c>argumentParser.HasArgument(string.Empty)</c> will return <c>true</c> because it has main argument</description></item>
        ///     <item><description><c>argumentParser.HasArgument("s")</c> will return <c>true</c></description></item>
        ///     <item><description><c>argumentParser.HasArgument("readonly")</c> will return <c>true</c></description></item>
        ///     <item><description><c>argumentParser.HasArgument("m")</c> will return <c>false</c></description></item>
        /// </list>
        /// </example>
        /// </summary>
        /// <param name="argName"></param>
        /// <returns>Returns <c>true</c> or <c>false</c></returns>
        public bool HasArgument(string argName)
        {
            return GetArgument(argName) != null;
        }

        /// <summary>
        /// Check if the <paramref name="argName"/> is set as a boolean.
        /// <example>
        /// <code>
        ///    YArgumentParser argumentParser = new YArgumentParser(new string[]
        ///        { 
        ///            "First value of the main argument",
        ///            "Second value of the main argument",
        ///            "-s",
        ///            "source",
        ///            "/d",
        ///            "destination",
        ///            "-readonly"
        ///        });
        /// </code>
        /// In results :
        /// <list type="bullet">
        ///     <item><description><c>argumentParser.ArgumentIsSet(string.Empty)</c> will return <c>false</c> because the main argument cannot be set as boolean</description></item>
        ///     <item><description><c>argumentParser.ArgumentIsSet("s")</c> will return <c>false</c> because this argument is not set as boolean</description></item>
        ///     <item><description><c>argumentParser.ArgumentIsSet("readonly")</c> will return <c>true</c></description></item>
        ///     <item><description><c>argumentParser.ArgumentIsSet("override")</c> will return <c>false</c></description></item>
        /// </list>
        /// </example>
        /// </summary>
        /// <param name="argName"></param>
        /// <returns>Returns <c>true</c> or <c>false</c></returns>
        public bool ArgumentIsSet(string argName)
        {
            YArgument arg = GetArgument(argName);
            return arg != null && arg.IsBoolean;
        }

        /// <summary>
        /// Get the main <c><see cref="YArgument"/></c> or <c>null</c> if not found.
        /// </summary>
        /// <returns>Returns the <c><see cref="YArgument"/></c> or <c>null</c></returns>
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
    /// This class represent an argument parsed by a <c><see cref="YArgumentParser"/></c>.
    /// <seealso cref="YArgumentParser"/>
    /// </summary>
    public class YArgument
    {
        /// <summary>
        /// The name of the argument without its modifier.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The values of the argument.
        /// </summary>
        public List<string> Values { get; set; } = null;

        /// <summary>
        /// Check if the argument is set as a boolean.
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
