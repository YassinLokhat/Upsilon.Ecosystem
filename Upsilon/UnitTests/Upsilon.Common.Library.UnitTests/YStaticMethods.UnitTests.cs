﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using FluentAssertions;
using Upsilon.Common.MetaHelper;
using System.IO;

namespace Upsilon.Common.Library.UnitTests
{
    [TestClass]
    public class YStaticMethods_UnitTests
    {
        [TestMethod]
        public void Test_01_StaticMethods_SerializeObject_Boolean()
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
        public void Test_02_StaticMethods_SerializeObject_Integer()
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
        public void Test_03_StaticMethods_SerializeObject_Decimal()
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
        public void Test_04_StaticMethods_SerializeObject_String()
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
        public void Test_05_StaticMethods_SerializeObject_DateTime()
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
        public void Test_06_StaticMethods_SerializeObject_Raw()
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
        public void Test_07_StaticMethods_Copy_OK()
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
    }
}
