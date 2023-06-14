namespace LogicEngine.Interfaces;

public interface IApplyable<T> where T : new()
{
    bool Apply(T item);
}
