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
        [
            new[]{ true, true, false },
            false,
            new[]{ true, true, true }
        ],
        [
            new[]{ true, false, true },
            false,
            new[]{ true, true, false }
        ],
        [
            new[]{ false, true, true },
            false,
            new[]{ true, false, false }
        ],
        [
            new[] { true, false, false },
            false,
            new[]{ true, true, false }
        ],
        [
            new[] { false, true, false },
            false,
            new[] { true, false, false }
        ],
        [
            new[] { false, false, true },
            false,
            new[] { true, false, false }
        ]
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

        sut.Apply(item).ShouldBe(expected);

        called1.ShouldBe(expectedFuncCalls[0]);
        called2.ShouldBe(expectedFuncCalls[1]);
        called3.ShouldBe(expectedFuncCalls[2]);
    }

    internal static object[] DetailedApplyTestSource = new[]
    {
        new object[]
        {
            new[] { false, false, false },
            new[] { "code 1", "code 2", "code 3" }
        },
        [
            new[] { true, false, false },
            new[] { "code 2", "code 3" }
        ],
        [
            new[] { false, true, false },
            new[] { "code 1", "code 3" }
        ],
        [
            new[] { false, false, true },
            new[] { "code 1", "code 2" }
        ],
        [
            new[] { false, true, true },
            new[] { "code 1" }
        ],
        [
            new[] { true, false, true },
            new[] { "code 2" }
        ],
        [
            new[] { true, true, false },
            new[] { "code 3" }
        ]
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
            .Tee(e => e.IsLeft.ShouldBeTrue())
            .OnLeft(e => e.ShouldBe(expected));

        called1.ShouldBe(true);
        called2.ShouldBe(true);
        called3.ShouldBe(true);
    }

    internal static object[] FirstMatchingTestSource = new[]
    {
        new object[]
        {
            new[] { false, false, false },
            Option<string>.None(),
            new[] { true, true, true }
        },
        [
            new[] { true, false, false },
            Option<string>.Some("code 1"),
            new[] { true, false, false }
        ],
        [
            new[] { false, true, false },
            Option<string>.Some("code 2"),
            new[] { true, true, false }
        ],
        [
            new[] { false, false, true },
            Option<string>.Some("code 3"),
            new[] { true, true, true }
        ],
        [
            new[] { false, true, true },
            Option<string>.Some("code 2"),
            new[] { true, true, false }
        ],
        [
            new[] { true, false, true },
            Option<string>.Some("code 1"),
            new[] { true, false, false }
        ],
        [
            new[] { true, true, false },
            Option<string>.Some("code 1"),
            new[] { true, false, false }
        ],
        [
            new[] { true, true, true },
            Option<string>.Some("code 1"),
            new[] { true, false, false }
        ]
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
            .ShouldBeEquivalentTo(expected);

        called1.ShouldBe(expectedFuncCalls[0]);
        called2.ShouldBe(expectedFuncCalls[1]);
        called3.ShouldBe(expectedFuncCalls[2]);
    }
}
