using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class ObstacleSystem : MonoBehaviour, IGameSystem
{
    [SerializeField] private Obstacle obstacle;
    [SerializeField] public GameConfiguration configuration;

    public void Spawn()
    {
        var gameGridSystem = GameSystemContainer.GetSystem<GameGridSystem>();
        var matrix = gameGridSystem.Grids;
        foreach (var bottomGrid in matrix[0])
        {
            if (!bottomGrid.Entity && UnityEngine.Random.Range(0, 2) == 1)
            {
                var newObstacle = Instantiate(obstacle, bottomGrid.transform.position, Quaternion.identity);
                newObstacle.GetComponent<CandyObstacle>().SetHp(UnityEngine.Random.Range(1, 3));
                bottomGrid.Entity = newObstacle.gameObject;
            }
        }
    }
}
