using LogicEngine.Internals;

namespace LogicEngine.Unit.Tests.Internals;
public class StaticSharedTests
{
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    public void Identity_ShouldAlwaysReturnSameObject(int value)
    {
        var input = new TestModel
        {
            IntProperty = value
        };

        StaticShared.Functions<TestModel>.Identity(input)
            .Should().Be(input);
    }

    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    public void AlwaysTrue_ShouldAlwaysReturnTrue(int value)
    {
        var input = new TestModel
        {
            IntProperty = value
        };

        StaticShared.Functions<TestModel>.AlwaysTrue(input)
            .Should().BeTrue();
    }

    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    public void Constant_ShouldAlwaysReturnOutputObject(int value)
    {
        var input = new TestModel
        {
            IntProperty = value
        };
        var output = new
        {
            Property = "something"
        };

        StaticShared.Functions<TestModel, object>.Constant(output)(input)
            .Should().Be(output);
    }

    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    public void AlwaysRightEitherUnitWith2Generics_ShouldAlwaysReturnRight(int value)
    {
        var input = new TestModel
        {
            IntProperty = value
        };

        StaticShared.Functions<TestModel, object>.AlwaysRightEitherUnit(input)
            .IsRight.Should().BeTrue();
    }
}
