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
            (true, true, true, true),
            true,
            (true, true, false, false)
        },
        // 1 false
        new object[]
        {
            (true, true, true, false),
            true,
            (true, true, false, false)
        },
        new object[]
        {
            (true, true, false, true),
            true,
            (true, true, false, false)
        },
        new object[]
        {
            (true, false, true, true),
            true,
            (true, true, true, true)
        },
        new object[]
        {
            (false, true, true, true),
            true,
            (true, false, true, true)
        },
        // 2 false
        new object[]
        {
            (true, true, false, false),
            true,
            (true, true, false, false)
        },
        new object[]
        {
            (true, false, false, true),
            false,
            (true, true, true, false)
        },
        new object[]
        {
            (false, false, true, true),
            true,
            (true, false, true, true)
        },
        new object[]
        {
            (false, true, false, true),
            false,
            (true, false, true, false)
        },
        new object[]
        {
            (true, false, true, false),
            false,
            (true, true, true, true)
        },
        // 3 false
        new object[]
        {
            (true, false, false, false),
            false,
            (true, true, true, false)
        },
        new object[]
        {
            (false, false, true, false),
            false,
            (true, false, true, true)
        },
        new object[]
        {
            (false, true, false, false),
            false,
            (true, false, true, false)
        },
        new object[]
        {
            (false, false, false, true),
            false,
            (true, false, true, false)
        },
        // 4 false
        new object[]
        {
            (false, false, false, false),
            false,
            (true, false, true, false)
        }
    };

    [TestCaseSource(nameof(ApplyTestSource))]
    public void Apply_WhenRulesSetsProvideDifferentResults_ShouldReturnExpectedResult(
        (bool, bool, bool, bool) funcOutputs,
        bool expected,
        (bool, bool, bool, bool) funcCalled)
    {
        var called11 = false;
        var called12 = false;
        var called21 = false;
        var called22 = false;

        var rulesSets = new CompiledRulesSet<TestModel>[]
        {
            new CompiledRulesSet<TestModel>(new CompiledRule<TestModel>[]
            {
                new (item => {called11 = true;  return funcOutputs.Item1; }, "code 1.1"),
                new (item => {called12 = true;  return funcOutputs.Item2; }, "code 1.2")
            }, "set 1"),
            new CompiledRulesSet<TestModel>(new CompiledRule<TestModel>[]
            {
                new (item => {called21 = true;  return funcOutputs.Item3; }, "code 2.1"),
                new (item => {called22 = true;  return funcOutputs.Item4; }, "code 2.2")
            }, "set 2")
        };

        var sut = new CompiledCatalog<TestModel>(rulesSets, "catalog name");

        var item = new AutoFaker<TestModel>().Generate();

        sut.Apply(item).Should().Be(expected);

        called11.Should().Be(funcCalled.Item1);
        called12.Should().Be(funcCalled.Item2);
        called21.Should().Be(funcCalled.Item3);
        called22.Should().Be(funcCalled.Item4);
    }

    internal static object[] DetailedApplyFailingTestSource = new[]
    {
        new object[]
        {
            (true, false, false, true),
            new[]{ "code 1.2", "code 2.1" }
        },
        new object[]
        {
            (false, true, false, true),
            new[]{ "code 1.1", "code 2.1" }
        },
        new object[]
        {
            (true, false, true, false),
            new[]{ "code 1.2", "code 2.2" }
        },
        // 3 false
        new object[]
        {
            (true, false, false, false),
            new[]{ "code 1.2", "code 2.1", "code 2.2" }
        },
        new object[]
        {
            (false, false, true, false),
            new[]{ "code 1.1", "code 1.2", "code 2.2" }
        },
        new object[]
        {
            (false, true, false, false),
            new[]{ "code 1.1", "code 2.1", "code 2.2" }
        },
        new object[]
        {
            (false, false, false, true),
            new[]{ "code 1.1", "code 1.2", "code 2.1" }
        },
        // 4 false
        new object[]
        {
            (false, false, false, false),
            new[]{ "code 1.1", "code 1.2", "code 2.1", "code 2.2" }
        }
    };

    [TestCaseSource(nameof(DetailedApplyFailingTestSource))]
    public void DetailedApply_WhenRulesSetIsNotSuccessful_ShouldReturnExpectedCodes(
        (bool, bool, bool, bool) funcOutputs,
        IEnumerable<string> expected
        )
    {
        var called11 = false;
        var called12 = false;
        var called21 = false;
        var called22 = false;

        var rulesSets = new CompiledRulesSet<TestModel>[]
        {
            new CompiledRulesSet<TestModel>(new CompiledRule<TestModel>[]
            {
                new (item => {called11 = true;  return funcOutputs.Item1; }, "code 1.1"),
                new (item => {called12 = true;  return funcOutputs.Item2; }, "code 1.2")
            }, "set 1"),
            new CompiledRulesSet<TestModel>(new CompiledRule<TestModel>[]
            {
                new (item => {called21 = true;  return funcOutputs.Item3; }, "code 2.1"),
                new (item => {called22 = true;  return funcOutputs.Item4; }, "code 2.2")
            }, "set 2")
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
            (true, true, true, true),
            (true, true, false, false)
        },
        new object[]
        {
            (true, true, true, false),
            (true, true, false, false)
        },
        new object[]
        {
            (true, true, false, true),
            (true, true, false, false)
        },
        new object[]
        {
            (true, false, true, true),
            (true, true, true, true)
        },
        new object[]
        {
            (false, true, true, true),
            (true, false, true, true)
        },
        new object[]
        {
            (true, true, false, false),
            (true, true, false, false)
        },
        new object[]
        {
            (false, false, true, true),
            (true, false, true, true)
        }
    };

    [TestCaseSource(nameof(DetailedApplySuccessfullTestSource))]
    public void DetailedApply_WhenRulesSetIsSuccessful_ShouldReturnRight(
        (bool, bool, bool, bool) funcOutputs,
        (bool, bool, bool, bool) funcCalled)
    {
        var called11 = false;
        var called12 = false;
        var called21 = false;
        var called22 = false;

        var rulesSets = new CompiledRulesSet<TestModel>[]
        {
            new CompiledRulesSet<TestModel>(new CompiledRule<TestModel>[]
            {
                new (item => {called11 = true;  return funcOutputs.Item1; }, "code 1.1"),
                new (item => {called12 = true;  return funcOutputs.Item2; }, "code 1.2")
            }, "set 1"),
            new CompiledRulesSet<TestModel>(new CompiledRule<TestModel>[]
            {
                new (item => {called21 = true;  return funcOutputs.Item3; }, "code 2.1"),
                new (item => {called22 = true;  return funcOutputs.Item4; }, "code 2.2")
            }, "set 2")
        };

        var sut = new CompiledCatalog<TestModel>(rulesSets, "catalog name");

        var item = new AutoFaker<TestModel>().Generate();

        sut.DetailedApply(item).IsRight.Should().BeTrue();

        called11.Should().Be(funcCalled.Item1);
        called12.Should().Be(funcCalled.Item2);
        called21.Should().Be(funcCalled.Item3);
        called22.Should().Be(funcCalled.Item4);
    }

    internal static object[] FirstMatchingTestSource = new[]
    {
        new object[]
        {
            (false, false, false, false),
            Option<string>.None(),
            (true, false, true, false)
        },
        new object[]
        {
            (true, false, false, false),
            Option<string>.None(),
            (true, true, true, false)
        },
        new object[]
        {
            (false, true, false, false),
            Option<string>.None(),
            (true, false, true, false)
        },
        new object[]
        {
            (false, false, true, false),
            Option<string>.None(),
            (true, false, true, true)
        },
        new object[]
        {
            (false, false, false, true),
            Option<string>.None(),
            (true, false, true, false)
        },
        new object[]
        {
            (true, true, false, false),
            Option<string>.Some("set 1"),
            (true, true, false, false)
        },
        new object[]
        {
            (true, false, true, false),
            Option<string>.None(),
            (true, true, true, true)
        },
        new object[]
        {
            (false, true, true, false),
            Option<string>.None(),
            (true, false, true, true)
        },
        new object[]
        {
            (true, false, false, true),
            Option<string>.None(),
            (true, true, true, false)
        },
        new object[]
        {
            (false, true, false, true),
            Option<string>.None(),
            (true, false, true, false)
        },
        new object[]
        {
            (false, false, true, true),
            Option<string>.Some("set 2"),
            (true, false, true, true)
        },
        new object[]
        {
            (true, true, true, false),
            Option<string>.Some("set 1"),
            (true, true, false, false)
        },
        new object[]
        {
            (true, true, false, true),
            Option<string>.Some("set 1"),
            (true, true, false, false)
        },
        new object[]
        {
            (true, false, true, true),
            Option<string>.Some("set 2"),
            (true, true, true, true)
        },
        new object[]
        {
            (false, true, true, true),
            Option<string>.Some("set 2"),
            (true, false, true, true)
        },
        new object[]
        {
            (true, true, true, true),
            Option<string>.Some("set 1"),
            (true, true, false, false)
        }
    };

    [TestCaseSource(nameof(FirstMatchingTestSource))]
    public void FirstMatching_ShouldReturnExpectedValue(
            (bool, bool, bool, bool) funcOutputs,
            Option<string> expected,
            (bool, bool, bool, bool) funcCalled)
    {
        var called11 = false;
        var called12 = false;
        var called21 = false;
        var called22 = false;

        var rulesSets = new CompiledRulesSet<TestModel>[]
        {
            new CompiledRulesSet<TestModel>(new CompiledRule<TestModel>[]
            {
                new (item => {called11 = true;  return funcOutputs.Item1; }, "code 1.1"),
                new (item => {called12 = true;  return funcOutputs.Item2; }, "code 1.2")
            }, "set 1"),
            new CompiledRulesSet<TestModel>(new CompiledRule<TestModel>[]
            {
                new (item => {called21 = true;  return funcOutputs.Item3; }, "code 2.1"),
                new (item => {called22 = true;  return funcOutputs.Item4; }, "code 2.2")
            }, "set 2")
        };

        var sut = new CompiledCatalog<TestModel>(rulesSets, "catalog name");

        var item = new AutoFaker<TestModel>().Generate();

        sut.FirstMatching(item).Should().Be(expected);

        called11.Should().Be(funcCalled.Item1);
        called12.Should().Be(funcCalled.Item2);
        called21.Should().Be(funcCalled.Item3);
        called22.Should().Be(funcCalled.Item4);
    }
}
