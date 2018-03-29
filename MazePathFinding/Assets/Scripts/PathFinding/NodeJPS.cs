
// класс вершины для алгоритма Jump Point Search
public class NodeJPS
{
    public int j;
    public int i;

    public float h;
    public float g;
    public float f;


    public float weight;
    public NodeJPS parent;
    public bool isWalkable;
    public bool isOpened;
    public bool isClosed;

    public NodeJPS(int i, int j, bool isWalkable)
    {
        this.j = j;
        this.i = i;
        this.isWalkable = isWalkable;
        parent = null;
    }

    public override bool Equals(object obj)
    {
        NodeJPS node = obj as NodeJPS;

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
