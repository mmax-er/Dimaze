using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

public class PathFinderAStar
{
    public List<NodeAS> workList;
    public List<NodeAS> pathList;
    public float time;

    List<NodeAS> openSet;
    private NodeAS[,] matrix;
    private int width;
    private int height;

    // конструктор класса и здесь выполняется алгоритм
    public PathFinderAStar(NodeAS start, NodeAS goal)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        height = TileSetManager.height;
        width = TileSetManager.width;
        matrix = new NodeAS[height, width];

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                matrix[i, j] = TileSetManager.GetNodeAS(i, j);
                matrix[i, j].isOpen = true;
            }
        }

        start = matrix[start.i, start.j];
        goal = matrix[goal.i, goal.j];

        pathList = new List<NodeAS>();
        workList = new List<NodeAS>();
        openSet = new List<NodeAS>();

        openSet.Add(start);
        start.isOpen = false;
        start.gScore = 0;
        start.hScore = GetDistance(start, goal);

        while (openSet.Count > 0)
        {
            int index = GetMinFNodeAS();
            if (index < 0)
            {
                break;
            }
            var current = openSet[index];
            workList.Add(current);
            openSet.RemoveAt(index);

            if (current.Equals(goal))
            {
                RetracePath(start, current);
                break;
            }

            foreach (var neighbor in GetNeighbours(current))
            {
                float newGScore = current.gScore + GetDistance(current, neighbor);

                neighbor.gScore = newGScore;
                neighbor.hScore = GetDistance(neighbor, goal);
                neighbor.fScore = newGScore + neighbor.hScore;
                neighbor.parent = current;
                neighbor.isOpen = false;
                openSet.Add(neighbor);
            }
        }

        sw.Stop();
        time = (sw.ElapsedTicks / (float)Stopwatch.Frequency) * 1000.0f;
    }

    // функция проходит от конечной точки до старта и запоминает этот путь
    private void RetracePath(NodeAS startNodeAS, NodeAS endNodeAS)
    {
        NodeAS currentNodeAS = endNodeAS;

        while (true)
        {
            pathList.Add(currentNodeAS);
            if (currentNodeAS == startNodeAS)
                break;
            currentNodeAS = currentNodeAS.parent;
        }

        pathList.Reverse();
    }

    private List<NodeAS> GetNeighbours(NodeAS NodeAS)
    {
        List<NodeAS> retList = new List<NodeAS>();

        NodeAS[] temp = new NodeAS[] {
            matrix[NodeAS.i - 1, NodeAS.j],
            matrix[NodeAS.i + 1, NodeAS.j],
            matrix[NodeAS.i, NodeAS.j - 1],
            matrix[NodeAS.i, NodeAS.j + 1] };

        foreach (var retVal in temp)
        {
            if (retVal != null && retVal.isWalkable && retVal.isOpen)
                retList.Add(retVal);
        }

        return retList;
    }

    // функция возвращает вершину (клетку) с минимальным значением f (fScore)
    private int GetMinFNodeAS()
    {
        float minF = Mathf.Infinity;
        int index = int.MaxValue;

        for (int i = 0; i < openSet.Count; i++)
        {
            if (minF > openSet[i].fScore)
            {
                minF = openSet[i].fScore;
                index = i;
            }
        }

        if (index >= 0 && index < openSet.Count)
            return index;
        else
            return -1;
    }

    private float GetDistance(NodeAS posA, NodeAS posB)
    {
        return Vector2.Distance(posA.GetVector2(), posB.GetVector2());
    }
}