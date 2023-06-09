//using LogicEngine.Interfaces.Compilers;
//using LogicEngine.Interfaces.Managers;
//using LogicEngine.Models;
//using System;
//using TinyFp;
//using TinyFp.Extensions;

//namespace LogicEngine.Managers;

//public class SingleRuleManager<T> : ISingleRuleManager<T> where T : new()
//{
//    private static readonly Func<T, Either<string, Unit>> AlwaysSatisfiedBy = _ => Unit.Default;
//    private readonly ISingleRuleCompiler _compiler;
//    private Func<T, Either<string, Unit>> _isSatisfiedBy = AlwaysSatisfiedBy;

//    public SingleRuleManager(ISingleRuleCompiler compiler) => _compiler = compiler;

//    public Rule Rule
//    {
//        set =>
//            _isSatisfiedBy =
//                value
//                    .Map(_compiler.Compile<T>)
//                    .Map(cr => cr.Executable)
//                    .OrElse(AlwaysSatisfiedBy);
//    }

//    public Either<string, Unit> IsSatisfiedBy(T item) => _isSatisfiedBy(item);
//}