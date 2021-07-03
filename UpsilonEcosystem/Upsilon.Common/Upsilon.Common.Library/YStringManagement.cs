using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Upsilon.Common.Library
{
    /// <summary>
    /// This static class contains the string management functions.
    /// </summary>
    public static class YStringManagement
    {
        /// <summary>
        /// Check if the <c><paramref name="identifiant"/></c> is valid as an identifiant according to the following criterias :
        /// <list type="bullet">
        /// <item><description>It cannot be null or empty.</description></item>
        /// <item><description>It contains only alphanumeric characters and/or underscore character.</description></item>
        /// <item><description>It does not start with a digit number.</description></item>
        /// </list>
        /// </summary>
        /// <param name="identifiant"></param>
        /// <returns>Return <c>true</c> or <c>false</c>.</returns>
        public static bool IsIdentifiant(this string identifiant)
        {
            string specialChars = new(identifiant.Where(x => !char.IsLetterOrDigit(x)).ToArray());
            specialChars = new(specialChars.Where(x => x != '_').ToArray());

            if (String.IsNullOrWhiteSpace(identifiant)
                || specialChars.Length != 0
                || char.IsDigit(identifiant[0]))

            {
                return false;
            }

            return true;
        }

        public static int GetNextClosureOf(this string str, string open, string close, int startIdex = 0)
        {
            int depth = 1;

            startIdex = str.IndexOf(open, startIdex);

            while (startIdex != -1 
                && depth > 0)
            {
                int nextOpen = str.IndexOf(open, startIdex + open.Length);
                int nextClose = str.IndexOf(close, startIdex + open.Length);

                if (nextClose == -1
                    || (nextOpen != -1
                        && nextOpen < nextClose))
                {
                    depth++;
                    startIdex = nextOpen;
                }
                else
                {
                    depth--;
                    startIdex = nextClose;
                }
            }

            return startIdex;
        }
    }
}
