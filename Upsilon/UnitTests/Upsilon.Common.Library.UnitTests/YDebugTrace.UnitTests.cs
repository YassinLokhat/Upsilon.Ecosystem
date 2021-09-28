using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Upsilon.Common.Library.UnitTests.TestClasses;

namespace Upsilon.Common.Library.UnitTests
{
    [TestClass]
    public class YDebugTrace_UnitTests : YUnitTestsClass
    {
        [TestMethod]
        public void Test_01_SimpleTraceStack_Case1()
        {
            // Given
            var param = 15;

            // When
            YDebugTraceClasses.Function1(param);

            // Then
            YDebugTrace.RootTrace.Should().NotBeNull();
            YDebugTrace.RootTrace.Traces.Count.Should().Be(1);

            var parent = YDebugTrace.RootTrace;

            var trace = parent.Traces.First();
            trace.TraceId.Should().Be(1);
            trace.FileName.Should().Be(@"\UpsilonEcosystem\Upsilon\UnitTests\Upsilon.Common.Library.UnitTests\TestClasses\YDebugTraceClasses.cs");
            trace.StartLine.Should().Be(13);
            trace.EndLine.Should().Be(18);
            trace.CallerMethod.Should().Be("Test_01_SimpleTraceStack_Case1");
            trace.CallerLine.Should().Be(22);
            trace.ExecutingMethodeName.Should().Be("Function1");
            trace.Parent.Should().Be(parent);
            trace.Parameters.Should().BeEquivalentTo(new object[] { param });
            trace.Return.Should().BeNull();
            trace.IsClosed.Should().BeTrue();
            trace.Traces.Count.Should().Be(1);

            parent = trace;
            trace = parent.Traces.First();
            trace.TraceId.Should().Be(1);
            trace.FileName.Should().Be(@"\UpsilonEcosystem\Upsilon\UnitTests\Upsilon.Common.Library.UnitTests\TestClasses\YDebugTraceClasses.cs");
            trace.StartLine.Should().Be(23);
            trace.EndLine.Should().Be(47);
            trace.CallerMethod.Should().Be("Function1");
            trace.CallerLine.Should().Be(16);
            trace.ExecutingMethodeName.Should().Be("Function2");
            trace.Parent.Should().Be(parent);
            trace.Parameters.Should().BeEquivalentTo(new object[] { param, false });
            trace.Return.Should().Be(param + 1);
            trace.IsClosed.Should().BeTrue();
            trace.Traces.Count.Should().Be(1);

            parent = trace;
            trace = parent.Traces.First();
            trace.TraceId.Should().Be(1);
            trace.FileName.Should().Be(@"\UpsilonEcosystem\Upsilon\UnitTests\Upsilon.Common.Library.UnitTests\TestClasses\YDebugTraceClasses.cs");
            trace.CallerMethod.Should().Be("Function2");
            trace.StartLine.Should().Be(59);
            trace.EndLine.Should().Be(59);
            trace.CallerLine.Should().Be(37);
            trace.ExecutingMethodeName.Should().Contain("Function4");
            trace.Return.Should().Be(true);
            trace.Parent.Should().Be(parent);
            trace.Parameters.Count().Should().Be(0);
            trace.IsClosed.Should().BeTrue();
            trace.Traces.Count.Should().Be(0);
        }

