using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace Upsilon.Common.Library
{
    /// <summary>
    /// Enumerate <c><see cref="YVersion"/></c> to string format.
    /// </summary>
    public enum YVersionFormat
    {
        /// <summary>
        /// <c>X.Yz</c> format where <c>X</c> is the Major, <c>Y</c> is the Minor and <c>z</c> is the Build as a letter.
        /// </summary>
        Simple = 0,
        /// <summary>
        /// <c>X.Y.Z</c> format where <c>X</c> is the Major, <c>Y</c> is the Minor and <c>Z</c> is the Build.
        /// </summary>
        Extended,
        /// <summary>
        /// <c>X.Y.Z.W</c> format where <c>X</c> is the Major, <c>Y</c> is the Minor, <c>Z</c> is the Build and <c>W</c> is the Revision.
        /// </summary>
        Full,
    }

    /// <summary>
    /// This class represents a version.
    /// </summary>
    public sealed class YVersion : IComparable
    {
        /// <summary>
        /// The Major version number.
        /// </summary>
        public int Major { get; private set; } = 0;
        /// <summary>
        /// The Minor version number.
        /// </summary>
        public int Minor { get; private set; } = 0;
        /// <summary>
        /// The Build version number.
        /// </summary>
        public int Build { get; private set; } = 0;
        /// <summary>
        /// The Revision version number.
        /// </summary>
        public int Revision { get; private set; } = 0;

        /// <summary>
        /// The alphabet used for the <c><see cref="YVersionFormat.Simple"/></c> string formating.
        /// </summary>
        public static readonly string Alphabet = "abcdefghijklmnopqrstuvwxyz";

        /// <summary>
        /// Create a new <c><see cref="YVersion"/></c> from the string <c><paramref name="version"/></c>.
        /// </summary>
        /// <param name="version">The string formated version.</param>
        public YVersion(string version)
        {
            string[] info = version.Split('.');
            int major = 0, minor = 0, build = 0, revision = 0;

            if (info.Length > 0)
            {
                _ = int.TryParse(info[0], out major);
            }
            if (info.Length > 2)
            {
                _ = int.TryParse(info[2], out build);
            }
            if (info.Length > 1
                && !int.TryParse(info[1], out minor))
            {
                int.TryParse(info[1][0..^1], out minor);
                build = YVersion.Alphabet.IndexOf(info[1].Last()) + 1;
            }
            if (info.Length > 3)
            {
                _ = int.TryParse(info[3], out revision);
            }

            this.Major = major;
            this.Minor = minor;
            this.Build = build;
            this.Revision = revision;
        }

        /// <summary>
        /// Create a new <c><see cref="YVersion"/></c> from the <c><see cref="Version"/> <paramref name="version"/></c>.
        /// </summary>
        /// <param name="version">The version.</param>
        public YVersion(Version version) : this($"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}") { }

        /// <summary>
        /// Get the string format of the <c><see cref="YVersion"/></c> according to the given <c><paramref name="format"/></c>.
        /// </summary>
        /// <param name="format">The format display.</param>
        /// <returns>Returns the <c><see cref="YVersion"/></c> formated string.</returns>
        public string ToString(YVersionFormat format)
        {
            return format switch
            {
                YVersionFormat.Simple => String.Format("{0}.{1}{2}", this.Major, this.Minor, this.Build != 0 ? "" + YVersion.Alphabet[(this.Build - 1) % YVersion.Alphabet.Length] : ""),
                YVersionFormat.Extended => String.Format("{0}.{1}.{2}", this.Major, this.Minor, this.Build),
                YVersionFormat.Full => String.Format("{0}.{1}.{2}.{3}", this.Major, this.Minor, this.Build, this.Revision),
                _ => "",
            };
        }

        /// <summary>
        /// Get the string format of the <c><see cref="YVersion"/></c> according to the given format.
        /// <list type="bullet">
        ///     <listheader>
        ///         <description>Characters for the format string.</description>
        ///     </listheader>
        ///     <item>
        ///         <description><c>M</c> for Major.</description>
        ///     </item>
        ///     <item>
        ///         <description><c>m</c> for Minor.</description>
        ///     </item>
        ///     <item>
        ///         <description><c>b</c> for Build.</description>
        ///     </item>
        ///     <item>
        ///         <description><c>r</c> for Revision.</description>
        ///     </item>
        /// </list>   
        /// </summary>
        /// <param name="format">The format of the string version.</param>
        /// <returns>Returns the <c><see cref="YVersion"/></c> string in the <c><see cref="YVersionFormat.Full"/></c> format.</returns>
        public string ToString(string format)
        {
            return format
                .Replace("M", Major.ToString())
                .Replace("m", Minor.ToString())
                .Replace("b", Build.ToString())
                .Replace("r", Revision.ToString());
        }

        /// <summary>
        /// Get the string format of the <c><see cref="YVersion"/></c> according to the <c><see cref="YVersionFormat.Full"/></c> format.
        /// </summary>
        /// <returns>Returns the <c><see cref="YVersion"/></c> string in the <c><see cref="YVersionFormat.Full"/></c> format.</returns>
        public override string ToString()
        {
            return this.ToString(YVersionFormat.Full);
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            int[] version = new int[4] { Major, Minor, Build, Revision };
            return version.GetHashCode();
        }

        #region Compare Operator
        /// <summary>
        /// Check if <c><paramref name="version1"/></c> is lower than <c><paramref name="version2"/></c>.
        /// </summary>
        /// <param name="version1">The first version to compare.</param>
        /// <param name="version2">The second version to compare.</param>
        /// <returns>Returns <c>true</c> or <c>false</c>.</returns>
        public static bool operator <(YVersion version1, YVersion version2)
        {
            return Comparison(version1, version2) < 0;
        }

        /// <summary>
        /// Check if <c><paramref name="version1"/></c> is greater than <c><paramref name="version2"/></c>.
        /// </summary>
        /// <param name="version1">The first version to compare.</param>
        /// <param name="version2">The second version to compare.</param>
        /// <returns>Returns <c>true</c> or <c>false</c>.</returns>
        public static bool operator >(YVersion version1, YVersion version2)
        {
            return Comparison(version1, version2) > 0;
        }

        /// <summary>
        /// Check if <c><paramref name="version1"/></c> is equal to <c><paramref name="version2"/></c>.
        /// </summary>
        /// <param name="version1">The first version to compare.</param>
        /// <param name="version2">The second version to compare.</param>
        /// <returns>Returns <c>true</c> or <c>false</c>.</returns>
        public static bool operator ==(YVersion version1, YVersion version2)
        {
            return Comparison(version1, version2) == 0;
        }

        /// <summary>
        /// Check if <c><paramref name="version1"/></c> is not equal to than <c><paramref name="version2"/></c>.
        /// </summary>
        /// <param name="version1">The first version to compare.</param>
        /// <param name="version2">The second version to compare.</param>
        /// <returns>Returns <c>true</c> or <c>false</c>.</returns>
        public static bool operator !=(YVersion version1, YVersion version2)
        {
            return Comparison(version1, version2) != 0;
        }

        /// <summary>
        /// Check if <c><paramref name="version1"/></c> is lower or equal than <c><paramref name="version2"/></c>.
        /// </summary>
        /// <param name="version1">The first version to compare.</param>
        /// <param name="version2">The second version to compare.</param>
        /// <returns>Returns <c>true</c> or <c>false</c>.</returns>
        public static bool operator <=(YVersion version1, YVersion version2)
        {
            return Comparison(version1, version2) <= 0;
        }

        /// <summary>
        /// Check if <c><paramref name="version1"/></c> is greater or equal than <c><paramref name="version2"/></c>.
        /// </summary>
        /// <param name="version1">The first version to compare.</param>
        /// <param name="version2">The second version to compare.</param>
        /// <returns>Returns <c>true</c> or <c>false</c>.</returns>
        public static bool operator >=(YVersion version1, YVersion version2)
        {
            return Comparison(version1, version2) >= 0;
        }

        /// <summary>
        /// Check if the current <c><see cref="YVersion"/></c> equals to <c><paramref name="obj"/></c>.
        /// </summary>
        /// <remarks>If <c><paramref name="obj"/></c> is not a <c><see cref="YVersion"/></c>, this will return <c>false</c>.</remarks>
        /// <param name="obj">The version to compare with.</param>
        /// <returns>Returns <c>true</c> or <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is YVersion)) return false;

            return this == (YVersion)obj;
        }

        /// <summary>
        /// Compare two <c><see cref="YVersion"/></c> and returns the diference between them.
        /// </summary>
        /// <param name="version1">The first version to compare.</param>
        /// <param name="version2">The second version to compare.</param>
        /// <returns>
        /// <para>Returns <c>1</c> if <c><paramref name="version1"/></c> is greater than <c><paramref name="version2"/></c>,</para>
        /// <para><c>-1</c> if <c><paramref name="version1"/></c> is lower than <c><paramref name="version2"/></c>,</para>
        /// <para><c>0</c> if <c><paramref name="version1"/></c> is equal to <c><paramref name="version2"/></c>.</para>
        /// </returns>
        public static int Comparison(YVersion version1, YVersion version2)
        {
            if (version1.Major < version2.Major)
                return -1;
            else if (version1.Major > version2.Major)
                return 1;

            if (version1.Minor < version2.Minor)
                return -1;
            else if (version1.Minor > version2.Minor)
                return 1;

            if (version1.Build < version2.Build)
                return -1;
            else if (version1.Build > version2.Build)
                return 1;

            if (version1.Revision < version2.Revision)
                return -1;
            else if (version1.Revision > version2.Revision)
                return 1;

            return 0;
        }

        /// <summary>
        /// Compare the current <c><see cref="YVersion"/></c> to <c><paramref name="obj"/></c> and returns the diference between them.
        /// </summary>
        /// <exception cref="Exception">If <c><paramref name="obj"/></c> is not a <c><see cref="YVersion"/></c>, an exception will be thrown.</exception>
        /// <param name="obj">The first version to compare.</param>
        /// <returns>
        /// <para>Returns <c>1</c> if the current <c><see cref="YVersion"/></c> is greater than <c><paramref name="obj"/></c>,</para>
        /// <para><c>-1</c> if the current <c><see cref="YVersion"/></c> is lower than <c><paramref name="obj"/></c>,</para>
        /// <para><c>0</c> if the current <c><see cref="YVersion"/></c> is equal to <c><paramref name="obj"/></c>.</para>
        /// </returns>
        public int CompareTo(object obj)
        {
            return Comparison(this, (YVersion)obj);
        }
        #endregion
    }
}
