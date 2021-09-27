using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using FluentAssertions;
using Upsilon.Common.MetaHelper;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Upsilon.Common.Library.UnitTests
{
    [TestClass]
    public class YStaticMethods_UnitTests : YUnitTestsClass
    {
        [TestMethod]
        public void Test_01_YStaticMethods_SerializeObject_Boolean()
        {
            Random random = new((int)DateTime.Now.Ticks);
            bool toSerialize = random.Next() % 2 == 0;

            // When
            string serialized = toSerialize.SerializeObject();
            bool deserialized = (bool)serialized.DeserializeObject(typeof(bool));

            // Then
            deserialized.Should().Be(toSerialize);
        }

        [TestMethod]
        public void Test_02_YStaticMethods_SerializeObject_Integer()
        {
            // Given
            Random random = new((int)DateTime.Now.Ticks);
            long toSerialize = random.Next();

            // When
            string serialized = toSerialize.SerializeObject();
            long deserialized = (long)serialized.DeserializeObject(typeof(long));

            // Then
            deserialized.Should().Be(toSerialize);
        }

        [TestMethod]
        public void Test_03_YStaticMethods_SerializeObject_Decimal()
        {
            // Given
            Random random = new((int)DateTime.Now.Ticks);
            decimal toSerialize = (decimal)random.NextDouble();

            // When
            string serialized = toSerialize.SerializeObject();
            decimal deserialized = (decimal)serialized.DeserializeObject(typeof(decimal));

            // Then
            deserialized.Should().Be(toSerialize);
        }

        [TestMethod]
        public void Test_04_YStaticMethods_SerializeObject_String()
        {
            // Given
            string toSerialize = YHelper.GetRandomString();

            // When
            string serialized = toSerialize.SerializeObject();
            string deserialized = (string)serialized.DeserializeObject(typeof(string));

            // Then
            deserialized.Should().Be(toSerialize);
        }

        [TestMethod]
        public void Test_05_YStaticMethods_SerializeObject_DateTime()
        {
            // Given
            Random random = new((int)DateTime.Now.Ticks);
            DateTime toSerialize = new(random.Next());

            // When
            string serialized = toSerialize.SerializeObject();
            DateTime deserialized = (DateTime)serialized.DeserializeObject(typeof(DateTime));

            // Then
            deserialized.Should().Be(toSerialize);
        }

        [TestMethod]
        public void Test_06_YStaticMethods_SerializeObject_Raw()
        {
            // Given
            Random random = new((int)DateTime.Now.Ticks);
            byte[] toSerialize = new byte[random.Next(0xFFFF)];
            random.NextBytes(toSerialize);

            // When
            string serialized = toSerialize.SerializeObject();
            byte[] deserialized = (byte[])serialized.DeserializeObject(typeof(byte[]));

            // Then
            deserialized.Should().BeEquivalentTo(toSerialize);
        }

        [TestMethod]
        public void Test_07_YStaticMethods_Copy_OK()
        {
            // Given
            string source = "source";
            string destination = "source";

            if (Directory.Exists(source))
            {
                Directory.Delete(source, true);
            }

            // When
            Action act = new(() =>
            {
                YStaticMethods.Copy(source, destination, false);
            });

            // Then
            act.Should().Throw<Exception>();

            // When
            Directory.CreateDirectory(source);
            File.WriteAllText("source/text.txt", "test");
            YStaticMethods.Copy(source, destination, true);

            // Then
            Directory.Exists("source/source").Should().BeTrue();
            File.Exists("source/source/text.txt").Should().BeTrue();
            File.ReadAllText("source/source/text.txt").Should().Be("test");

            // When
            destination = "destination";
            YStaticMethods.Copy(source, destination, true);

            // Then
            Directory.Exists("destination/source").Should().BeTrue();
            File.Exists("destination/source/text.txt").Should().BeTrue();
            File.ReadAllText("destination/source/text.txt").Should().Be("test");
            Directory.Exists("destination/source/source").Should().BeTrue();
            File.Exists("destination/source/source/text.txt").Should().BeTrue();
            File.ReadAllText("destination/source/source/text.txt").Should().Be("test");

            // When
            act = new(() =>
            {
                YStaticMethods.Copy(source, destination, false);
            });

            // Then
            act.Should().Throw<Exception>();

            // Finaly
            Directory.Delete("source", true);
            Directory.Delete("destination", true);
        }

        [TestMethod]
        public void Test_08_YStaticMethods_GetCommonRootDirectory_Directories_OK()
        {
            // Given
            var directories = new string[] 
            {
                @"C:\Directory1\Directory2\Directory3",
                @"C:\Directory1\Directory2\Directory3\Directory4",
                @"C:\Directory1\Directory2\",
            };

            // When
            string root = YStaticMethods.GetCommonRootDirectory(directories, '\\');

            // Then
            root.Should().Be(@"C:\Directory1\Directory2");
        }

        [TestMethod]
        public void Test_09_YStaticMethods_GetCommonRootDirectory_Directories_KO()
        {
            // Given
            var directories = new string[] 
            {
                @"C:\Directory1\Directory2\Directory3",
                @"D:\Directory1\Directory2\Directory3\Directory4",
                @"C:\Directory1\Directory2\",
            };

            // When
            string root = YStaticMethods.GetCommonRootDirectory(directories, '\\');

            // Then
            root.Should().Be(string.Empty);
        }

        [TestMethod]
        public void Test_10_YStaticMethods_GetCommonRootDirectory_Urls_OK()
        {
            // Given
            var directories = new string[] 
            {
                "http://www.test.te/Directory1/Directory2/Directory3",
                "http://www.test.te/Directory1/Directory2/Directory3/Directory4",
                "http://www.test.te/Directory1/Directory2",
            };

            // When
            string root = YStaticMethods.GetCommonRootDirectory(directories, '/');

            // Then
            root.Should().Be(@"http://www.test.te/Directory1/Directory2");
        }

        [TestMethod]
        public void Test_11_YStaticMethods_GetCommonRootDirectory_OnlyOneElement_OK()
        {
            // Given
            var directories = new string[] 
            {
                "http://www.test.te/Directory1/Directory2",
            };

            // When
            string root = YStaticMethods.GetCommonRootDirectory(directories, '/');

            // Then
            root.Should().Be(@"http://www.test.te/Directory1/Directory2");
        }

        [TestMethod]
        public void Test_12_YStaticMethods_CopyFrom_YVersion_OK()
        {
            // Given
            Random random = new Random((int)DateTime.Now.Ticks);
            
            // When
            YVersion soure = new YVersion($"{random.Next(0, 256)}.{random.Next(0, 256)}.{random.Next(0, 256)}.1");
            YVersion destination = new YVersion($"{random.Next(0, 256)}.{random.Next(0, 256)}.{random.Next(0, 256)}");

            // Then
            (soure == destination).Should().BeFalse();
            soure.Revision.Should().Be(1);
            destination.Revision.Should().Be(0);

            // When
            destination.CopyFrom(soure);

            // Then
            (soure == destination).Should().BeTrue();
            soure.Revision.Should().Be(1);
            destination.Revision.Should().Be(1);
        }

        [TestMethod]
        public void Test_12_YStaticMethods_CopyFrom_YAssembly_OK()
        {
            // Given
            YHelperConfiguration configuration = new()
            {
                Reference = "202108091545",
                Extention = "json",
                Directory = YUnitTestFilesDirectory.Files,
            };

            var deployedList = File.ReadAllText(YHelper.GetTestFilePath(configuration, false, true)).DeserializeObject<Dictionary<string, List<YAssembly>>>();
            var assembly = deployedList["Upsilon.Common.Library"][0].Clone();
            assembly.Dependencies = new[] { new YDependency() { Name = assembly.Name, MaximalVersion = assembly.Version, MinimalVersion = assembly.Version } };

            // When
            var asm = deployedList[assembly.Name].Find(x => x.Name == assembly.Name && x.YVersion == assembly.YVersion);

            // Then
            asm.Dependencies.Length.Should().Be(0);

            // When
            asm.CopyFrom(assembly);

            // Then
            asm.Dependencies.Length.Should().Be(1);
            asm.Dependencies[0].Name.Should().Be(assembly.Name);
            asm.Dependencies[0].MaximalVersion.Should().Be(assembly.Version);
            asm.Dependencies[0].MinimalVersion.Should().Be(assembly.Version);
        }

        [TestMethod]
        public void Test_13_YStaticMethods_TakeElementFrom_OK()
        {
            // Given
            var array = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            // When
            var subArray1 = array.TakeElementFrom(2, 5).ToArray();
            var subArray2 = array.TakeElementFrom(2).ToArray();

            // Then
            subArray1.Should().BeEquivalentTo(new int[] { 2, 3, 4, 5, 6 });
            subArray2.Should().BeEquivalentTo(new int[] { 2, 3, 4, 5, 6, 7, 8, 9 });
        }

        [TestMethod]
        public void Test_14_YStaticMethods_TakeElementFrom_StartIndex_K0()
        {
            // Given
            var array = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            // When
            var act = new Action(() =>
            {
                var subArray = array.TakeElementFrom(50, 5).ToArray();
            });

            // Then
            act.Should().Throw<Exception>();
        }

        [TestMethod]
        public void Test_15_YStaticMethods_TakeElementFrom_Count_K0()
        {
            // Given
            var array = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            // When
            var act = new Action(() =>
            {
                var subArray = array.TakeElementFrom(5, 50).ToArray();
            });

            // Then
            act.Should().Throw<Exception>();
        }
    }
}
