using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace Upsilon.Common.Library
{
    public enum YVersionFormat
    {
        Simple = 0,
        Extended,
        Full,
    }

    public sealed class YVersion : IComparable
    {
        public int Major { get; private set; } = 0;
        public int Minor { get; private set; } = 0;
        public int Build { get; private set; } = 0;
        public int Revision { get; private set; } = 0;

        public static readonly string Alphabet = "abcdefghijklmnopqrstuvwxyz";

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

        public YVersion(Version version) : this($"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}") { }

        public string ToString(YVersionFormat format)
        {
            switch (format)
            {
                case YVersionFormat.Simple:
                    return String.Format("{0}.{1}{2}", this.Major, this.Minor, this.Build != 0 ? "" + YVersion.Alphabet[(this.Build - 1) % YVersion.Alphabet.Length] : "");
                case YVersionFormat.Extended:
                    return String.Format("{0}.{1}.{2}", this.Major, this.Minor, this.Build);
                case YVersionFormat.Full:
                    return String.Format("{0}.{1}.{2}.{3}", this.Major, this.Minor, this.Build, this.Revision);
                default:
                    return "";
            }
        }

        public override string ToString()
        {
            return this.ToString(YVersionFormat.Full);
        }

        public override int GetHashCode()
        {
            int[] version = new int[4] { Major, Minor, Build, Revision };
            return version.GetHashCode();
        }

        #region Compare Operator
        public static bool operator <(YVersion version1, YVersion version2)
        {
            return Comparison(version1, version2) < 0;
        }

        public static bool operator >(YVersion version1, YVersion version2)
        {
            return Comparison(version1, version2) > 0;
        }

        public static bool operator ==(YVersion version1, YVersion version2)
        {
            return Comparison(version1, version2) == 0;
        }

        public static bool operator !=(YVersion version1, YVersion version2)
        {
            return Comparison(version1, version2) != 0;
        }

        public static bool operator <=(YVersion version1, YVersion version2)
        {
            return Comparison(version1, version2) <= 0;
        }

        public static bool operator >=(YVersion version1, YVersion version2)
        {
            return Comparison(version1, version2) >= 0;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is YVersion)) return false;

            return this == (YVersion)obj;
        }

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

        public int CompareTo(object obj)
        {
            return Comparison(this, (YVersion)obj);
        }
        #endregion
    }
}
