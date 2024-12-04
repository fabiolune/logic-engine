namespace LogicEngine.Interfaces;

public interface IAppliable<in T> where T : new()
{
    /// <summary>
    /// Applies the item to the appliable
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    bool Apply(T item);
}
