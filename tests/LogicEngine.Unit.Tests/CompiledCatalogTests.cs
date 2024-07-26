using AutoBogus;
using System.Collections.Generic;
using TinyFp;
using TinyFp.Extensions;

namespace LogicEngine.Unit.Tests;

public class CompiledCatalogTests
{
    internal static object[] ApplyTestSource = new[]
    {
        // 0 false
        new object[]
        {
            new[]{ true, true, true, true },
            true,
            new[]{ true, true, false, false }
        },
        // 1 false
        [
            new[]{ true, true, true, false },
            true,
            new[]{ true, true, false, false }
        ],
        [
            new[]{ true, true, false, true },
            true,
            new[]{ true, true, false, false }
        ],
        [
            new[]{ true, false, true, true },
            true,
            new[]{ true, true, true, true }
        ],
        [
            new[]{ false, true, true, true },
            true,
            new[]{ true, false, true, true }
        ],
        // 2 false
        [
            new[]{ true, true, false, false },
            true,
            new[]{ true, true, false, false }
        ],
        [
            new[]{ true, false, false, true },
            false,
            new[]{ true, true, true, false }
        ],
        [
            new[]{ false, false, true, true },
            true,
            new[]{ true, false, true, true }
        ],
        [
            new[]{ false, true, false, true },
            false,
            new[]{ true, false, true, false }
        ],
        [
            new[]{ true, false, true, false },
            false,
            new[]{ true, true, true, true }
        ],
        // 3 false
        [
            new[]{ true, false, false, false },
            false,
            new[]{ true, true, true, false }
        ],
        [
            new[]{ false, false, true, false },
            false,
            new[]{ true, false, true, true }
        ],
        [
            new[]{ false, true, false, false },
            false,
            new[]{ true, false, true, false }
        ],
        [
            new[]{ false, false, false, true },
            false,
            new[]{ true, false, true, false }
        ],
        // 4 false
        [
            new[]{ false, false, false, false },
            false,
            new[]{ true, false, true, false }
        ]
    };

    [TestCaseSource(nameof(ApplyTestSource))]
    public void Apply_WhenRulesSetsProvideDifferentResults_ShouldReturnExpectedResult(
        bool[] funcOutputs,
        bool expected,
        bool[] funcCalled)
    {
        var called11 = false;
        var called12 = false;
        var called21 = false;
        var called22 = false;

        var rulesSets = new CompiledRulesSet<TestModel>[]
        {
            new(
            [
                new (item => {called11 = true;  return funcOutputs[0]; }, "code 1.1"),
                new (item => {called12 = true;  return funcOutputs[1]; }, "code 1.2")
            ], "set 1"),
            new(
            [
                new (item => {called21 = true;  return funcOutputs[2]; }, "code 2.1"),
                new (item => {called22 = true;  return funcOutputs[3]; }, "code 2.2")
            ], "set 2")
        };

        var sut = new CompiledCatalog<TestModel>(rulesSets, "catalog name");

        var item = new AutoFaker<TestModel>().Generate();

        sut.Apply(item).Should().Be(expected);

        called11.Should().Be(funcCalled[0]);
        called12.Should().Be(funcCalled[1]);
        called21.Should().Be(funcCalled[2]);
        called22.Should().Be(funcCalled[3]);
    }

    internal static object[] DetailedApplyFailingTestSource = new[]
    {
        new object[]
        {
            new[]{ true, false, false, true },
            new[]{ "code 1.2", "code 2.1" }
        },
        [
            new[]{ false, true, false, true },
            new[]{ "code 1.1", "code 2.1" }
        ],
        [
            new[]{ true, false, true, false },
            new[]{ "code 1.2", "code 2.2" }
        ],
        // 3 false
        [
            new[]{ true, false, false, false },
            new[]{ "code 1.2", "code 2.1", "code 2.2" }
        ],
        [
            new[]{ false, false, true, false },
            new[]{ "code 1.1", "code 1.2", "code 2.2" }
        ],
        [
            new[]{ false, true, false, false },
            new[]{ "code 1.1", "code 2.1", "code 2.2" }
        ],
        [
            new[]{ false, false, false, true },
            new[]{ "code 1.1", "code 1.2", "code 2.1" }
        ],
        // 4 false
        [
            new[]{ false, false, false, false },
            new[]{ "code 1.1", "code 1.2", "code 2.1", "code 2.2" }
        ]
    };

    [TestCaseSource(nameof(DetailedApplyFailingTestSource))]
    public void DetailedApply_WhenRulesSetIsNotSuccessful_ShouldReturnExpectedCodes(
        bool[] funcOutputs,
        IEnumerable<string> expected
        )
    {
        var called11 = false;
        var called12 = false;
        var called21 = false;
        var called22 = false;

        var rulesSets = new CompiledRulesSet<TestModel>[]
        {
            new(
            [
                new (item => {called11 = true;  return funcOutputs[0]; }, "code 1.1"),
                new (item => {called12 = true;  return funcOutputs[1]; }, "code 1.2")
            ], "set 1"),
            new(
            [
                new (item => {called21 = true;  return funcOutputs[2]; }, "code 2.1"),
                new (item => {called22 = true;  return funcOutputs[3]; }, "code 2.2")
            ], "set 2")
        };

        var sut = new CompiledCatalog<TestModel>(rulesSets, "catalog name");

        var item = new AutoFaker<TestModel>().Generate();

        sut.DetailedApply(item)
            .Tee(e => e.IsLeft.Should().BeTrue())
            .OnLeft(e => e.Should().BeEquivalentTo(expected));

        called11.Should().BeTrue();
        called12.Should().BeTrue();
        called21.Should().BeTrue();
        called22.Should().BeTrue();
    }

