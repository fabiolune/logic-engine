using System.Collections.Generic;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RulesEngine.Extensions;
using RulesEngine.Interfaces;
using RulesEngine.Models;
using TinyFp;

namespace RulesEngine.Unit.Tests.Extensions
{
    public class ExtensionsTests
    {
        private Mock<IRulesManager<TestModel>> _mockManager;

        [SetUp]
        public void SetUp()
        {
            _mockManager = new Mock<IRulesManager<TestModel>>();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ItemSatisfiesRules_ShouldInvokeManagerItemSatisfiesRulesAndReturnItsResult(bool expectation)
        {
            var item = new TestModel();
            _mockManager.Setup(_ => _.ItemSatisfiesRules(item)).Returns(expectation);

            var result = item.SatisfiesRules(_mockManager.Object);

            result.Should().Be(expectation);
            _mockManager.Verify(_ => _.ItemSatisfiesRules(item), Times.Once);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ItemSatisfiesRulesWithMessage_ShouldInvokeManagerItemSatisfiesRulesWithMessageAndReturnsItsResult(
            bool satisfy)
        {
            var expected = satisfy
                ? TinyFp.Unit.Default
                : Either<IEnumerable<string>, TinyFp.Unit>.Left(new []{ "a", "b" });

            var item = new TestModel();
            _mockManager.Setup(_ => _.ItemSatisfiesRulesWithMessage(item)).Returns(expected);

            var result = item.SatisfiesRulesWithMessage(_mockManager.Object);

            result.Should().Be(expected);
            _mockManager.Verify(_ => _.ItemSatisfiesRulesWithMessage(item), Times.Once);
        }
    }
}