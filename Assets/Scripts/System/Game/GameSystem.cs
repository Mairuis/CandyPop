using System;
using UnityEngine;

/// <summary>
/// 游戏核心逻辑实现
/// 负责状态转移调度，状态更新
/// </summary>
public class GameSystem : MonoBehaviour
{
    [SerializeField] public GameConfiguration configuration;

    private GameRound _currentRound;

    private void Awake()
    {
        GameSystemContainer.GetSystem<ObstacleSystem>().configuration = configuration;
        _currentRound = GameRound.Create(configuration);
        _currentRound.ChangeState(new GameStatePlayerAiming(_currentRound));

        Debug.Log("GameSystem Awake");
    }

    private void OnEnable()
    {
        _currentRound.GameState.OnEnable();

        Debug.Log("GameSystem OnEnable");
    }

    private void OnDisable()
    {
        _currentRound.GameState.OnDisable();

        Debug.Log("GameSystem OnDisable");
    }

    private void Update()
    {
        _currentRound.Update();
    }

}
