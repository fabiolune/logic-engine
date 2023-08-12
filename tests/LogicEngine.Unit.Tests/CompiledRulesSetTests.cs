using AutoBogus;
using TinyFp;
using TinyFp.Extensions;

namespace LogicEngine.Unit.Tests;

public class CompiledRulesSetTests
{
    internal static object[] ApplyTestSource = new[]
    {
        new object[]
        {
            new[]{ true, true, true },
            true,
            new[]{ true, true, true }
        },
        new object[]
        {
            new[]{ true, true, false },
            false,
            new[]{ true, true, true }
        },
        new object[]
        {
            new[]{ true, false, true },
            false,
            new[]{ true, true, false }
        },
        new object[]
        {
            new[]{ false, true, true },
            false,
            new[]{ true, false, false }
        },
        new object[]
        {
            new[] { true, false, false },
            false,
            new[]{ true, true, false }
        },
        new object[]
        {
            new[] { false, true, false },
            false,
            new[] { true, false, false }
        },
        new object[]
        {
            new[] { false, false, true },
            false,
            new[] { true, false, false }
        }
    };

    [TestCaseSource(nameof(ApplyTestSource))]
    public void Apply_WhenCompiledRulesReturnDifferentValues_ShouldReturnExpectedResults(bool[] funcOutputs, bool expected, bool[] expectedFuncCalls)
    {
        var called1 = false;
        var called2 = false;
        var called3 = false;

        var compiledRules = new CompiledRule<TestModel>[]
        {
            new (item =>
            {
                called1 = true;
                return funcOutputs[0];
            }, "code 1"),
            new (item =>
            {
                called2 = true;
                return funcOutputs[1];
            }, "code 2"),
            new (item =>
            {
                called3 = true;
                return funcOutputs[2];
            }, "code 3")
        };

        var sut = new CompiledRulesSet<TestModel>(compiledRules, "set name");

        var item = new AutoFaker<TestModel>().Generate();

        sut.Apply(item).Should().Be(expected);

        called1.Should().Be(expectedFuncCalls[0]);
        called2.Should().Be(expectedFuncCalls[1]);
        called3.Should().Be(expectedFuncCalls[2]);
    }

    internal static object[] DetailedApplyTestSource = new[]
    {
        new object[]
        {
            new[] { false, false, false },
            new[] { "code 1", "code 2", "code 3" }
        },
        new object[]
        {
            new[] { true, false, false },
            new[] { "code 2", "code 3" }
        },
        new object[]
        {
            new[] { false, true, false },
            new[] { "code 1", "code 3" }
        },
        new object[]
        {
            new[] { false, false, true },
            new[] { "code 1", "code 2" }
        },
        new object[]
        {
            new[] { false, true, true },
            new[] { "code 1" }
        },
        new object[]
        {
            new[] { true, false, true },
            new[] { "code 2" }
        },
        new object[]
        {
            new[] { true, true, false },
            new[] { "code 3" }
        }
    };

    [TestCaseSource(nameof(DetailedApplyTestSource))]
    public void DetailedApply_WhenSomeRuleIsNotSatisfied_ShouldReturnLeft(bool[] funcOutputs, string[] expected)
    {
        var called1 = false;
        var called2 = false;
        var called3 = false;

        var compiledRules = new CompiledRule<TestModel>[]
        {
            new (item =>
            {
                called1 = true;
                return funcOutputs[0];
            }, "code 1"),
            new (item =>
            {
                called2 = true;
                return funcOutputs[1];
            }, "code 2"),
            new (item =>
            {
                called3 = true;
                return funcOutputs[2];
            }, "code 3")
        };

        var sut = new CompiledRulesSet<TestModel>(compiledRules, "set name");

        var item = new AutoFaker<TestModel>().Generate();

        sut
            .DetailedApply(item)
            .Tee(e => e.IsLeft.Should().BeTrue())
            .OnLeft(e => e.Should().BeEquivalentTo(expected));

        called1.Should().Be(true);
        called2.Should().Be(true);
        called3.Should().Be(true);
    }

    internal static object[] FirstMatchingTestSource = new[]
    {
        new object[]
        {
            new[] { false, false, false },
            Option<string>.None(),
            new[] { true, true, true }
        },
        new object[]
        {
            new[] { true, false, false },
            Option<string>.Some("code 1"),
            new[] { true, false, false }
        },
        new object[]
        {
            new[] { false, true, false },
            Option<string>.Some("code 2"),
            new[] { true, true, false }
        },
        new object[]
        {
            new[] { false, false, true },
            Option<string>.Some("code 3"),
            new[] { true, true, true }
        },
        new object[]
        {
            new[] { false, true, true },
            Option<string>.Some("code 2"),
            new[] { true, true, false }
        },
        new object[]
        {
            new[] { true, false, true },
            Option<string>.Some("code 1"),
            new[] { true, false, false }
        },
        new object[]
        {
            new[] { true, true, false },
            Option<string>.Some("code 1"),
            new[] { true, false, false }
        },
        new object[]
        {
            new[] { true, true, true },
            Option<string>.Some("code 1"),
            new[] { true, false, false }
        }
    };

    [TestCaseSource(nameof(FirstMatchingTestSource))]
    public void FirstMatching_ShouldReturnExpectedValue(bool[] funcOutputs, Option<string> expected, bool[] expectedFuncCalls)
    {
        var called1 = false;
        var called2 = false;
        var called3 = false;

        var compiledRules = new CompiledRule<TestModel>[]
        {
            new (item =>
            {
                called1 = true;
                return funcOutputs[0];
            }, "code 1"),
            new (item =>
            {
                called2 = true;
                return funcOutputs[1];
            }, "code 2"),
            new (item =>
            {
                called3 = true;
                return funcOutputs[2];
            }, "code 3")
        };

        var sut = new CompiledRulesSet<TestModel>(compiledRules, "set name");

        var item = new AutoFaker<TestModel>().Generate();

        sut
            .FirstMatching(item)
            .Should().BeEquivalentTo(expected);

        called1.Should().Be(expectedFuncCalls[0]);
        called2.Should().Be(expectedFuncCalls[1]);
        called3.Should().Be(expectedFuncCalls[2]);
    }
}
