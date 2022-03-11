using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using LogicEngine.Interfaces;
using LogicEngine.Models;
using Moq;
using NUnit.Framework;
using TinyFp;
using TinyFp.Extensions;
using static System.Array;

namespace LogicEngine.Unit.Tests;

[TestFixture]
public class RulesManagerTests
{
    private Mock<IRulesCatalogCompiler> _mockCompiler;
    private RulesManager<TestModel> _sut;
    private RulesCatalog _catalog;

    [SetUp]
    public void SetUp()
    {
        _catalog = new RulesCatalog(new RulesSet[] { new(new[] { new Rule("property", Internals.OperatorType.Contains, "value", "code") }) }, "name");
        _mockCompiler = new Mock<IRulesCatalogCompiler>();
        _sut = new RulesManager<TestModel>(_mockCompiler.Object);
    }

    [Test]
    public void ItemSatisfiesRules_WhenCompilerReturnsNoElements_ShouldReturnTrue()
    {
        _mockCompiler.Setup(_ => _.CompileCatalog<TestModel>(_catalog))
            .Returns(new CompiledCatalog<TestModel>(Empty<Func<TestModel, Either<string, TinyFp.Unit>>[]>()));

        var item = new TestModel();

        _sut.Catalog = _catalog;

        var result = _sut.ItemSatisfiesRules(item);

        result.Should().BeTrue();

        var resultWithMessage = _sut.ItemSatisfiesRulesWithMessage(item);

        resultWithMessage.IsRight.Should().BeTrue();
    }

    [Test]
    public void ItemSatisfiesRules_WhenCompilerReturnSuccessfulElements_ShouldReturnCorrespondingResult()
    {
        _mockCompiler.Setup(_ => _.CompileCatalog<TestModel>(_catalog))
            .Returns(new CompiledCatalog<TestModel>(new[]
            {
                new Func<TestModel, Either<string, TinyFp.Unit>>[]
                {
                    _ => Either<string, TinyFp.Unit>.Right(TinyFp.Unit.Default),
                }
            }));

        var item = new TestModel();

        _sut.Catalog = _catalog;

        var result = _sut.ItemSatisfiesRules(item);

        result.Should().BeTrue();

        var resultWithMessage = _sut.ItemSatisfiesRulesWithMessage(item);

        resultWithMessage.IsRight.Should().BeTrue();
    }

    [Test]
    public void ItemSatisfiesRules_WhenCompilerReturnFailingElements_ShouldReturnCorrespondingResult()
    {
        _mockCompiler.Setup(_ => _.CompileCatalog<TestModel>(_catalog))
            .Returns(new CompiledCatalog<TestModel>(new[]
            {
                new Func<TestModel, Either<string, TinyFp.Unit>>[]
                {
                    _ => Either<string, TinyFp.Unit>.Left("code")
                }
            }));

        var item = new TestModel();

        _sut.Catalog = _catalog;

        var result = _sut.ItemSatisfiesRules(item);

        result.Should().BeFalse();

        var resultWithMessage = _sut.ItemSatisfiesRulesWithMessage(item);

        resultWithMessage.IsLeft.Should().BeTrue();

        resultWithMessage.OnLeft(_ => _.Should().BeEquivalentTo("code"));
    }

    [Test]
    public void ItemSatisfiesRulesWithMessage_WhenCompilerReturnsLeftValuedFunctionsWithEmptyCode_ShouldReturnEmptyCored()
    {
        _mockCompiler.Setup(_ => _.CompileCatalog<TestModel>(_catalog))
            .Returns(new CompiledCatalog<TestModel>(new[]
            {
                new Func<TestModel, Either<string, TinyFp.Unit>>[]
                {
                    _ => Either<string, TinyFp.Unit>.Left(""),
                }
            }));

        var item = new TestModel();

        _sut.Catalog = _catalog;

        var resultWithMessage = _sut.ItemSatisfiesRulesWithMessage(item);

        resultWithMessage.IsLeft.Should().BeTrue();

        resultWithMessage.OnLeft(_ => _.Should().BeEmpty());
    }

    [Test]
    public void When_FilterOnSetOfMatchingItems_ShouldReturnEquivalentResult()
    {
        _mockCompiler.Setup(_ => _.CompileCatalog<TestModel>(_catalog))
            .Returns(new CompiledCatalog<TestModel>(new[]
            {
                new Func<TestModel, Either<string, TinyFp.Unit>>[]
                {
                    _ => _.IntEnumerableProperty.Contains(25).ToOption(b => !b).Match(_ => TinyFp.Unit.Default,
                        () => Either<string, TinyFp.Unit>.Left(string.Empty)),
                    _ => _.IntEnumerableProperty.Contains(28).ToOption(b => !b).Match(_ => TinyFp.Unit.Default,
                        () => Either<string, TinyFp.Unit>.Left(string.Empty))
                },
                new Func<TestModel, Either<string, TinyFp.Unit>>[]
                {
                    _ => _.IntEnumerableProperty.Contains(27).ToOption(b => !b).Match(_ => TinyFp.Unit.Default,
                        () => Either<string, TinyFp.Unit>.Left(string.Empty))
                }
            }));

        var items = new List<TestModel>
        {
            new()
            {
                IntEnumerableProperty = new[]
                {
                    25,
                    28
                }
            },
            new()
            {
                IntEnumerableProperty = new[]
                {
                    27
                }
            }
        };

        var result = _sut.Filter(items);

        result.Should().BeEquivalentTo(items);
    }

