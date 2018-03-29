using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// основной класс, отвечает за:
// 1) создание 3d лабиринта
// 2) запуск генерации или поиска пути
// 3) воспроизведение анимации
public class TileSetManager : MonoBehaviour
{

    public static int widthMaze = 5;  // j
    public static int heightMaze = 5; // i
    public static int width = widthMaze * 2 + 1;   // j
    public static int height = heightMaze * 2 + 1; // i
    public static Tile[,] tileMap;
    public static System.Random rand = new System.Random();
    public static float SpeedAnimation = 1;

    //public Coroutine coroutine;
    public GameObject TilePrefab;
    public UIManager uiManager;
    public GameObject StartPath;
    public GameObject EndPath;

    private void Awake()
    {
        CreateTileGrid();
    }

    void Start()
    {
        for (int i = 1; i < height; i++)
        {
            if (i % 2 != 0 && i < height - 1)
            {
                for (int j = 1; j < width; j++)
                {
                    if (j % 2 != 0 && j < width - 1)
                    {
                        tileMap[i, j].SetType(1);
                    }
                }
            }
        }
    }

    public void StarGenerateMaze(int i)
    {
        ResetGrid();
        CollectData cdata = new CollectData(i);

        uiManager.SetGenerateMazeTime(cdata.time);
        //StopAnimation();
        if (i == 1 || i == 2)
        {
            AnimImmd(cdata.removeWall);
            //coroutine = StartCoroutine("Anim", cdata.removeWall);
        }
        else
        {
            Anim3Immd(cdata.removeCell);
            //coroutine = StartCoroutine("Anim3", cdata.removeCell);
        }
    }

    public void StartFindingPath(int i)
    {
        ResetGridForPathFinding();

        var spos = StartPath.transform.position;
        var epos = EndPath.transform.position;

        CollectPath cpath = new CollectPath(new Point(spos.z, spos.x), new Point(epos.z, epos.x), i);

        uiManager.SetPathFindingTime(cpath.time);
        uiManager.SetLengthPath(cpath.pathList.Count, cpath.workList.Count);
        //StopAnimation();
        //coroutine = StartCoroutine("AnimPath", cpath);
        AnimPathImmd(cpath);
    }

