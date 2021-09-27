using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using FluentAssertions;

namespace Upsilon.Common.Library.UnitTests
{
    [TestClass]
    public class YVersion_UnitTests : YUnitTestsClass
    {
        private readonly short loopCount = 1000;

        [TestMethod]
        public void Test_01_FullVersionOK()
        {
            // Given
            Random random = new((int)DateTime.Now.Ticks);
            for (int i = 0; i < loopCount; i++)
            {
                int major = random.Next();
                int minor = random.Next();
                int build = random.Next();
                int revision = random.Next();
                string strVersion = $"{major}.{minor}.{build}.{revision}";

                // When
                YVersion version = new(strVersion);

                // Then
                version.Major.Should().Be(major);
                version.Minor.Should().Be(minor);
                version.Build.Should().Be(build);
                version.Revision.Should().Be(revision);

                version.ToString(YVersionFormat.Extended).Should().Be($"{major}.{minor}.{build}");
                version.ToString(YVersionFormat.Full).Should().Be(strVersion);
                version.ToString("M.m.b.r").Should().Be(strVersion);
            }
        }

        [TestMethod]
        public void Test_02_SimpleVersion1OK()
        {
            // Given
            Random random = new((int)DateTime.Now.Ticks);
            for (int i = 0; i < loopCount; i++)
            {
                int major = random.Next();
                int minor = random.Next();
                string strVersion = $"{major}.{minor}";

                // When
                YVersion version = new(strVersion);

                // Then
                version.Major.Should().Be(major);
                version.Minor.Should().Be(minor);
                version.Build.Should().Be(0);
                version.Revision.Should().Be(0);

                version.ToString(YVersionFormat.Simple).Should().Be(strVersion);
                version.ToString(YVersionFormat.Extended).Should().Be($"{major}.{minor}.0");
                version.ToString(YVersionFormat.Full).Should().Be($"{major}.{minor}.0.0");
            }
        }

        [TestMethod]
        public void Test_03_SimpleVersion2OK()
        {
            // Given
            Random random = new((int)DateTime.Now.Ticks);
            for (int i = 0; i < loopCount; i++)
            {
                int major = random.Next();
                int minor = random.Next();
                int build = random.Next(YVersion.Alphabet.Length) + 1;
                string strVersion = $"{major}.{minor}{YVersion.Alphabet[build - 1]}";

                // When
                YVersion version = new(strVersion);

                // Then
                version.Major.Should().Be(major);
                version.Minor.Should().Be(minor);
                version.Build.Should().Be(build);
                version.Revision.Should().Be(0);

                version.ToString(YVersionFormat.Simple).Should().Be(strVersion);
                version.ToString(YVersionFormat.Extended).Should().Be($"{major}.{minor}.{build}");
                version.ToString(YVersionFormat.Full).Should().Be($"{major}.{minor}.{build}.0");
            }
        }

        [TestMethod]
        public void Test_04_ExtendedVersionOK()
        {
            // Given
            Random random = new((int)DateTime.Now.Ticks);
            for (int i = 0; i < loopCount; i++)
            {
                int major = random.Next();
                int minor = random.Next();
                int build = random.Next();
                string strVersion = $"{major}.{minor}.{build}";

                // When
                YVersion version = new(strVersion);

                // Then
                version.Major.Should().Be(major);
                version.Minor.Should().Be(minor);
                version.Build.Should().Be(build);
                version.Revision.Should().Be(0);

                version.ToString(YVersionFormat.Extended).Should().Be($"{major}.{minor}.{build}");
                version.ToString(YVersionFormat.Full).Should().Be($"{major}.{minor}.{build}.0");
            }
        }
    }
}
