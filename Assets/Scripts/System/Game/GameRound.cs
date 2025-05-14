using System.Linq;
using DG.Tweening;
using UnityEngine;

public interface IGameState
{
    public void Update();

    public void Enter();

    public void Exit();

    public void OnEnable();

    public void OnDisable();
}

public abstract class AbstractGameState : IGameState
{
    protected readonly GameRound GameRound;

    protected AbstractGameState(GameRound gameRound)
    {
        GameRound = gameRound;
    }

    public virtual void Update()
    {
    }

    public virtual void Enter()
    {
    }

    public virtual void Exit()
    {
    }

    public virtual void OnEnable()
    {
    }

    public virtual void OnDisable()
    {
    }
}

public class GameRound
{
    private GameConfiguration _configuration;
    private IGameState _state;
    public IGameState GameState => _state;

    public static GameRound Create(GameConfiguration configuration)
    {
        return new GameRound();
    }

    public void ChangeState(IGameState gameState)
    {
        _state?.Exit();
        _state = gameState;
        _state.Enter();
    }

    public void Update()
    {
        _state.Update();
    }
}

public class GameStatePlayerAiming : AbstractGameState
{
    public GameStatePlayerAiming(GameRound gameRound) : base(gameRound)
    {
    }

    public override void Enter()
    {
        GameSystemContainer.GetSystem<LauncherSystem>().IsLaunchEnabled = true;
        Subscribe();

        GameSystemContainer.GetSystem<ObstacleSystem>().Spawn();
    }

    public override void OnEnable()
    {
        Subscribe();
    }

    public override void OnDisable()
    {
        Unsubscribe();
    }

    public override void Exit()
    {
        GameSystemContainer.GetSystem<LauncherSystem>().IsLaunchEnabled = false;
        Unsubscribe();
    }

    private void Subscribe()
    {
        var launcherSystem = GameSystemContainer.GetSystem<LauncherSystem>();
        launcherSystem.OnPlayerLaunch += OnPlayerLaunch;

        Debug.Log("GameStatePlayerAiming Subscribe");
    }

    private void Unsubscribe()
    {
        var launcherSystem = GameSystemContainer.GetSystem<LauncherSystem>();
        launcherSystem.OnPlayerLaunch -= OnPlayerLaunch;

        Debug.Log("GameStatePlayerAiming Unsubscribe ");
    }

    private void OnPlayerLaunch()
    {
        Debug.Log("Player Launch");

        GameRound.ChangeState(new GameStatePlayerBulletFly(GameRound));
    }
}

public class GameStatePlayerBulletFly : AbstractGameState
{
    public GameStatePlayerBulletFly(GameRound gameRound) : base(gameRound)
    {
    }

    public override void Update()
    {
        if (GameSystemContainer.GetSystem<RecycleSystem>().Count >=
            GameSystemContainer.GetSystem<LauncherSystem>().LaunchedCount)
        {
            GameRound.ChangeState(new GameStateItemAttack(GameRound));

            Debug.Log("Bullet Fly Done");
        }
    }
}

public class GameStateItemAttack : AbstractGameState
{
    public GameStateItemAttack(GameRound gameRound) : base(gameRound)
    {
    }

    public override void Enter()
    {
        var gameGrid = GameSystemContainer.GetSystem<GameGridSystem>();
        var grids = gameGrid.Grids;

        //判断是否失败
        if (grids[grids.Rows - 1].Any(grid => grid.Entity))
        {
            GameRound.ChangeState(new GameStateGameOver(GameRound));
            return;
        }

        // 创建一个 sequence，所有 tween 会并行执行
        var sequence = DOTween.Sequence();

        for (int i = grids.Rows - 1; i > 0; i--)
        {
            for (int j = 0; j < grids.Cols; j++)
            {
                // 为每个循环变量创建本地副本
                var currentGrid = grids[i][j];
                var targetGrid = grids[currentGrid.X, currentGrid.Y - 1];
                var obstacle = targetGrid.Entity;
                if (!targetGrid.Entity)
                {
                    continue;
                }

                currentGrid.Entity = obstacle;
                targetGrid.Entity = null;

                sequence.Join(obstacle.transform.DOMove(currentGrid.transform.position, 0.5F).SetEase(Ease.OutQuad));
            }
        }

        sequence
            .OnComplete(() =>
            {
                GameSystemContainer.GetSystem<ObstacleSystem>().Spawn();
                GameRound.ChangeState(new GameStatePlayerAiming(GameRound));
            })
            .Play();
    }
}

public class GameStateGameOver : AbstractGameState
{
    public GameStateGameOver(GameRound gameRound) : base(gameRound)
    {
    }

    public override void Enter()
    {
        //exit game
        Debug.Log("Game Over");
    }
}
