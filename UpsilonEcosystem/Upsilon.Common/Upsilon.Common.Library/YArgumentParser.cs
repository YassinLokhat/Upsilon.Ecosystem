using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Upsilon.Common.Library
{
    public class YArgumentParser
    {
        public List<YArgument> Arguments { get; private set; }
        public string[] Args { get; private set; }

        private static readonly char[] ArgModifiers = new[] { '-', '/' };

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

                if (IsArgumentModifier(this.Args[i]))
                {
                    j++;
                    argument.Name = this.Args[i].TrimStart(this.Args[i][0]);
                }

                while (j < this.Args.Length
                    && !IsArgumentModifier(this.Args[j]))
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

        public YArgument GetArgument(string argName)
        {
            return this.Arguments.Find(x => x.Name == argName);
        }

        public bool HasArgument(string argName)
        {
            return GetArgument(argName) != null;
        }

        public bool ArgumentIsSet(string argName)
        {
            YArgument arg = GetArgument(argName);
            return arg != null && arg.IsBoolean;
        }

        public YArgument GetMainArgument()
        {
            return GetArgument(string.Empty);
        }

        public static bool IsArgumentModifier(string arg)
        {
            return (!string.IsNullOrEmpty(arg) && ArgModifiers.Contains(arg[0]));
        }
    }

    public class YArgument
    {
        public string Name { get; set; }
        public List<string> Values { get; set; } = null;
        public bool IsBoolean
        {
            get
            {
                return Values == null;
            }
        }
    }
}