    public void ResetGrid()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                tileMap[i, j].SetType(0);
            }
        }

        for (int i = 1; i < height; i++)
        {
            if (i % 2 != 0 && i < height - 1)
            {
                for (int j = 1; j < width; j++)
                {
                    if (j % 2 != 0 && j < width - 1)
                    {
                        tileMap[i, j].SetType(1);
                        tileMap[i, j].SetColor(Color.green);
                    }
                }
            }
        }
    }

    public void ResetGridForPathFinding()
    {
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (tileMap[i, j].type == 1)
                {
                    tileMap[i, j].SetColor(Color.white);
                }
            }
        }
    }

    //public void StopAnimation()
    //{
    //    if (coroutine != null)
    //        StopCoroutine(coroutine);
    //    coroutine = null;
    //}

    public void CreateTileGrid()
    {
        tileMap = new Tile[height, width];

        var countChild = transform.childCount;
        for (int i = 0; i < countChild; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                var tile = Instantiate(TilePrefab);
                tile.transform.parent = transform;
                tile.GetComponent<Tile>().SetPoint(new Point(i, j));
            }
        }
    }


    // анимация генерации алгоритма Крускаля и Эллера
    void AnimImmd(List<Point> walls)
    {
        foreach (var wall in walls)
        {
            if (tileMap[wall.i, wall.j - 1].type == 1 &&
                tileMap[wall.i, wall.j + 1].type == 1)
            {
                tileMap[wall.i, wall.j - 1].SetType(1);
                tileMap[wall.i, wall.j + 1].SetType(1);
            }

            if (tileMap[wall.i - 1, wall.j].type == 1 &&
                tileMap[wall.i + 1, wall.j].type == 1)
            {
                tileMap[wall.i - 1, wall.j].SetType(1);
                tileMap[wall.i + 1, wall.j].SetType(1);
            }

            tileMap[wall.i, wall.j].SetType(1);

        }
    }

    //IEnumerator Anim(List<Point> walls)
    //{
    //    //yield return new WaitForSeconds(2);
    //    var waitsec = new WaitForSeconds(SpeedAnimation);
    //    foreach (var wall in walls)
    //    {
    //        if (tileMap[wall.i, wall.j - 1].type == 1 &&
    //            tileMap[wall.i, wall.j + 1].type == 1)
    //        {
    //            tileMap[wall.i, wall.j - 1].SetType(1);
    //            tileMap[wall.i, wall.j + 1].SetType(1);
    //        }

    //        if (tileMap[wall.i - 1, wall.j].type == 1 &&
    //            tileMap[wall.i + 1, wall.j].type == 1)
    //        {
    //            tileMap[wall.i - 1, wall.j].SetType(1);
    //            tileMap[wall.i + 1, wall.j].SetType(1);
    //        }

    //        tileMap[wall.i, wall.j].SetType(1);
    //        if (SpeedAnimation != 0)
    //            yield return waitsec;

    //    }
    //    coroutine = null;
    //    yield return waitsec;
    //}

    void Anim3Immd(List<DelCell> walls)
    {
        for (int i = 0; i < walls.Count; i++)
        {
            var w1 = walls[i].p;

            if (walls[i].type == 0)
            {
                tileMap[w1.i, w1.j].SetColor(Color.red);
            }
            else if (walls[i].type == 1)
            {
                tileMap[w1.i, w1.j].SetColor(Color.white);

                Point[] arr = new Point[] {
                    new Point(w1.i, w1.j+1),
                    new Point(w1.i, w1.j-1),
                    new Point(w1.i+1, w1.j),
                    new Point(w1.i-1, w1.j)};

                foreach (var a in arr)
                {
                    if (a.i > 0 && a.j > 0 && a.i < height - 1 && a.j < width - 1)
                    {
                        if (tileMap[a.i, a.j].type == 1)
                        {
                            tileMap[a.i, a.j].SetColor(Color.white);
                        }
                    }
                }
            }
            else if (walls[i].type == 2)
            {
                tileMap[w1.i, w1.j].SetType(1);
                tileMap[w1.i, w1.j].SetColor(Color.red);
            }
        }
    }

    // анимация генерации алгоритма Growing tree
    //IEnumerator Anim3(List<DelCell> walls)
    //{
    //    //yield return new WaitForSeconds(2);
    //    var waitsec = new WaitForSeconds(SpeedAnimation);
    //    for (int i = 0; i < walls.Count; i++)
    //    {
    //        var w1 = walls[i].p;

    //        if (walls[i].type == 0)
    //        {
    //            tileMap[w1.i, w1.j].SetColor(Color.red);
    //            if (SpeedAnimation != 0)
    //                yield return waitsec;
    //        }
    //        else if (walls[i].type == 1)
    //        {
    //            tileMap[w1.i, w1.j].SetColor(Color.white);

    //            Point[] arr = new Point[] {
    //                new Point(w1.i, w1.j+1),
    //                new Point(w1.i, w1.j-1),
    //                new Point(w1.i+1, w1.j),
    //                new Point(w1.i-1, w1.j)};

    //            foreach (var a in arr)
    //            {
    //                if (a.i > 0 && a.j > 0 && a.i < height - 1 && a.j < width - 1)
    //                {
    //                    if (tileMap[a.i, a.j].type == 1)
    //                    {
    //                        tileMap[a.i, a.j].SetColor(Color.white);
    //                    }
    //                }
    //            }

    //            if (SpeedAnimation != 0)
    //                yield return waitsec;
    //        }
    //        else if (walls[i].type == 2)
    //        {
    //            tileMap[w1.i, w1.j].SetType(1);
    //            tileMap[w1.i, w1.j].SetColor(Color.red);

    //            if (SpeedAnimation != 0)
    //                yield return waitsec;
    //        }
    //    }
    //    coroutine = null;
    //    yield return waitsec;
    //}

    void AnimPathImmd(CollectPath cpath)
    {
        var listUsed = new List<NodeAS>();
        for (int i = 0; i < cpath.workList.Count; i++)
        {
            var tt = cpath.workList[i];
            tileMap[tt.i, tt.j].SetColor(Color.yellow);
            listUsed.Add(new NodeAS(tt.i, tt.j, true));

            NodeAS[] temp = new NodeAS[] {
                GetNodeAS(tt.i - 1, tt.j),
                GetNodeAS(tt.i + 1, tt.j),
                GetNodeAS(tt.i, tt.j - 1),
                GetNodeAS(tt.i, tt.j + 1)};

            foreach (var retVal in temp)
            {
                if (retVal != null && retVal.isWalkable && !listUsed.Contains(retVal))
                    tileMap[retVal.i, retVal.j].SetColor(Color.cyan);
            }
        }

        foreach (var tile in cpath.pathList)
        {
            tileMap[tile.i, tile.j].SetColor(Color.red);
        }
    }

    // анимация поиска пути всех 3 алгоритмов
    //IEnumerator AnimPath(CollectPath cpath)
    //{
    //    var waitsec = new WaitForSeconds(SpeedAnimation);
    //    var listUsed = new List<NodeAS>();
    //    for (int i = 0; i < cpath.workList.Count; i++)
    //    {
    //        var tt = cpath.workList[i];
    //        tileMap[tt.i, tt.j].SetColor(Color.yellow);
    //        listUsed.Add(new NodeAS(tt.i, tt.j, true));

    //        NodeAS[] temp = new NodeAS[] {
    //            GetNodeAS(tt.i - 1, tt.j),
    //            GetNodeAS(tt.i + 1, tt.j),
    //            GetNodeAS(tt.i, tt.j - 1),
    //            GetNodeAS(tt.i, tt.j + 1)};

    //        foreach (var retVal in temp)
    //        {
    //            if (retVal != null && retVal.isWalkable && !listUsed.Contains(retVal))
    //                tileMap[retVal.i, retVal.j].SetColor(Color.cyan);
    //        }
    //        if (SpeedAnimation != 0)
    //            yield return waitsec;
    //    }

    //    foreach (var tile in cpath.pathList)
    //    {
    //        tileMap[tile.i, tile.j].SetColor(Color.red);
    //        yield return new WaitForSeconds(0.0001f);
    //    }

    //    coroutine = null;
    //}


    public static NodeAS GetNodeAS(int i, int j)
    {
        if (i >= 0 && j >= 0)
        {
            if (j < width && i < height)
            {
                var type = tileMap[i, j].type;
                var node = new NodeAS(i, j, type == 0 ? false : true);
                return node;
            }
        }
        return null;
    }

    public static NodeDi GetNodeDi(int i, int j)
    {
        if (i >= 0 && j >= 0)
        {
            if (j < width && i < height)
            {
                var type = tileMap[i, j].type;
                var node = new NodeDi(i, j, type == 0 ? false : true);
                return node;
            }
        }
        return null;
    }

    public static NodeJPS GetNodeJPS(int i, int j)
    {
        if (i >= 0 && j >= 0)
        {
            if (j < width && i < height)
            {
                var type = tileMap[i, j].type;
                var node = new NodeJPS(i, j, type == 0 ? false : true);
                return node;
            }
        }
        return null;
    }

    public static void ChangeSize(int w, int h)
    {
        widthMaze = w;
        heightMaze = h;
        width = widthMaze * 2 + 1;
        height = heightMaze * 2 + 1;
    }
}
