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

        rule.Apply(It.IsAny<TestModel>()).Should().Be(returnValue);

        timesCalled.Should().Be(1);
    }

    [Test]
    public void DetailedApply_WhenFuncReturnsTrue_ShouldReturnRight()
    {
        var timesCalled = 0;

        var rule = new CompiledRule<TestModel>(item => true.Tee(b => timesCalled++), "some code");

        rule.DetailedApply(It.IsAny<TestModel>()).IsRight.Should().BeTrue();

        timesCalled.Should().Be(1);
    }

    [Test]
    public void DetailedApply_WhenFuncReturnsFalse_ShouldReturnLeftWithExpectedCode()
    {
        var timesCalled = 0;

        var rule = new CompiledRule<TestModel>(item => false.Tee(b => timesCalled++), "some code");

        rule.DetailedApply(It.IsAny<TestModel>()).Tee(e => e.IsLeft.Should().BeTrue()).OnLeft(s => s.Should().Be("some code"));

        timesCalled.Should().Be(1);
    }
}
