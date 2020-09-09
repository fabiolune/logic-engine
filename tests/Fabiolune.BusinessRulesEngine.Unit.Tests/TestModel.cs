using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Fabiolune.BusinessRulesEngine.Unit.Tests
{
    [ExcludeFromCodeCoverage]
    internal struct TestModel
    {
        public TestEnum EnumProperty { get; set; }
        public string StringProperty { get; set; }
        public string StringProperty2 { get; set; }
        public int IntProperty { get; set; }
        public int IntProperty2 { get; set; }
        public double DoubleProperty { get; set; }
        public Dictionary<string, string> StringStringDictionaryProperty { get; set; }
        public IEnumerable<int> IntEnumerableProperty { get; set; }
        public IEnumerable<int> IntEnumerableProperty2 { get; set; }
        public IEnumerable<string> StringEnumerableProperty { get; set; }
        public string[] StringArrayProperty { get; set; }
        public string[] StringArrayProperty2 { get; set; }
    }
}