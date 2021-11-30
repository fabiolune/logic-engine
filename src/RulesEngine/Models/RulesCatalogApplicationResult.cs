using System.Collections.Generic;

namespace RulesEngine.Models
{
    public struct RulesCatalogApplicationResult
    {
        public static readonly RulesCatalogApplicationResult Successful = new(true);
        
        public static RulesCatalogApplicationResult Failed(IEnumerable<string> codes) => new(codes);

        private RulesCatalogApplicationResult(bool success)
        {
            Success = success;
            FailedCodes = default;
        }

        private RulesCatalogApplicationResult(IEnumerable<string> codes)
        {
            Success = false;
            FailedCodes = codes;
        }

        public bool Success { get;}
        public IEnumerable<string> FailedCodes { get; }
    }
}