    internal static object[] DetailedApplySuccessfullTestSource = new[]
    {
        new object[]
        {
            new[]{ true, true, true, true },
            new[]{ true, true, false, false }
        },
        [
            new[]{ true, true, true, false },
            new[]{ true, true, false, false }
        ],
        [
            new[]{ true, true, false, true },
            new[]{ true, true, false, false }
        ],
        [
            new[]{ true, false, true, true },
            new[]{ true, true, true, true }
        ],
        [
            new[]{ false, true, true, true },
            new[]{ true, false, true, true }
        ],
        [
            new[]{ true, true, false, false },
            new[]{ true, true, false, false }
        ],
        [
            new[]{ false, false, true, true },
            new[]{ true, false, true, true }
        ]
    };

    [TestCaseSource(nameof(DetailedApplySuccessfullTestSource))]
    public void DetailedApply_WhenRulesSetIsSuccessful_ShouldReturnRight(
        bool[] funcOutputs,
        bool[] funcCalled)
    {
        var called11 = false;
        var called12 = false;
        var called21 = false;
        var called22 = false;

        var rulesSets = new CompiledRulesSet<TestModel>[]
        {
            new(
            [
                new (item => {called11 = true;  return funcOutputs[0]; }, "code 1.1"),
                new (item => {called12 = true;  return funcOutputs[1]; }, "code 1.2")
            ], "set 1"),
            new(
            [
                new (item => {called21 = true;  return funcOutputs[2]; }, "code 2.1"),
                new (item => {called22 = true;  return funcOutputs[3]; }, "code 2.2")
            ], "set 2")
        };

        var sut = new CompiledCatalog<TestModel>(rulesSets, "catalog name");

        var item = new AutoFaker<TestModel>().Generate();

        sut.DetailedApply(item).IsRight.Should().BeTrue();

        called11.Should().Be(funcCalled[0]);
        called12.Should().Be(funcCalled[1]);
        called21.Should().Be(funcCalled[2]);
        called22.Should().Be(funcCalled[3]);
    }

    internal static object[] FirstMatchingTestSource = new[]
    {
        new object[]
        {
            new[]{ false, false, false, false },
            Option<string>.None(),
            new[]{ true, false, true, false }
        },
        [
            new[]{ true, false, false, false },
            Option<string>.None(),
            new[]{ true, true, true, false }
        ],
        [
            new[]{ false, true, false, false },
            Option<string>.None(),
            new[]{ true, false, true, false }
        ],
        [
            new[]{ false, false, true, false },
            Option<string>.None(),
            new[]{ true, false, true, true }
        ],
        [
            new[]{ false, false, false, true },
            Option<string>.None(),
            new[]{ true, false, true, false }
        ],
        [
            new[]{ true, true, false, false },
            Option<string>.Some("set 1"),
            new[]{ true, true, false, false }
        ],
        [
            new[]{ true, false, true, false },
            Option<string>.None(),
            new[]{ true, true, true, true }
        ],
        [
            new[]{ false, true, true, false },
            Option<string>.None(),
            new[]{ true, false, true, true }
        ],
        [
            new[]{ true, false, false, true },
            Option<string>.None(),
            new[]{ true, true, true, false }
        ],
        [
            new[]{ false, true, false, true },
            Option<string>.None(),
            new[]{ true, false, true, false }
        ],
        [
            new[]{ false, false, true, true },
            Option<string>.Some("set 2"),
            new[]{ true, false, true, true }
        ],
        [
            new[]{ true, true, true, false },
            Option<string>.Some("set 1"),
            new[]{ true, true, false, false }
        ],
        [
            new[]{ true, true, false, true },
            Option<string>.Some("set 1"),
            new[]{ true, true, false, false }
        ],
        [
            new[]{ true, false, true, true },
            Option<string>.Some("set 2"),
            new[]{ true, true, true, true }
        ],
        [
            new[]{ false, true, true, true },
            Option<string>.Some("set 2"),
            new[]{ true, false, true, true }
        ],
        [
            new[]{ true, true, true, true },
            Option<string>.Some("set 1"),
            new[]{ true, true, false, false }
        ]
    };

    [TestCaseSource(nameof(FirstMatchingTestSource))]
    public void FirstMatching_ShouldReturnExpectedValue(
            bool[] funcOutputs,
            Option<string> expected,
            bool[] funcCalled)
    {
        var called11 = false;
        var called12 = false;
        var called21 = false;
        var called22 = false;

        var rulesSets = new CompiledRulesSet<TestModel>[]
        {
            new(
            [
                new (item => {called11 = true;  return funcOutputs[0]; }, "code 1.1"),
                new (item => {called12 = true;  return funcOutputs[1]; }, "code 1.2")
            ], "set 1"),
            new(
            [
                new (item => {called21 = true;  return funcOutputs[2]; }, "code 2.1"),
                new (item => {called22 = true;  return funcOutputs[3]; }, "code 2.2")
            ], "set 2")
        };

        var sut = new CompiledCatalog<TestModel>(rulesSets, "catalog name");

        var item = new AutoFaker<TestModel>().Generate();

        sut.FirstMatching(item).Should().Be(expected);

        called11.Should().Be(funcCalled[0]);
        called12.Should().Be(funcCalled[1]);
        called21.Should().Be(funcCalled[2]);
        called22.Should().Be(funcCalled[3]);
    }
}
