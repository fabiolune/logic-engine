using System.Collections.Generic;
using Fabiolune.BusinessRulesEngine.Extensions;
using Fabiolune.BusinessRulesEngine.Interfaces;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Fabiolune.BusinessRulesEngine.Unit.Tests.Extensions
{
    public class EnumerableExtensionsTests
    {
        private readonly Mock<IBusinessRulesManager<TestModel>> _mockManager = new Mock<IBusinessRulesManager<TestModel>>();

        [Test]
        public void Filter_ShouldInvokeManagerFilterAndReturnItsResult()
        {
            var expectation = new List<TestModel>
            {
                new TestModel
                {
                    IntProperty = 0
                }
            };
            var items = new List<TestModel>
            {
                new TestModel(),
                new TestModel(),
                new TestModel()
            };
            _mockManager.Setup(_ => _.Filter(items)).Returns(expectation);


            var result = items.Filter(_mockManager.Object);

            result.Should().BeEquivalentTo(expectation);
            _mockManager.Verify(_ => _.Filter(items), Times.Once);
        }

        [Test]
        public void FirstOrDefault_ShouldInvokeManagerFirstOrDefaultAndReturnItsResult()
        {
            var expectation = new TestModel
            {
                IntProperty = 0
            };
            var items = new List<TestModel>
            {
                new TestModel(),
                new TestModel(),
                new TestModel()
            };
            _mockManager.Setup(_ => _.FirstOrDefault(items)).Returns(expectation);


            var result = items.FirstOrDefault(_mockManager.Object);

            result.Should().BeEquivalentTo(expectation);
            _mockManager.Verify(_ => _.FirstOrDefault(items), Times.Once);
        }
    }
}