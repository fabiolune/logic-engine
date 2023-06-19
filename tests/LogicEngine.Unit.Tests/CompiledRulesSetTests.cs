using TinyFp;
using TinyFp.Extensions;

namespace LogicEngine.Unit.Tests;

public class CompiledRulesSetTests
{
    internal static object[] ApplyTestSource = new[]
    {
        new object[]
        {
            (true, true, true),
            true,
            (true, true, true)
        },
        new object[]
        {
            (true, true, false),
            false,
            (true, true, true)
        },
        new object[]
        {
            (true, false, true),
            false,
            (true, true, false)
        },
        new object[]
        {
            (false, true, true),
            false,
            (true, false, false)
        },
        new object[]
        {
            (true, false, false),
            false,
            (true, true, false)
        },
        new object[]
        {
            (false, true, false),
            false,
            (true, false, false)
        },
        new object[]
        {
            (false, false, true),
            false,
            (true, false, false)
        }
    };

    [TestCaseSource(nameof(ApplyTestSource))]
    public void Apply_WhenCompiledRulesReturnDifferentValues_ShouldReturnExpectedResults((bool, bool, bool) funcOutputs, bool expected, (bool, bool, bool) expectedFuncCalls)
    {
        var called1 = false;
        var called2 = false;
        var called3 = false;

        var compiledRules = new CompiledRule<TestModel>[]
        {
            new (item =>
            {
                called1 = true;
                return funcOutputs.Item1;
            }, "code 1"),
            new (item =>
            {
                called2 = true;
                return funcOutputs.Item2;
            }, "code 2"),
            new (item =>
            {
                called3 = true;
                return funcOutputs.Item3;
            }, "code 3")
        };

        var sut = new CompiledRulesSet<TestModel>(compiledRules, "set name");

        sut.Apply(It.IsAny<TestModel>()).Should().Be(expected);

        called1.Should().Be(expectedFuncCalls.Item1);
        called2.Should().Be(expectedFuncCalls.Item2);
        called3.Should().Be(expectedFuncCalls.Item3);
    }

    internal static object[] DetailedApplyTestSource = new[]
    {
        new object[]
        {
            (false, false, false),
            new[] { "code 1", "code 2", "code 3" }
        },
        new object[]
        {
            (true, false, false),
            new[] { "code 2", "code 3" }
        },
        new object[]
        {
            (false, true, false),
            new[] { "code 1", "code 3" }
        },
        new object[]
        {
            (false, false, true),
            new[] { "code 1", "code 2" }
        },
        new object[]
        {
            (false, true, true),
            new[] { "code 1" }
        },
        new object[]
        {
            (true, false, true),
            new[] { "code 2" }
        },
        new object[]
        {
            (true, true, false),
            new[] { "code 3" }
        }
    };

    [TestCaseSource(nameof(DetailedApplyTestSource))]
    public void DetailedApply_WhenSomeRuleIsNotSatisfied_ShouldReturnLeft((bool, bool, bool) funcOutputs, string[] expected)
    {
        var called1 = false;
        var called2 = false;
        var called3 = false;

        var compiledRules = new CompiledRule<TestModel>[]
        {
            new (item =>
            {
                called1 = true;
                return funcOutputs.Item1;
            }, "code 1"),
            new (item =>
            {
                called2 = true;
                return funcOutputs.Item2;
            }, "code 2"),
            new (item =>
            {
                called3 = true;
                return funcOutputs.Item3;
            }, "code 3")
        };

        var sut = new CompiledRulesSet<TestModel>(compiledRules, "set name");

        sut
            .DetailedApply(It.IsAny<TestModel>())
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
            (false, false, false),
            Option<string>.None(),
            (true, true, true)
        },
        new object[]
        {
            (true, false, false),
            Option<string>.Some("code 1"),
            (true, false, false)
        },
        new object[]
        {
            (false, true, false),
            Option<string>.Some("code 2"),
            (true, true, false)
        },
        new object[]
        {
            (false, false, true),
            Option<string>.Some("code 3"),
            (true, true, true)
        },
        new object[]
        {
            (false, true, true),
            Option<string>.Some("code 2"),
            (true, true, false)
        },
        new object[]
        {
            (true, false, true),
            Option<string>.Some("code 1"),
            (true, false, false)
        },
        new object[]
        {
            (true, true, false),
            Option<string>.Some("code 1"),
            (true, false, false)
        },
        new object[]
        {
            (true, true, true),
            Option<string>.Some("code 1"),
            (true, false, false)
        }
    };

    [TestCaseSource(nameof(FirstMatchingTestSource))]
    public void FirstMatching_ShouldReturnExpectedValue((bool, bool, bool) funcOutputs, Option<string> expected, (bool, bool, bool) expectedFuncCalls)
    {
        var called1 = false;
        var called2 = false;
        var called3 = false;

        var compiledRules = new CompiledRule<TestModel>[]
        {
            new (item =>
            {
                called1 = true;
                return funcOutputs.Item1;
            }, "code 1"),
            new (item =>
            {
                called2 = true;
                return funcOutputs.Item2;
            }, "code 2"),
            new (item =>
            {
                called3 = true;
                return funcOutputs.Item3;
            }, "code 3")
        };

        var sut = new CompiledRulesSet<TestModel>(compiledRules, "set name");

        sut
            .FirstMatching(It.IsAny<TestModel>())
            .Should().BeEquivalentTo(expected);

        called1.Should().Be(expectedFuncCalls.Item1);
        called2.Should().Be(expectedFuncCalls.Item2);
        called3.Should().Be(expectedFuncCalls.Item3);
    }
}
