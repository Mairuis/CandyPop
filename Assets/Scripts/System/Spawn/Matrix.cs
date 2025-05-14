using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 通用二维矩阵管理类，适用于游戏中的障碍物地图、状态表示等
/// </summary>
/// <typeparam name="T">矩阵元素的类型</typeparam>
public class Matrix<T> : IEnumerable<T>
{
    private T[,] _data;

    /// <summary>
    /// 矩阵的行数
    /// </summary>
    public int Rows { get; private set; }

    /// <summary>
    /// 矩阵的列数
    /// </summary>
    public int Cols { get; private set; }

    /// <summary>
    /// 获取指定行的所有元素作为数组
    /// </summary>
    /// <param name="row">行索引</param>
    /// <returns>包含该行所有元素的数组</returns>
    public T[] this[int row]
    {
        get
        {
            if (row < 0 || row >= Rows)
                throw new IndexOutOfRangeException($"行索引 {row} 超出范围 [0, {Rows - 1}]");

            var rowArray = new T[Cols];
            for (int c = 0; c < Cols; c++)
            {
                rowArray[c] = _data[row, c];
            }

            return rowArray;
        }
    }

    /// <summary>
    /// 通过索引器访问矩阵元素
    /// </summary>
    /// <param name="row">行索引</param>
    /// <param name="col">列索引</param>
    /// <returns>指定位置的元素</returns>
    public T this[int col, int row]
    {
        get => Get(row, col);
        set => Set(row, col, value);
    }

    /// <summary>
    /// 初始化指定行列数的空矩阵
    /// </summary>
    /// <param name="rows">行数</param>
    /// <param name="cols">列数</param>
    public Matrix(int rows, int cols)
    {
        if (rows <= 0 || cols <= 0)
            throw new ArgumentException("矩阵维度必须为正数");

        Rows = rows;
        Cols = cols;
        _data = new T[rows, cols];
    }

    /// <summary>
    /// 初始化矩阵并填充默认值
    /// </summary>
    /// <param name="rows">行数</param>
    /// <param name="cols">列数</param>
    /// <param name="defaultValue">默认值</param>
    public Matrix(int rows, int cols, T defaultValue) : this(rows, cols)
    {
        Clear(defaultValue);
    }

    /// <summary>
    /// 设置指定位置的值
    /// </summary>
    /// <param name="row">行索引</param>
    /// <param name="col">列索引</param>
    /// <param name="value">要设置的值</param>
    public void Set(int row, int col, T value)
    {
        if (!IsValidPosition(row, col))
            throw new IndexOutOfRangeException($"坐标 ({row}, {col}) 超出矩阵范围");

        _data[row, col] = value;
    }

    /// <summary>
    /// 获取指定位置的值
    /// </summary>
    /// <param name="row">行索引</param>
    /// <param name="col">列索引</param>
    /// <returns>指定位置的值</returns>
    public T Get(int row, int col)
    {
        if (!IsValidPosition(row, col))
            throw new IndexOutOfRangeException($"坐标 ({row}, {col}) 超出矩阵范围");

        return _data[row, col];
    }

