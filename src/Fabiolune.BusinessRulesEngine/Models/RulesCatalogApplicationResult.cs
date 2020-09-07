using System.Collections.Generic;

namespace Fabiolune.BusinessRulesEngine.Models
{
    public struct RulesCatalogApplicationResult
    {
        public static readonly RulesCatalogApplicationResult Successful =
            new RulesCatalogApplicationResult(true);
        
        public static RulesCatalogApplicationResult Failed(IEnumerable<string> codes)
            => new RulesCatalogApplicationResult(codes);

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