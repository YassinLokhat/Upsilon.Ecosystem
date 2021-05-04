using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using FluentAssertions;

namespace Upsilon.Common.Library.UnitTests
{
    [TestClass]
    class ArgumentParser_UnitTests
    {
        [TestClass]
        public class StaticMethods_UnitTests
        {
            [TestMethod]
            public void Test_01_ArgumentParser_GetArgument_OK()
            {
                // Given
                string[] args = { "/n", "Variable", "--values", "Test1", "test2", "//present" };

                // When
                ArgumentParser argParser = new ArgumentParser(args);
                Argument variable = argParser.GetArgument("n");
                Argument values = argParser.GetArgument("values");
                Argument present = argParser.GetArgument("present");

                // Then
                argParser.HasArgument("present").Should().BeTrue();
                argParser.ArgumentIsSet("present").Should().BeTrue();
                argParser.HasArgument("n").Should().BeTrue();
                argParser.ArgumentIsSet("n").Should().BeFalse();
                argParser.HasArgument("test").Should().BeFalse();
                argParser.ArgumentIsSet("test").Should().BeFalse();

                variable.Name.Should().Be("n");
                variable.Values.Should().BeEquivalentTo(new string[] { "Variable" });
                variable.IsBoolean.Should().BeFalse();
                values.Name.Should().Be("values");
                values.Values.Should().BeEquivalentTo(new string[] { "Test1", "test2" });
                values.IsBoolean.Should().BeFalse();
                present.IsBoolean.Should().BeTrue();
                present.Name.Should().Be("present");
                present.IsBoolean.Should().BeTrue();
            }

            [TestMethod]
            public void Test_02_ArgumentParser_WithMain()
            {
                // Given
                string[] args = { "Main", "/n", "Variable", "--values", "Test1", "test2", "/p" };

                // When
                ArgumentParser argParser = new ArgumentParser(args);
                Argument main = argParser.GetMainArgument();
                Argument variable = argParser.GetArgument("n");
                Argument values = argParser.GetArgument("values");
                Argument present = argParser.GetArgument("p");

                // Then
                main.Name.Should().Be(string.Empty);
                main.Values.Should().BeEquivalentTo(new string[] { "Main" });
                main.IsBoolean.Should().BeFalse();
                variable.Name.Should().Be("n");
                variable.Values.Should().BeEquivalentTo(new string[] { "Variable" });
                variable.IsBoolean.Should().BeFalse();
                values.Name.Should().Be("values");
                values.Values.Should().BeEquivalentTo(new string[] { "Test1", "test2" });
                values.IsBoolean.Should().BeFalse();
                present.IsBoolean.Should().BeTrue();
                present.Name.Should().Be("p");
                present.IsBoolean.Should().BeTrue();
            }

            [TestMethod]
            public void Test_03_ArgumentParser_WithEmpty()
            {
                // Given
                string[] args = { "", "/n", "", "--values", "Test1", "test2", "/p" };

                // When
                ArgumentParser argParser = new ArgumentParser(args);
                Argument main = argParser.GetMainArgument();
                Argument variable = argParser.GetArgument("n");
                Argument values = argParser.GetArgument("values");
                Argument present = argParser.GetArgument("p");

                // Then
                main.Name.Should().Be(string.Empty);
                main.Values.Should().BeEquivalentTo(new string[] { string.Empty });
                main.IsBoolean.Should().BeFalse();
                variable.Name.Should().Be("n");
                variable.Values.Should().BeEquivalentTo(new string[] { string.Empty });
                variable.IsBoolean.Should().BeFalse();
                values.Name.Should().Be("values");
                values.Values.Should().BeEquivalentTo(new string[] { "Test1", "test2" });
                values.IsBoolean.Should().BeFalse();
                present.IsBoolean.Should().BeTrue();
                present.Name.Should().Be("p");
                present.IsBoolean.Should().BeTrue();
            }
        }
    }
}
