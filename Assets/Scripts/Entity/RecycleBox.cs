using UnityEngine;
using System;

public class RecycleBox : MonoBehaviour
{
    public event Action<GameObject> OnProjectileEntered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var projectile = collision.gameObject;
        if (projectile.CompareTag(StringPool.Projectile))
        {
            // 触发球进入事件
            OnProjectileEntered?.Invoke(projectile);
        }
    }
}
