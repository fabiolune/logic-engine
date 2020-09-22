using System.Runtime.Serialization;
using Fabiolune.BusinessRulesEngine.Internals;
using Newtonsoft.Json;

namespace Fabiolune.BusinessRulesEngine.Models
{
    public class Rule
    {
        [JsonConstructor]
        public Rule(string property, OperatorType @operator, string value)
        {
            Property = property;
            Operator = @operator;
            Value = value;
        }

        public Rule(string property, OperatorType @operator, string value, string code, string description) : this(property, @operator, value)
        {
            Code = code;
            Description = description;
        }

        [DataMember(Name = "property")]
        public string Property { get; set; }
        [DataMember(Name = "operator")]
        public OperatorType Operator { get; set; }
        [DataMember(Name = "value")]
        public string Value { get; set; }
        [DataMember(Name = "description")]
        public string Description { get; set; }
        [DataMember(Name = "code")]
        public string Code { get; set; }
        public override string ToString() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}