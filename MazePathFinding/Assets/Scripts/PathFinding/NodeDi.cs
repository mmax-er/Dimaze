using UnityEngine;

// класс вершины для алгоритма Дейкстры
public class NodeDi
{
    public int j;
    public int i;

    public float weight;
    public NodeDi parent;
    public bool isWalkable;
    public bool isOpen;

    public NodeDi(int i, int j, bool isWalkable)
    {
        isOpen = true;
        this.j = j;
        this.i = i;
        this.isWalkable = isWalkable;
        resetNode();
    }

    public void resetNode()
    {
        weight = int.MaxValue;
        parent = null;
    }

    // -------------------------------- Setters --------------------------------

    public void setParentNode(NodeDi node)
    {
        parent = node;
    }

    public void setWeight(float value)
    {
        weight = value;
    }

    public void setWalkable(bool value)
    {
        isWalkable = value;
    }

    public Vector2 getPosition()
    {
        return new Vector2(j, i);
    }

    public override bool Equals(object obj)
    {
        NodeDi node = obj as NodeDi;

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

