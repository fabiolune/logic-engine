//using FluentAssertions;
//using LogicEngine.Interfaces.Compilers;
//using LogicEngine.Managers;
//using LogicEngine.Models;
//using Moq;
//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using TinyFp;

//namespace LogicEngine.Unit.Tests;
//public class CompositeRulesSetManagerTests
//{
//    private Mock<IRulesSetCompiler> _compiler;
//    private CompositeRulesSetManager<TestModel, string> _sut;
//    private static readonly RulesSet FirstSet = new(new[] { new Rule("prop1", Internals.OperatorType.None, "val1", "code1")});
//    private static readonly RulesSet SecondSet = new(new[] { new Rule("prop2", Internals.OperatorType.None, "val2", "code2") });
//    private static readonly IEnumerable<(string, RulesSet)> Data = new[]
//    {
//        ("key 1", FirstSet),
//        ("key 2", SecondSet)
//    };

//    [SetUp]
//    public void SetUp()
//    {
//        _compiler = new Mock<IRulesSetCompiler>();
//        _sut = new CompositeRulesSetManager<TestModel, string>(_compiler.Object);
//    }

//    internal static object[] TestCases = new[]
//    {
//        new object[]
//        {
//            new Func<TestModel, Either<string, TinyFp.Unit>>[]
//            {
//                tm => Either<string, TinyFp.Unit>.Right(TinyFp.Unit.Default)
//            },
//            new Func<TestModel, Either<string, TinyFp.Unit>>[]
//            {
//                tm => Either<string, TinyFp.Unit>.Left("some value")
//            },
//            Option<string>.Some("key 1")
//        },
//        new object[]
//        {
//            new Func<TestModel, Either<string, TinyFp.Unit>>[]
//            {
//                tm => Either<string, TinyFp.Unit>.Right(TinyFp.Unit.Default),
//                tm => Either<string, TinyFp.Unit>.Left("some value")
//            },
//            new Func<TestModel, Either<string, TinyFp.Unit>>[]
//            {
//                tm => Either<string, TinyFp.Unit>.Left("some value")
//            },
//            Option<string>.None()
//        },
//        new object[]
//        {
//            new Func<TestModel, Either<string, TinyFp.Unit>>[]
//            {
//                tm => Either<string, TinyFp.Unit>.Right(TinyFp.Unit.Default),
//                tm => Either<string, TinyFp.Unit>.Left("some value"),
//                tm => Either<string, TinyFp.Unit>.Right(TinyFp.Unit.Default)
//            },
//            new Func<TestModel, Either<string, TinyFp.Unit>>[]
//            {
//                tm => Either<string, TinyFp.Unit>.Left("some value")
//            },
//            Option<string>.None()
//        },
//        new object[]
//        {
//            new Func<TestModel, Either<string, TinyFp.Unit>>[]
//            {
//                tm => Either<string, TinyFp.Unit>.Left("some value")
//            },
//            new Func<TestModel, Either<string, TinyFp.Unit>>[]
//            {
//                tm => Either<string, TinyFp.Unit>.Right(TinyFp.Unit.Default)
//            },
//            Option<string>.Some("key 2")
//        },
//        new object[]
//        {
//            new Func<TestModel, Either<string, TinyFp.Unit>>[]
//            {
//                tm => Either<string, TinyFp.Unit>.Left("some value")
//            },
//            new Func<TestModel, Either<string, TinyFp.Unit>>[]
//            {
//                tm => Either<string, TinyFp.Unit>.Left("some value"),
//                tm => Either<string, TinyFp.Unit>.Right(TinyFp.Unit.Default)
//            },
//            Option<string>.None()
//        },
//        new object[]
//        {
//            new Func<TestModel, Either<string, TinyFp.Unit>>[]
//            {
//                tm => Either<string, TinyFp.Unit>.Left("some value")
//            },
//            new Func<TestModel, Either<string, TinyFp.Unit>>[]
//            {
//                tm => Either<string, TinyFp.Unit>.Left("some value"),
//                tm => Either<string, TinyFp.Unit>.Right(TinyFp.Unit.Default),
//                tm => Either<string, TinyFp.Unit>.Left("some value")
//            },
//            Option<string>.None()
//        },
//        new object[]
//        {
//            new Func<TestModel, Either<string, TinyFp.Unit>>[]
//            {
//                tm => Either<string, TinyFp.Unit>.Right(TinyFp.Unit.Default)
//            },
//            new Func<TestModel, Either<string, TinyFp.Unit>>[]
//            {
//                tm => Either<string, TinyFp.Unit>.Right(TinyFp.Unit.Default)
//            },
//            Option<string>.Some("key 1")
//        },
//        new object[]
//        {
//            new Func<TestModel, Either<string, TinyFp.Unit>>[]
//            {
//                tm => Either<string, TinyFp.Unit>.Left("some value"),
//                tm => Either<string, TinyFp.Unit>.Right(TinyFp.Unit.Default)
//            },
//            new Func<TestModel, Either<string, TinyFp.Unit>>[]
//            {
//                tm => Either<string, TinyFp.Unit>.Right(TinyFp.Unit.Default)
//            },
//            Option<string>.Some("key 2")
//        },
//        new object[]
//        {
//            new Func<TestModel, Either<string, TinyFp.Unit>>[]
//            {
//                tm => Either<string, TinyFp.Unit>.Left("some value"),
//                tm => Either<string, TinyFp.Unit>.Right(TinyFp.Unit.Default),
//                tm => Either<string, TinyFp.Unit>.Left("some value")
//            },
//            new Func<TestModel, Either<string, TinyFp.Unit>>[]
//            {
//                tm => Either<string, TinyFp.Unit>.Right(TinyFp.Unit.Default)
//            },
//            Option <string>.Some("key 2")
//        },
//        new object[]
//        {
//            new Func<TestModel, Either<string, TinyFp.Unit>>[]
//            {
//                tm => Either<string, TinyFp.Unit>.Left("some value 1")
//            },
//            new Func<TestModel, Either<string, TinyFp.Unit>>[]
//            {
//                tm => Either<string, TinyFp.Unit>.Left("some value 2")
//            },
//            Option<string>.None()
//        }
//    };

//    [TestCaseSource(nameof(TestCases))]
//    public void FirstMatching_ShouldReturnCorrespondingKeyOrNone(
//        Func<TestModel, Either<string, TinyFp.Unit>>[] firstExecutables,
//        Func<TestModel, Either<string, TinyFp.Unit>>[] secondExecutables,
//        Option<string> expectedResult
//        )
//    {
//        _compiler
//            .Setup(c => c.Compile<TestModel>(FirstSet))
//            .Returns(new CompiledRulesSet<TestModel>(firstExecutables));
//        _compiler
//            .Setup(c => c.Compile<TestModel>(SecondSet))
//            .Returns(new CompiledRulesSet<TestModel>(secondExecutables));

//        _sut.Set = Data;
//        var result = _sut.FirstMatching(new TestModel());

//        result.Should().Be(expectedResult);
//    }

//    [Test]
//    public void FirstMatching_WhenNullDataArePassed_ShouldAlwaysReturnNone()
//    {
//        _sut.Set = null;
//        var result = _sut.FirstMatching(new TestModel());

//        result
//            .Should()
//            .Be(Option<string>.None());
//    }

//    [Test]
//    public void FirstMatching_WhenEmptyDataArePassed_ShouldAlwaysReturnNone()
//    {
//        _sut.Set = Array.Empty<(string, RulesSet)>();
//        var result = _sut.FirstMatching(new TestModel());

//        result
//            .Should()
//            .Be(Option<string>.None());
//    }


//}