    /// <summary>
    /// 将矩阵所有单元格重置为指定值
    /// </summary>
    /// <param name="value">重置值，默认为T的默认值</param>
    public void Clear(T value = default)
    {
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                _data[r, c] = value;
            }
        }
    }

    /// <summary>
    /// 检查指定坐标是否在矩阵范围内
    /// </summary>
    /// <param name="row">行索引</param>
    /// <param name="col">列索引</param>
    /// <returns>true表示有效，false表示越界</returns>
    public bool IsValidPosition(int row, int col)
    {
        return row >= 0 && row < Rows && col >= 0 && col < Cols;
    }

    /// <summary>
    /// 返回用于foreach遍历的枚举器
    /// </summary>
    public IEnumerator<T> GetEnumerator()
    {
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                yield return _data[r, c];
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// 返回指定行的所有元素
    /// </summary>
    /// <param name="row">行索引</param>
    /// <returns>该行元素的迭代器</returns>
    public IEnumerable<T> GetRow(int row)
    {
        if (row < 0 || row >= Rows)
            throw new ArgumentOutOfRangeException(nameof(row));

        for (int c = 0; c < Cols; c++)
        {
            yield return _data[row, c];
        }
    }

    /// <summary>
    /// 返回指定列的所有元素
    /// </summary>
    /// <param name="col">列索引</param>
    /// <returns>该列元素的迭代器</returns>
    public IEnumerable<T> GetColumn(int col)
    {
        if (col < 0 || col >= Cols)
            throw new ArgumentOutOfRangeException(nameof(col));

        for (int r = 0; r < Rows; r++)
        {
            yield return _data[r, col];
        }
    }

    /// <summary>
    /// 返回所有元素及其坐标
    /// </summary>
    /// <returns>包含行、列和元素值的元组迭代器</returns>
    public IEnumerable<(int row, int col, T value)> GetAllWithPositions()
    {
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                yield return (r, c, _data[r, c]);
            }
        }
    }

    /// <summary>
    /// 返回指定矩形区域内的元素
    /// </summary>
    /// <param name="startRow">区域起点行</param>
    /// <param name="startCol">区域起点列</param>
    /// <param name="height">区域高度</param>
    /// <param name="width">区域宽度</param>
    /// <returns>区域内元素的迭代器</returns>
    public IEnumerable<T> GetRegion(int startRow, int startCol, int height, int width)
    {
        if (startRow < 0 || startCol < 0 || height <= 0 || width <= 0)
            throw new ArgumentException("区域参数无效");

        int endRow = Math.Min(startRow + height, Rows);
        int endCol = Math.Min(startCol + width, Cols);

        for (int r = startRow; r < endRow; r++)
        {
            for (int c = startCol; c < endCol; c++)
            {
                yield return _data[r, c];
            }
        }
    }

    /// <summary>
    /// 填充指定矩形区域为给定值
    /// </summary>
    /// <param name="startRow">区域起点行</param>
    /// <param name="startCol">区域起点列</param>
    /// <param name="height">区域高度</param>
    /// <param name="width">区域宽度</param>
    /// <param name="value">填充值</param>
    public void FillRegion(int startRow, int startCol, int height, int width, T value)
    {
        if (startRow < 0 || startCol < 0 || height <= 0 || width <= 0)
            throw new ArgumentException("区域参数无效");

        int endRow = Math.Min(startRow + height, Rows);
        int endCol = Math.Min(startCol + width, Cols);

        for (int r = startRow; r < endRow; r++)
        {
            for (int c = startCol; c < endCol; c++)
            {
                _data[r, c] = value;
            }
        }
    }

    /// <summary>
    /// 将另一个矩阵的内容复制到指定位置
    /// </summary>
    /// <param name="source">源矩阵</param>
    /// <param name="destRow">目标起始行</param>
    /// <param name="destCol">目标起始列</param>
    public void CopyFrom(Matrix<T> source, int destRow, int destCol)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        for (int r = 0; r < source.Rows; r++)
        {
            for (int c = 0; c < source.Cols; c++)
            {
                int targetRow = destRow + r;
                int targetCol = destCol + c;

                if (IsValidPosition(targetRow, targetCol))
                {
                    _data[targetRow, targetCol] = source[c, r];
                }
            }
        }
    }

    /// <summary>
    /// 检查矩阵中是否包含指定值
    /// </summary>
    /// <param name="value">要查找的值</param>
    /// <returns>true表示存在</returns>
    public bool Contains(T value)
    {
        EqualityComparer<T> comparer = EqualityComparer<T>.Default;
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                if (comparer.Equals(_data[r, c], value))
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 查找所有匹配指定值的位置
    /// </summary>
    /// <param name="value">要查找的值</param>
    /// <returns>包含所有匹配坐标的列表</returns>
    public List<(int row, int col)> FindAll(T value)
    {
        var result = new List<(int row, int col)>();
        EqualityComparer<T> comparer = EqualityComparer<T>.Default;
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                if (comparer.Equals(_data[r, c], value))
                    result.Add((r, c));
            }
        }

        return result;
    }

    /// <summary>
    /// 调整矩阵大小，保留现有数据，填充新区域
    /// </summary>
    /// <param name="newRows">新的行数</param>
    /// <param name="newCols">新的列数</param>
    /// <param name="defaultValue">新区域的默认值</param>
    public void Resize(int newRows, int newCols, T defaultValue = default)
    {
        if (newRows <= 0 || newCols <= 0)
            throw new ArgumentException("矩阵维度必须为正数");

        var newData = new T[newRows, newCols];

        // 填充默认值
        for (int r = 0; r < newRows; r++)
        {
            for (int c = 0; c < newCols; c++)
            {
                newData[r, c] = defaultValue;
            }
        }

        // 复制现有数据
        int minRows = Math.Min(Rows, newRows);
        int minCols = Math.Min(Cols, newCols);

        for (int r = 0; r < minRows; r++)
        {
            for (int c = 0; c < minCols; c++)
            {
                newData[r, c] = _data[r, c];
            }
        }

        _data = newData;
        Rows = newRows;
        Cols = newCols;
    }

    /// <summary>
    /// 检查给定形状是否可以放置在指定位置
    /// </summary>
    /// <param name="shape">形状的布尔矩阵</param>
    /// <param name="row">目标行位置</param>
    /// <param name="col">目标列位置</param>
    /// <returns>true表示可以放置</returns>
    public bool CanPlaceShape(bool[,] shape, int row, int col)
    {
        if (shape == null)
            throw new ArgumentNullException(nameof(shape));

        int shapeRows = shape.GetLength(0);
        int shapeCols = shape.GetLength(1);

        // 检查是否超出矩阵边界
        if (row < 0 || col < 0 || row + shapeRows > Rows || col + shapeCols > Cols)
            return false;

        // 检查是否与现有元素重叠
        EqualityComparer<T> comparer = EqualityComparer<T>.Default;
        for (int r = 0; r < shapeRows; r++)
        {
            for (int c = 0; c < shapeCols; c++)
            {
                if (shape[r, c] && !comparer.Equals(_data[row + r, col + c], default))
                    return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 将形状放置到矩阵中
    /// </summary>
    /// <param name="shape">形状的布尔矩阵</param>
    /// <param name="row">目标行位置</param>
    /// <param name="col">目标列位置</param>
    /// <param name="value">要填充的值</param>
    public void PlaceShape(bool[,] shape, int row, int col, T value)
    {
        if (shape == null)
            throw new ArgumentNullException(nameof(shape));

        int shapeRows = shape.GetLength(0);
        int shapeCols = shape.GetLength(1);

        if (row < 0 || col < 0 || row + shapeRows > Rows || col + shapeCols > Cols)
            throw new ArgumentException("形状在当前位置超出矩阵边界");

        for (int r = 0; r < shapeRows; r++)
        {
            for (int c = 0; c < shapeCols; c++)
            {
                if (shape[r, c])
                {
                    _data[row + r, col + c] = value;
                }
            }
        }
    }

    /// <summary>
    /// 将矩阵内容整体向下移动指定行数
    /// </summary>
    /// <param name="rows">移动的行数</param>
    public void MoveDown(int rows = 1)
    {
        if (rows <= 0)
            return;

        for (int r = Rows - 1; r >= rows; r--)
        {
            for (int c = 0; c < Cols; c++)
            {
                _data[r, c] = _data[r - rows, c];
            }
        }

        // 填充顶部空出的行
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                _data[r, c] = default;
            }
        }
    }

    /// <summary>
    /// 计算矩阵中指定值的总数
    /// </summary>
    /// <param name="value">要计数的值</param>
    /// <returns>值的出现次数</returns>
    public int Count(T value)
    {
        int count = 0;
        EqualityComparer<T> comparer = EqualityComparer<T>.Default;

        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                if (comparer.Equals(_data[r, c], value))
                    count++;
            }
        }

        return count;
    }

    /// <summary>
    /// 检查指定行是否全为默认值
    /// </summary>
    /// <param name="row">要检查的行</param>
    /// <returns>true表示全为默认值</returns>
    public bool IsRowEmpty(int row)
    {
        if (row < 0 || row >= Rows)
            throw new ArgumentOutOfRangeException(nameof(row));

        EqualityComparer<T> comparer = EqualityComparer<T>.Default;
        for (int c = 0; c < Cols; c++)
        {
            if (!comparer.Equals(_data[row, c], default))
                return false;
        }

        return true;
    }

    /// <summary>
    /// 批量更新多个位置的值
    /// </summary>
    /// <param name="updates">包含位置和新值的元组列表</param>
    public void BatchUpdate(List<(int row, int col, T value)> updates)
    {
        if (updates == null)
            throw new ArgumentNullException(nameof(updates));

        foreach (var (row, col, value) in updates)
        {
            if (IsValidPosition(row, col))
            {
                _data[row, col] = value;
            }
        }
    }

    /// <summary>
    /// 创建当前矩阵的深拷贝
    /// </summary>
    /// <returns>新的矩阵实例</returns>
    public Matrix<T> Clone()
    {
        Matrix<T> clone = new Matrix<T>(Rows, Cols);
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Cols; c++)
            {
                clone[c, r] = _data[c, r];
            }
        }

        return clone;
    }
}
