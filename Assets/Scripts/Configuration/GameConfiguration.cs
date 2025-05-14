using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.U2D;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "GameConfiguration", menuName = "CandyPop/GameConfiguration")]
public class GameConfiguration : ScriptableObject
{

    [Header("关卡设置")]
    // 最大关卡数（超过该关卡后，数值将不再变化）
    public int maxLevel = 100;

    [Header("物品数量设置")] public int baseItemCount = 5;

    public int maxItemCount = 20; // 上限

    // 关卡增长曲线：输入为归一化关卡数（0～1）
    public AnimationCurve itemCountCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("物品 HP 设置")] public int baseItemHP = 1;
    public int maxItemHP = 300;
    public AnimationCurve itemHPCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("投射物设置")] public int baseProjectileCount = 10;

    public int minProjectileCount = 5;

    // 关卡越高，投射物数量逐渐变化
    public AnimationCurve projectileCountCurve = AnimationCurve.Linear(0, 1, 1, 0);

    /// <summary>
    /// 根据原始关卡数据计算归一化的关卡值（范围0～1）。
    /// 假设关卡是从1开始的，超过maxLevel后归一化值为1。
    /// </summary>
    private float GetNormalizedLevel(int level)
    {
        int clampedLevel = Mathf.Clamp(level, 1, maxLevel);
        return (clampedLevel - 1f) / (maxLevel - 1f);
    }

    public void CreateObstacle()
    {
    }
}

[Serializable]
public class ItemTypeConfiguration
{
    [SerializeField] private Obstacle obstacle;

}
