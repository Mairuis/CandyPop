using UnityEngine;

public class GameSystemRegister : MonoBehaviour
{
    [SerializeField] private GameObject[] systems;

    void Awake()
    {
        foreach (var system in systems)
        {
            var components = system.GetComponents<Component>();
            foreach (var component in components)
            {
                if (component is IGameSystem gameSystem)
                {
                    GameSystemContainer.Instance.Register(gameSystem);
                }
            }
        }
    }
}
