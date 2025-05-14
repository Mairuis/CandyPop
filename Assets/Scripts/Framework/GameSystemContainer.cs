using System.Collections.Generic;

public interface IGameSystem
{

}

public class GameSystemContainer
{
    private static GameSystemContainer _instance;

    public static GameSystemContainer Instance
    {
        get
        {
            if (_instance != null) return _instance;
            lock (typeof(GameSystemContainer))
            {
                _instance ??= new GameSystemContainer();
            }
            return _instance;
        }
    }

    private readonly Dictionary<string, IGameSystem> _systems = new();

    public void Register(IGameSystem system)
    {
        _systems[system.GetType().Name] = system;
    }

    public void Unregister(IGameSystem system)
    {
        _systems.Remove(system.GetType().Name);
    }

    public IGameSystem GetSystem(string name)
    {
        return _systems[name];
    }

    public T GetSystemByType<T>() where T : IGameSystem
    {
        return (T)_systems[typeof(T).Name];
    }

    public static T GetSystem<T>() where T : IGameSystem
    {
        return Instance.GetSystemByType<T>();
    }
}
