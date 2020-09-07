using System;
using Fabiolune.BusinessRulesEngine.Models;

namespace Fabiolune.BusinessRulesEngine.Interfaces
{
    public interface IBusinessRulesRetriever<T> where T : new()
    {
        RulesCatalog GetCatalog();
        event EventHandler<RulesCatalog> CatalogChanged;
    }
}