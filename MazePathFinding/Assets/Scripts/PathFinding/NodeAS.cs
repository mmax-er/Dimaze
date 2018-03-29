using UnityEngine;

// класс вершины для алгоритма А *
public class NodeAS
{
    public bool isOpen;

    public int j;
    public int i;

    public float hScore;
    public float gScore;

    public float fScore;

    public NodeAS parent = null;
    public bool isWalkable = true;

    public NodeAS(int i, int j, bool isWalkable)
    {
        this.j = j;
        this.i = i;
        this.isWalkable = isWalkable;
    }

    public NodeAS GetCopy()
    {
        NodeAS node = new NodeAS(j, i, isWalkable);
        node.gScore = gScore;
        node.hScore = hScore;
        node.parent = parent;

        return node;
    }

    public Vector2 GetVector2()
    {
        return new Vector2(j, i);
    }

    public override string ToString()
    {
        return "(" + i.ToString() + "; " + j.ToString() + ")";
    }

    public override bool Equals(object obj)
    {
        NodeAS node = obj as NodeAS;

        if (node.i == i && node.j == j)
            return true;
        else
            return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}