namespace LogicEngine.Interfaces;

public interface IAppliable<T> where T : new()
{
    bool Apply(T item);
}
