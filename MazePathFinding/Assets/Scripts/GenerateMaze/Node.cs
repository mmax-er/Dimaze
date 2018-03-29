public class Node
{
    public Point p;
    public int t;
    public int type = 0;

    public Node(Point p)
    {
        this.p = p;
    }

    public Node(Point p, int t)
    {
        this.t = t;
        this.p = p;
    }
}