        [TestMethod]
        public void Test_02_SimpleTraceStack_Case2()
        {
            // Given
            var param = 20;

            // When
            YDebugTraceClasses.Function1(param);

            // Then
            YDebugTrace.RootTrace.Should().NotBeNull();
            YDebugTrace.RootTrace.Traces.Count.Should().Be(1);

            var parent = YDebugTrace.RootTrace;

            var trace = parent.Traces.First();
            trace.TraceId.Should().Be(1);
            trace.FileName.Should().Be(@"\UpsilonEcosystem\Upsilon\UnitTests\Upsilon.Common.Library.UnitTests\TestClasses\YDebugTraceClasses.cs");
            trace.StartLine.Should().Be(13);
            trace.EndLine.Should().Be(18);
            trace.CallerMethod.Should().Be("Test_02_SimpleTraceStack_Case2");
            trace.CallerLine.Should().Be(82);
            trace.ExecutingMethodeName.Should().Be("Function1");
            trace.Parent.Should().Be(parent);
            trace.Parameters.Should().BeEquivalentTo(new object[] { param });
            trace.Return.Should().BeNull();
            trace.IsClosed.Should().BeTrue();
            trace.Traces.Count.Should().Be(1);

            parent = trace;
            trace = parent.Traces.First();
            trace.TraceId.Should().Be(1);
            trace.FileName.Should().Be(@"\UpsilonEcosystem\Upsilon\UnitTests\Upsilon.Common.Library.UnitTests\TestClasses\YDebugTraceClasses.cs");
            trace.StartLine.Should().Be(23);
            trace.EndLine.Should().Be(47);
            trace.CallerMethod.Should().Be("Function1");
            trace.CallerLine.Should().Be(16);
            trace.ExecutingMethodeName.Should().Be("Function2");
            trace.Parent.Should().Be(parent);
            trace.Parameters.Should().BeEquivalentTo(new object[] { param, false });
            trace.Return.Should().Be(param + 1);
            trace.IsClosed.Should().BeTrue();
            trace.Traces.Count.Should().Be(1);

            parent = trace;
            trace = parent.Traces.First();
            trace.TraceId.Should().Be(1);
            trace.FileName.Should().Be(@"\UpsilonEcosystem\Upsilon\UnitTests\Upsilon.Common.Library.UnitTests\TestClasses\YDebugTraceClasses.cs");
            trace.CallerMethod.Should().Be("Function2");
            trace.StartLine.Should().Be(52);
            trace.EndLine.Should().Be(54);
            trace.CallerLine.Should().Be(31);
            trace.ExecutingMethodeName.Should().Contain("Function3");
            trace.Return.Should().BeNull();
            trace.Parent.Should().Be(parent);
            trace.Parameters.Count().Should().Be(0);
            trace.IsClosed.Should().BeTrue();
            trace.Traces.Count.Should().Be(0);
        }

        [TestMethod]
        public void Test_03_ExceptionThrown_Case1()
        {
            // Given
            var param = 15;

            // When
            Action act = new(() => 
            {
                YDebugTraceClasses.Function1(param, true);
            });

            // Then
            act.Should().Throw<Exception>();
            YDebugTrace.RootTrace.Should().NotBeNull();
            YDebugTrace.RootTrace.Traces.Count.Should().Be(1);

            var parent = YDebugTrace.RootTrace;

            var trace = parent.Traces.First();
            trace.TraceId.Should().Be(1);
            trace.FileName.Should().Be(@"\UpsilonEcosystem\Upsilon\UnitTests\Upsilon.Common.Library.UnitTests\TestClasses\YDebugTraceClasses.cs");
            trace.StartLine.Should().Be(13);
            trace.EndLine.Should().Be(-1);
            trace.CallerMethod.Should().Contain("Test_03_ExceptionThrown_Case1");
            trace.CallerLine.Should().Be(144);
            trace.ExecutingMethodeName.Should().Be("Function1");
            trace.Parent.Should().Be(parent);
            trace.Parameters.Should().BeEquivalentTo(new object[] { param });
            trace.Return.Should().BeNull();
            trace.IsClosed.Should().BeFalse();
            trace.Traces.Count.Should().Be(1);

            parent = trace;
            trace = parent.Traces.First();
            trace.TraceId.Should().Be(1);
            trace.FileName.Should().Be(@"\UpsilonEcosystem\Upsilon\UnitTests\Upsilon.Common.Library.UnitTests\TestClasses\YDebugTraceClasses.cs");
            trace.StartLine.Should().Be(23);
            trace.EndLine.Should().Be(-1);
            trace.CallerMethod.Should().Be("Function1");
            trace.CallerLine.Should().Be(16);
            trace.ExecutingMethodeName.Should().Be("Function2");
            trace.Parent.Should().Be(parent);
            trace.Parameters.Should().BeEquivalentTo(new object[] { param, true });
            trace.Return.Should().BeNull();
            trace.IsClosed.Should().BeFalse();
            trace.Traces.Count.Should().Be(1);

            parent = trace;
            trace = parent.Traces.First();
            trace.TraceId.Should().Be(1);
            trace.FileName.Should().Be(@"\UpsilonEcosystem\Upsilon\UnitTests\Upsilon.Common.Library.UnitTests\TestClasses\YDebugTraceClasses.cs");
            trace.CallerMethod.Should().Be("Function2");
            trace.StartLine.Should().Be(59);
            trace.EndLine.Should().Be(59);
            trace.CallerLine.Should().Be(37);
            trace.ExecutingMethodeName.Should().Contain("Function4");
            trace.Return.Should().Be(true);
            trace.Parent.Should().Be(parent);
            trace.Parameters.Count().Should().Be(0);
            trace.IsClosed.Should().BeTrue();
            trace.Traces.Count.Should().Be(0);
        }

