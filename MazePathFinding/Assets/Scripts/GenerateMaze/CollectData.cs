using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

public class CollectData
{
    // для анимации Крускаля, Эллера
    public List<Point> removeWall;

    // для анимации Growing tree
    public List<DelCell> removeCell;

    // время выполнения алгоритма
    public float time;

    private Node[,] maze;
    private List<Node> list;
    // длина (X)
    private int w;
    // висота (Y)
    private int h;

    // создает временную копию лабиринта, и запускает выполнение лабиринта
    public CollectData(int indexAlgorithm)
    {
        // список стін що видаляють, потрібний для відтворення анімації
        removeWall = new List<Point>();

        // розміот лабіринту
        w = TileSetManager.width;
        h = TileSetManager.height;

        // копія лабіринту
        maze = new Node[h, w];
        // в цей список записуються проходимі клітинки, щоб обєднювати регіони після видалення стіни
        list = new List<Node>();

        int k = 0;
        for (int i = 1; i < h - 1; i++)
        {
            for (int j = 1; j < w - 1; j++)
            {
                var p = new Point(i, j);
                maze[i, j] = new Node(p, k);
                maze[i, j].type = TileSetManager.tileMap[i, j].type;
                k++;

                if (maze[i, j].type == 1)
                {
                    list.Add(maze[i, j]);
                }
            }
        }

        // рахує час виконання частини коду
        Stopwatch sw = new Stopwatch();
        sw.Start();
        if (indexAlgorithm == 1)
            AlgorithmKruskala();
        if (indexAlgorithm == 2)
            AlgorithmEller();
        if (indexAlgorithm == 3)
            AlgorithmGrowingTree();
        sw.Stop();
        // обрахунок часу (в мілісекундах)
        time = (sw.ElapsedTicks / (float)Stopwatch.Frequency) * 1000;
    }


    #region Реалізація алгоритму Growing Tree
    private void AlgorithmGrowingTree()
    {
        removeCell = new List<DelCell>();
        for (int i = 1; i < h - 1; i++)
        {
            for (int j = 1; j < w - 1; j++)
            {
                maze[i, j].t = 0;
            }
        }

        List<Point> points = new List<Point>();
        while (true)
        {
            int i = UnityEngine.Random.Range(1, h - 1);
            int j = UnityEngine.Random.Range(1, w - 1);

            if (maze[i, j].type == 1)
            {
                points.Add(new Point(i, j));
                break;
            }
        }

        var rem = new List<Point>();
        while (points.Count > 0)
        {
            Point p = points.Last();
            removeCell.Add(new DelCell(p, 0));
            maze[p.i, p.j].t = 1;
            var pList = GetNeigh(p);

            if (pList.Count == 0)
            {
                removeCell.Add(new DelCell(p, 1));
                maze[p.i, p.j].t = 2;
                points.RemoveAll(x => x.Equals(p));
                rem.Add(p);
            }
            else if (pList.Count > 0)
            {
                int index = UnityEngine.Random.Range(0, pList.Count);
                points.Add(pList[index]);

                var ppp = GetWallPosition(p, pList[index]);
                removeCell.Add(new DelCell(ppp, 2));
            }
        }
    }

    // возвращает соседи указанного тайла (клетки)
    private List<Point> GetNeigh(Point p)
    {
        var points = new List<Point>();

        Point[] pArr = {
            new Point(p.i+2, p.j),
            new Point(p.i-2, p.j),
            new Point(p.i, p.j+2),
            new Point(p.i, p.j-2)};

        foreach (var pp in pArr)
        {
            if (pp.i > 0 && pp.j > 0 && pp.i < h - 1 && pp.j < w - 1)
            {
                if (maze[pp.i, pp.j].t < 1)
                    if (maze[pp.i, pp.j].type == 1)
                    {
                        points.Add(pp);
                    }
            }
        }

        return points;
    }

    // определение позиции стены между двумя пустыми ячейками, нужно для анимации, чтобы знать какие стены удалять
    private Point GetWallPosition(Point w1, Point w2)
    {
        int i = 0;
        int j = 0;

        // проверка для того чтобы определить стена по вертикали или горизонтали
        if (w1.j == w2.j)
        {
            // проверка по вертикали
            j = w1.j;
            if (w1.i < w2.i)
            {
                i = w1.i + 1;
            }
            else if (w1.i > w2.i)
            {
                i = w2.i + 1;
            }
        }
        else if (w1.i == w2.i)
        {
            // проверка по горизонтали
            i = w1.i;
            if (w1.j < w2.j)
            {
                j = w1.j + 1;
            }
            else if (w1.j > w2.j)
            {
                j = w2.j + 1;
            }
        }

        return new Point(i, j);
    }
    #endregion

    #region Реалізація алгоритму Ейлера
    private void AlgorithmEller()
    {
        for (int i = 1; i < h - 2; i += 2)
        {
            CreateRow(i);
            CreateVertical(i);
        }
        CreateLastRow();
    }

