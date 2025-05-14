using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class GameGridSystem : MonoBehaviour, IGameSystem
{
    [SerializeField] private Rect cellAreaRect = new(0, 0, 0.5f, 0.5f);
    [SerializeField] private Vector2 cellSpacing = Vector2.zero;
    [SerializeField] private int row = 10;
    [SerializeField] private int col = 7;
    public Matrix<Grid> Grids { get; private set; }

    public Rect Cell => cellAreaRect;

    private void Awake()
    {
        InitializeGrid();
#if GAME_DEBUG
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<GameAreaVisualizer>().enabled = true;
#endif
    }

    private void InitializeGrid()
    {
        //计算当前设定区域可以分成多少个cell (考虑间距设置)
        var cellWidth = cellAreaRect.width + cellSpacing.x;
        var cellHeight = cellAreaRect.height + cellSpacing.y;

        //创建网格
        Grids = new Matrix<Grid>(row, col);

        //遍历当前设定区域，创建网格
        for (int x = 0; x < col; x++)
        {
            for (int y = 0; y < row; y++)
            {
                //创建 gameObject
                var gridObject = new GameObject("Node " + x + ", " + y);
                var gridTransform = gridObject.transform;

                gridTransform.SetParent(transform);
                gridTransform.localPosition = new Vector3(
                    ((x * cellWidth) + (cellWidth / 2)) - ((col * cellWidth) / 2),
                    ((y * cellHeight) + (cellHeight / 2)) - ((row * cellHeight) / 2),
                    0);
                //创建网格
                var grid = gridObject.AddComponent<Grid>();
                grid.Set(x, y);


                //添加到矩阵
                Grids.Set(y, x, grid);
            }
        }
    }
}