    [Test]
    public void When_FilterOnSetOfSomeMatchingItems_ShouldFilterThemOut()
    {
        _mockCompiler.Setup(_ => _.CompileCatalog<TestModel>(_catalog))
            .Returns(new CompiledCatalog<TestModel>(new[]
            {
                new Func<TestModel, Either<string, TinyFp.Unit>>[]
                {
                    _ => _.IntEnumerableProperty.Contains(25).ToOption(b => !b).Match(_ => TinyFp.Unit.Default,
                        () => Either<string, TinyFp.Unit>.Left(string.Empty)),
                    _ => _.IntEnumerableProperty.Contains(28).ToOption(b => !b).Match(_ => TinyFp.Unit.Default,
                        () => Either<string, TinyFp.Unit>.Left(string.Empty))
                },
                new Func<TestModel, Either<string, TinyFp.Unit>>[]
                {
                    _ => _.IntEnumerableProperty.Contains(27).ToOption(b => !b).Match(_ => TinyFp.Unit.Default,
                        () => Either<string, TinyFp.Unit>.Left(string.Empty))
                }
            }));

        _sut.Catalog = _catalog;
        var items = new List<TestModel>
        {
            new()
            {
                IntEnumerableProperty = new[]
                {
                    28
                }
            },
            new()
            {
                IntEnumerableProperty = new[]
                {
                    27
                }
            }
        };

        var result = _sut.Filter(items);

        result.Should().BeEquivalentTo(new List<TestModel>
        {
            new()
            {
                IntEnumerableProperty = new[]
                {
                    27
                }
            }
        }, options => options.ComparingByMembers<TestModel>());
    }

    [Test]
    public void When_FirstOrDefaultOnSetOfSomeMatchingItems_ShouldReturnFirst()
    {
        _mockCompiler.Setup(_ => _.CompileCatalog<TestModel>(_catalog))
            .Returns(new CompiledCatalog<TestModel>(new[]
            {
                new Func<TestModel, Either<string, TinyFp.Unit>>[]
                {
                    _ => _.IntEnumerableProperty.Contains(25).ToOption(b => !b).Match(_ => TinyFp.Unit.Default,
                        () => Either<string, TinyFp.Unit>.Left(string.Empty)),
                    _ => _.IntEnumerableProperty.Contains(28).ToOption(b => !b).Match(_ => TinyFp.Unit.Default,
                        () => Either<string, TinyFp.Unit>.Left(string.Empty))
                },
                new Func<TestModel, Either<string, TinyFp.Unit>>[]
                {
                    _ => _.IntEnumerableProperty.Contains(27).ToOption(b => !b).Match(_ => TinyFp.Unit.Default,
                        () => Either<string, TinyFp.Unit>.Left(string.Empty))
                }
            }));
        _sut.Catalog = _catalog;
        var items = new List<TestModel>
        {
            new()
            {
                IntEnumerableProperty = new[]
                {
                    28
                }
            },
            new()
            {
                IntEnumerableProperty = new[]
                {
                    27
                }
            },
            new()
            {
                IntEnumerableProperty = new[]
                {
                    25,
                    28
                }
            }
        };

        var result = _sut.FirstOrDefault(items);

        result.Should().BeEquivalentTo(new TestModel
        {
            IntEnumerableProperty = new[]
            {
                27
            }
        }, options => options.ComparingByMembers<TestModel>());
    }

    [Test]
    public void When_FirstOrDefaultOnSetOfNonMatchingItems_ShouldReturnDefault()
    {
        _mockCompiler.Setup(_ => _.CompileCatalog<TestModel>(_catalog))
            .Returns(new CompiledCatalog<TestModel>(new[]
            {
                new Func<TestModel, Either<string, TinyFp.Unit>>[]
                {
                    _ => _.IntEnumerableProperty.Contains(25).ToOption(b => !b).Match(_ => TinyFp.Unit.Default,
                        () => Either<string, TinyFp.Unit>.Left(string.Empty)),
                    _ => _.IntEnumerableProperty.Contains(28).ToOption(b => !b).Match(_ => TinyFp.Unit.Default,
                        () => Either<string, TinyFp.Unit>.Left(string.Empty))
                },
                new Func<TestModel, Either<string, TinyFp.Unit>>[]
                {
                    _ => _.IntEnumerableProperty.Contains(27).ToOption(b => !b).Match(_ => TinyFp.Unit.Default,
                        () => Either<string, TinyFp.Unit>.Left(string.Empty))
                }
            }));
        _sut.Catalog = _catalog;
        var items = new List<TestModel>
        {
            new()
            {
                IntEnumerableProperty = new[]
                {
                    28
                }
            },
            new()
            {
                IntEnumerableProperty = new[]
                {
                    26
                }
            },
            new()
            {
                IntEnumerableProperty = new[]
                {
                    25,
                    29
                }
            }
        };

        var result = _sut.FirstOrDefault(items);

        result.Should().BeEquivalentTo(default(TestModel), options => options.ComparingByMembers<TestModel>());
    }
}