    private void CreateRow(int i)
    {
        for (int j = 1; j < w - 2; j += 2)
        {
            int t1 = maze[i, j].t;
            int t2 = maze[i, j + 2].t;

            if (t1 != t2)
            {
                if (UnityEngine.Random.Range(0, 2) > 0)
                {
                    RemoveWall(new Point(i, j + 1), t1, t2);
                }
            }
        }
    }

    // соединение ячеек в последней строке
    private void CreateLastRow()
    {
        int y = h - 2; // Row
        for (int j = 1; j < w - 2; j += 2)
        {
            int t1 = maze[y, j].t;
            int t2 = maze[y, j + 2].t;

            if (t1 != t2)
            {
                RemoveWall(new Point(y, j + 1), t1, t2);
            }
        }
    }

    private void CreateVertical(int i)
    {
        bool isRemove = false;
        bool isAdded = false;

        for (int j = 1; j < w - 2; j += 2)
        {
            int t1 = maze[i, j].t;
            int t2 = maze[i, j + 2].t;
            var pTopWall = new Point(i + 1, j);

            if (t1 != t2)
            {
                if (!isAdded)
                {
                    RemoveWall(pTopWall, t1, maze[i + 2, j].t);
                }
                isAdded = false;
            }
            else
            {
                isRemove = UnityEngine.Random.Range(0, 2) > 0;
                if (isRemove)
                {
                    RemoveWall(pTopWall, t1, maze[i + 2, j].t);
                    isAdded = true;
                }
            }
        }
        CheckLastVertical(i, isAdded);
    }

    // соединение ячеек в последнем столбце
    private void CheckLastVertical(int i, bool isAdded)
    {
        int t1 = maze[i, w - 2].t;
        int t2 = maze[i, w - 4].t;
        var pTopWall = new Point(i + 1, w - 2);

        if (t1 != t2)
        {
            RemoveWall(pTopWall, t1, maze[i + 2, w - 2].t);
        }
        else
        {
            if (isAdded ? UnityEngine.Random.Range(0, 2) > 0 : true)
            {
                RemoveWall(pTopWall, t1, maze[i + 2, w - 2].t);
            }
        }
    }
    #endregion

    #region Реалізація алгоритму Крускала
    private void AlgorithmKruskala()
    {
        List<Point> walls = new List<Point>();
        for (int i = 1; i < h - 1; i++)
        {
            for (int j = 1; j < w - 1; j++)
            {
                if (maze[i, j].type == 0)
                {
                    walls.Add(maze[i, j].p);
                }
            }
        }

        // перемешивания стен в списке случайным образом
        walls = walls.OrderBy(a => Guid.NewGuid()).ToList();

        for (int i = 0; i < walls.Count; i++)
        {
            var tl = GetNeighborhoods(walls[i]);

            if (tl.Count == 2)
            {
                var t1 = maze[tl[0].i, tl[0].j].t;
                var t2 = maze[tl[1].i, tl[1].j].t;

                if (t1 != t2)
                {
                    RemoveWall(walls[i], t1, t2);
                }
            }
        }
    }

    // возвращает соседи указанного тайла
    private List<Point> GetNeighborhoods(Point point)
    {
        // соседи по горизонтали
        var p11 = new Point(point.i, point.j - 1);
        var p12 = new Point(point.i, point.j + 1);

        // соседи по вертикали
        var p21 = new Point(point.i - 1, point.j);
        var p22 = new Point(point.i + 1, point.j);

        // если подходят соседи по горизонтали, берем их
        if (p12.j < w - 1 && p11.j > 0)
        {
            if (maze[p12.i, p12.j].type == 1 && maze[p11.i, p11.j].type == 1)
            {
                // или разные номера регионы этих соседей
                if (maze[p12.i, p12.j].t != maze[p11.i, p11.j].t)
                {
                    return new List<Point>() { p11, p12 };
                }
            }
        }
        // если соседи по горизонтали не подходят то проверяем соседей по вертикали, если подходят берем их
        if (p22.i < h - 1 && p21.i > 0)
        {
            if (maze[p22.i, p22.j].type == 1 && maze[p21.i, p21.j].type == 1)
            {
                // или разные номера регионов этих соседей
                if (maze[p22.i, p22.j].t != maze[p21.i, p21.j].t)
                {
                    return new List<Point>() { p21, p22 };
                }
            }
        }

        // ни один сосед не подошел тогда возвращаем пустой список
        return new List<Point>();
    }
    #endregion

    // функция удаляет указанную стену, и обеднюе указанные регионы регионы
    private void RemoveWall(Point p, int t1, int t2)
    {
        removeWall.Add(p);
        maze[p.i, p.j].type = 1;
        maze[p.i, p.j].t = t2;
        list.Add(maze[p.i, p.j]);

        for (int j = 0; j < list.Count; j++)
        {
            if (list[j].t == t1)
                list[j].t = t2;
        }
    }
}