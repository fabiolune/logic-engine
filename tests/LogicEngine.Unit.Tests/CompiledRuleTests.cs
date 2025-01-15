using AutoBogus;
using TinyFp.Extensions;

namespace LogicEngine.Unit.Tests;
public class CompiledRuleTests
{
    [TestCase(true)]
    [TestCase(false)]
    public void Apply_WhenFuncReturnsSpecificBool_ShouldReturnThatBool(bool returnValue)
    {
        var timesCalled = 0;

        var rule = new CompiledRule<TestModel>(item => returnValue.Tee(b => timesCalled++), "some code");

        var item = new AutoFaker<TestModel>().Generate();

        rule.Apply(item).ShouldBe(returnValue);

        timesCalled.ShouldBe(1);
    }

    [Test]
    public void DetailedApply_WhenFuncReturnsTrue_ShouldReturnRight()
    {
        var timesCalled = 0;

        var rule = new CompiledRule<TestModel>(item => true.Tee(b => timesCalled++), "some code");

        var item = new AutoFaker<TestModel>().Generate();

        rule.DetailedApply(item).IsRight.ShouldBeTrue();

        timesCalled.ShouldBe(1);
    }

    [Test]
    public void DetailedApply_WhenFuncReturnsFalse_ShouldReturnLeftWithExpectedCode()
    {
        var timesCalled = 0;

        var rule = new CompiledRule<TestModel>(item => false.Tee(b => timesCalled++), "some code");

        var item = new AutoFaker<TestModel>().Generate();

        rule.DetailedApply(item).Tee(e => e.IsLeft.ShouldBeTrue()).OnLeft(s => s.ShouldBe("some code"));

        timesCalled.ShouldBe(1);
    }
}
