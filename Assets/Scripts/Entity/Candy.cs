using System;
using UnityEngine;

public class Candy : MonoBehaviour
{
    public event Action<Collision2D> OnCandyHit;

    private void OnCollisionEnter2D(Collision2D other)
    {
        OnCandyHit?.Invoke(other);
    }
}
