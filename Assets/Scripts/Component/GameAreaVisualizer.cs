using UnityEditor;
using UnityEngine;

public class GameAreaVisualizer : MonoBehaviour
{
    private GameGridSystem _gameGridSystem;

    private void Awake()
    {
        _gameGridSystem = GetComponent<GameGridSystem>();
        if (_gameGridSystem == null)
        {
            Debug.LogError("No MeshArea component found on this GameObject!");
            enabled = false;
        }
    }


    // ReSharper disable Unity.PerformanceAnalysis
    private void Update()
    {
        DrawNode(_gameGridSystem.Grids);
    }

    // 绘制节点的矩形框和从中心到四角的连线
    private void DrawNode(Matrix<Grid> grids)
    {
        if (grids == null) return;

        for (int x = 0; x < grids.Cols; x++)
        {
            for (int y = 0; y < grids.Rows; y++)
            {
                Grid grid = grids[x, y];
                if (grid == null) continue;

                // 获取网格位置
                Vector3 position = grid.transform.position;

                // 计算网格的边界
                float halfWidth = _gameGridSystem.Cell.width / 2;
                float halfHeight = _gameGridSystem.Cell.height / 2;

                // 定义矩形的四个顶点
                Vector3 bottomLeft = new Vector3(position.x - halfWidth, position.y - halfHeight, 0);
                Vector3 topLeft = new Vector3(position.x - halfWidth, position.y + halfHeight, 0);
                Vector3 topRight = new Vector3(position.x + halfWidth, position.y + halfHeight, 0);
                Vector3 bottomRight = new Vector3(position.x + halfWidth, position.y - halfHeight, 0);

                // 根据网格坐标设置颜色
                Color color = Color.HSVToRGB(grid.X * 0.1f + grid.Y * 0.05f, 1, 1);

                // 绘制矩形边框
                Debug.DrawLine(bottomLeft, topLeft, color);
                Debug.DrawLine(topLeft, topRight, color);
                Debug.DrawLine(topRight, bottomRight, color);
                Debug.DrawLine(bottomRight, bottomLeft, color);

                // 绘制从中心到四角的连线
                Debug.DrawLine(position, bottomLeft, color);
                Debug.DrawLine(position, topLeft, color);
                Debug.DrawLine(position, topRight, color);
                Debug.DrawLine(position, bottomRight, color);
            }
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    void OnGUI()
    {
        DrawGridUIText(_gameGridSystem.Grids);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    void DrawGridUIText(Matrix<Grid> grids)
    {
        if (grids == null) return;

        for (int x = 0; x < grids.Cols; x++)
        {
            for (int y = 0; y < grids.Rows; y++)
            {
                Grid grid = grids[x, y];
                if (grid == null) continue;

                // 获取网格位置
                Vector3 position = grid.transform.position;

                // 在网格中心绘制文本，显示坐标信息
                UnityEngine.GUI.Label(
                    new Rect(
                        Camera.main.WorldToScreenPoint(position).x - 50,
                        Screen.height - Camera.main.WorldToScreenPoint(position).y - 10,
                        100,
                        20
                    ),
                    $"X: {grid.X} Y: {grid.Y}",
                    new GUIStyle
                    {
                        fontSize = 10,
                        normal = { textColor = Color.HSVToRGB(grid.X * 0.1f + grid.Y * 0.05f, 1, 1) },
                        alignment = TextAnchor.MiddleCenter
                    }
                );
            }
        }
    }
}
