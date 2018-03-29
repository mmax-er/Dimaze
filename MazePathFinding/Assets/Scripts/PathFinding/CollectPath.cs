using System.Collections.Generic;

// класс выполняет поиск пути указанному алгоритму, 
// и преобразует полученные данные в один формат чтобы не писать три функции для аниций
public class CollectPath
{
    public List<Point> workList;
    public List<Point> pathList;
    public float time;

    public CollectPath(Point s, Point e, int index)
    {
        workList = new List<Point>();
        pathList = new List<Point>();

        if (index == 1)
        {
            var path = new PathFinderAStar(new NodeAS(s.i, s.j, true), new NodeAS(e.i, e.j, true));

            foreach (var node in path.workList)
                workList.Add(new Point(node.i, node.j));

            foreach (var node in path.pathList)
                pathList.Add(new Point(node.i, node.j));

            time = path.time;
        }
        else if(index == 2)
        {
            var path = new PathFinderDijkstra(new NodeDi(s.i, s.j, true), new NodeDi(e.i, e.j, true));

            foreach (var node in path.workList)
                workList.Add(new Point(node.i, node.j));

            foreach (var node in path.pathList)
                pathList.Add(new Point(node.i, node.j));

            time = path.time;
        }
        else if(index == 3)
        {
            var path = new PathFindingJPF(new NodeJPS(s.i, s.j, true), new NodeJPS(e.i, e.j, true));

            foreach (var node in path.workList)
                workList.Add(new Point(node.i, node.j));

            foreach (var node in path.pathList)
                pathList.Add(new Point(node.i, node.j));

            time = path.time;
        }
    }
}

