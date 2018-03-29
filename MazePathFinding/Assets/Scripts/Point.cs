
// структура данных, используется в программе для обозначения (запоминание) позиции ячейки
public struct Point
{
    public int i;
    public int j;

    public Point(int i, int j)
    {
        this.i = i;
        this.j = j;
    }

    public Point(float i, float j)
    {
        this.i = (int)i;
        this.j = (int)j;
    }

    public override bool Equals(object obj)
    {
        Point p = (Point)obj;
        if (p.i == i && p.j == j)
            return true;
        else
            return false; 
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return "(" + i.ToString() + "; " + j.ToString() + ")";
    }
}


