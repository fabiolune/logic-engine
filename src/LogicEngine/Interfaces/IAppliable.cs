namespace LogicEngine.Interfaces;

public interface IAppliable<in T> where T : new()
{
    bool Apply(T item);
}
