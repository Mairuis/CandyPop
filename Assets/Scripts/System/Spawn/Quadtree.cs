using System;
using System.Collections.Generic;
using UnityEngine;

public class QuadTreeNode
{
    public int X { get; }
    public int Y { get; }
    public int Level { get; }
    public Rect Rect { get; }
    public QuadTreeNode[] Children { get; }
    public bool IsOccupied { get; set; }
    public bool IsLeaf => Children == null || Children.Length == 0 || Children[0] == null;

    private Rect _leafRect;

    public QuadTreeNode(Rect leafRect, Rect rect, int level, int x, int y)
    {
        _leafRect = leafRect;
        Rect = rect;
        Level = level;
        X = x;
        Y = y;
        Children = new QuadTreeNode[4];
    }

    public void Subdivided()
    {
        if (!IsSubdividable())
        {
            return;
        }

        if (!IsEmpty())
        {
            throw new Exception("Already subdivided");
        }

        float subWidth = Rect.width / 2f;
        float subHeight = Rect.height / 2f;


        float centerX = Rect.x + Rect.width / 2f;
        float centerY = Rect.y + Rect.height / 2f;

        // 左上子节点：以父节点中心为基准，左上区域
        Children[0] = new QuadTreeNode(
            _leafRect,
            new Rect(centerX - subWidth, centerY, subWidth, subHeight),
            Level + 1, 2 * X, 2 * Y + 1);

        // 右上子节点：以父节点中心为基准，右上区域
        Children[1] = new QuadTreeNode(
            _leafRect,
            new Rect(centerX, centerY, subWidth, subHeight),
            Level + 1, 2 * X + 1, 2 * Y + 1);

        // 左下子节点：以父节点中心为基准，左下区域
        Children[2] = new QuadTreeNode(
            _leafRect,
            new Rect(centerX - subWidth, centerY - subHeight, subWidth, subHeight),
            Level + 1, 2 * X, 2 * Y);

        // 右下子节点：以父节点中心为基准，右下区域
        Children[3] = new QuadTreeNode(
            _leafRect,
            new Rect(centerX, centerY - subHeight, subWidth, subHeight),
            Level + 1, 2 * X + 1, 2 * Y);

        foreach (var child in Children)
        {
            child.Subdivided();
        }
    }

    public List<QuadTreeNode> GetAllNode()
    {
        var nodes = new List<QuadTreeNode>();
        var queue = new Queue<QuadTreeNode>();
        queue.Enqueue(this);
        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            if (node.IsLeaf)
            {
                nodes.Add(node);
                continue;
            }

            foreach (var child in node.Children)
            {
                queue.Enqueue(child);
            }
        }
        return nodes;
    }

    public bool IsSubdividable()
    {
        return Rect.width >= 2 * _leafRect.width && Rect.height >= 2 * _leafRect.height;
    }

    public bool IsEmpty()
    {
        return Children[0] == null;
    }
}
