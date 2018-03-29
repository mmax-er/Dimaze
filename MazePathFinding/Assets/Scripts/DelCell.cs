
// нужен для анимации алгоритма Growing Tree
public class DelCell
{
    // позиция ячейки которую нужно анимировать
    public Point p;

    // тип анимации
    // 0 - сделать клетку красной
    // 1 - сделать клетку и ее соседей белыми
    // 2 - удалить стену и сделать клетку красной
    public int type;

    public DelCell(Point p, int type)
    {
        this.p = p;
        this.type = type;
    }

    public override bool Equals(object obj)
    {
        DelCell obj1 = (DelCell)obj;

        if (obj1.p.Equals(p) && obj1.type == type)
            return true;
        else
            return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
