using System;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public int X { get; set; }
    public int Y { get; set; }

    private GameObject _entity;

    public GameObject Entity
    {
        get => _entity;
        set
        {
            _entity = value;
            if (_entity)
            {
                value.transform.SetParent(transform);
            }
        }
    }

    public void Set(int x, int y)
    {
        X = x;
        Y = y;
    }

    private void Update()
    {
        if (Entity)
        {
            Debug.DrawLine(transform.position, Entity.transform.position,
                Color.HSVToRGB(X * 100 + Y, 1, 1));
        }
    }
}
