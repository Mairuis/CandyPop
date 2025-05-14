using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;
using DG.Tweening;

public class RecycleSystem : MonoBehaviour, IGameSystem
{
    [SerializeField] private Transform[] _leftWaypoints;
    [SerializeField] private Transform[] _rightWaypoints;
    [SerializeField] private float _projectileMoveDuration = 1f; // 整体运动时间
    [SerializeField] private Ease _projectileMoveEase = Ease.Linear; // 缓动曲线
    [SerializeField] private RecycleBox _recycleBox;

    // 定义输出事件
    public event Action<int> OnProjectileRecycled;
    public event Action<int> OnCountChanged;
    public event Action<GameObject> OnProjectileRecycleStarted;
    public event Action<GameObject> OnProjectileRecycleCompleted;

    private Dictionary<GameObject, Tween> _recyclingProjectiles = new Dictionary<GameObject, Tween>();

    private int _count;
    public int Count
    {
        get => _count;
        private set
        {
            if (_count != value)
            {
                _count = value;
                OnCountChanged?.Invoke(_count);
            }
        }
    }

    private void Awake()
    {
        // 查找场景中的所有RecycleBox并订阅它们的事件
        if (_recycleBox == null)
        {
            _recycleBox = FindFirstObjectByType<RecycleBox>();
        }
    }

    private void OnEnable()
    {
        // 订阅RecycleBox的事件
        if (_recycleBox != null)
        {
            _recycleBox.OnProjectileEntered += StartRecycleProjectile;
        }
        else
        {
            Debug.LogWarning("RecycleBox reference is missing. RecycleSystem will not function properly.");
        }
    }

    private void OnDisable()
    {
        // 取消订阅RecycleBox的事件
        if (_recycleBox != null)
        {
            _recycleBox.OnProjectileEntered -= StartRecycleProjectile;
        }
    }

    private void OnDestroy()
    {
        // 清理所有Tween以防内存泄漏
        foreach (var tween in _recyclingProjectiles.Values)
        {
            tween?.Kill();
        }
        _recyclingProjectiles.Clear();
    }

    // 开始回收投射物
    private void StartRecycleProjectile(GameObject projectile)
    {
        if (projectile == null || !projectile.CompareTag(StringPool.Projectile))
            return;

        // 如果投射物已经在回收中，忽略
        if (_recyclingProjectiles.ContainsKey(projectile))
            return;

        // 确定使用左侧还是右侧路径
        Transform[] waypoints = projectile.transform.position.x < 0 ? _leftWaypoints : _rightWaypoints;

        // 找到最近的路径点
        var closestIndex = waypoints
            .Select((waypoint, index) => new { Index = index, Distance = Vector2.Distance(projectile.transform.position, waypoint.position) })
            .OrderBy(x => x.Distance)
            .First().Index;

        // 创建从最近点到终点的路径
        Vector3[] path = new Vector3[waypoints.Length - closestIndex];
        for (int i = 0; i < path.Length; i++)
        {
            path[i] = waypoints[i + closestIndex].position;
        }

        // 禁用投射物的物理和碰撞
        DisableProjectilePhysics(projectile);

        // 使用DOTween移动投射物
        Tween moveTween = MoveProjectileAlongPath(projectile, path);

        // 存储Tween引用以便后续管理
        _recyclingProjectiles.Add(projectile, moveTween);

        // 触发事件
        OnProjectileRecycleStarted?.Invoke(projectile);

        // 增加计数
        Count++;
        OnProjectileRecycled?.Invoke(Count);
    }

    // 禁用投射物的物理和碰撞
    private void DisableProjectilePhysics(GameObject projectile)
    {
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        Collider2D c = projectile.GetComponent<Collider2D>();
        if (c != null)
        {
            c.enabled = false;
        }
    }

    // 使用DOTween移动投射物沿路径运动
    private Tween MoveProjectileAlongPath(GameObject projectile, Vector3[] path)
    {
        return projectile.transform.DOPath(path, _projectileMoveDuration, PathType.Linear, PathMode.TopDown2D)
            .SetEase(_projectileMoveEase)
            .OnComplete(() => OnProjectilePathCompleted(projectile));
    }

    // 投射物完成路径移动的回调
    private void OnProjectilePathCompleted(GameObject projectile)
    {
        if (projectile == null) return;

        // 从正在回收的列表中移除
        if (_recyclingProjectiles.ContainsKey(projectile))
        {
            _recyclingProjectiles.Remove(projectile);
        }

        // 触发完成事件
        OnProjectileRecycleCompleted?.Invoke(projectile);

        // 直接销毁投射物
        Destroy(projectile);
    }

    // 获取正在回收的投射物的数量
    public int GetRecyclingProjectilesCount()
    {
        return _recyclingProjectiles.Count;
    }

    // 重置计数
    public void ResetCount()
    {
        Count = 0;
    }

    // 清空所有回收中的投射物
    public void ClearAllProjectiles()
    {
        // 停止所有正在进行的Tween并销毁投射物
        foreach (var kvp in _recyclingProjectiles)
        {
            kvp.Value?.Kill();
            if (kvp.Key != null)
            {
                Destroy(kvp.Key);
            }
        }
        _recyclingProjectiles.Clear();
    }
}