        [TestMethod]
        public void Test_04_ExceptionThrown_Case2()
        {
            // Given
            var param = 20;

            // When
            Action act = new(() => 
            {
                YDebugTraceClasses.Function1(param, true);
            });

            // Then
            act.Should().Throw<Exception>();
            YDebugTrace.RootTrace.Should().NotBeNull();
            YDebugTrace.RootTrace.Traces.Count.Should().Be(1);

            var parent = YDebugTrace.RootTrace;

            var trace = parent.Traces.First();
            trace.TraceId.Should().Be(1);
            trace.FileName.Should().Be(@"\UpsilonEcosystem\Upsilon\UnitTests\Upsilon.Common.Library.UnitTests\TestClasses\YDebugTraceClasses.cs");
            trace.StartLine.Should().Be(13);
            trace.EndLine.Should().Be(-1);
            trace.CallerMethod.Should().Contain("Test_04_ExceptionThrown_Case2");
            trace.CallerLine.Should().Be(208);
            trace.ExecutingMethodeName.Should().Be("Function1");
            trace.Parent.Should().Be(parent);
            trace.Parameters.Should().BeEquivalentTo(new object[] { param });
            trace.Return.Should().BeNull();
            trace.IsClosed.Should().BeFalse();
            trace.Traces.Count.Should().Be(1);

            parent = trace;
            trace = parent.Traces.First();
            trace.TraceId.Should().Be(1);
            trace.FileName.Should().Be(@"\UpsilonEcosystem\Upsilon\UnitTests\Upsilon.Common.Library.UnitTests\TestClasses\YDebugTraceClasses.cs");
            trace.StartLine.Should().Be(23);
            trace.EndLine.Should().Be(-1);
            trace.CallerMethod.Should().Be("Function1");
            trace.CallerLine.Should().Be(16);
            trace.ExecutingMethodeName.Should().Be("Function2");
            trace.Parent.Should().Be(parent);
            trace.Parameters.Should().BeEquivalentTo(new object[] { param, true });
            trace.Return.Should().BeNull();
            trace.IsClosed.Should().BeFalse();
            trace.Traces.Count.Should().Be(1);

            parent = trace;
            trace = parent.Traces.First();
            trace.TraceId.Should().Be(1);
            trace.FileName.Should().Be(@"\UpsilonEcosystem\Upsilon\UnitTests\Upsilon.Common.Library.UnitTests\TestClasses\YDebugTraceClasses.cs");
            trace.CallerMethod.Should().Be("Function2");
            trace.StartLine.Should().Be(52);
            trace.EndLine.Should().Be(54);
            trace.CallerLine.Should().Be(31);
            trace.ExecutingMethodeName.Should().Contain("Function3");
            trace.Return.Should().BeNull();
            trace.Parent.Should().Be(parent);
            trace.Parameters.Count().Should().Be(0);
            trace.IsClosed.Should().BeTrue();
            trace.Traces.Count.Should().Be(0);
        }
    }
}
