using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PathFinderDijkstra
{
    public List<NodeDi> workList;
    public List<NodeDi> pathList;
    public float time;

    private List<NodeDi> openList;
    private NodeDi[,] matrix;
    private int width;
    private int height;

    public PathFinderDijkstra(NodeDi start, NodeDi end)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        height = TileSetManager.height;
        width = TileSetManager.width;
        matrix = new NodeDi[height, width];
        
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                matrix[i, j] = TileSetManager.GetNodeDi(i, j);
                matrix[i, j].isOpen = true;
            }
        }

        pathList = new List<NodeDi>();
        NodeDi node = DijkstrasAlgo(matrix[start.i, start.j], matrix[end.i, end.j]);

        while (node != null)
        {
            pathList.Add(node);
            node = node.parent;
        }

        pathList.Reverse();
        sw.Stop();
        time = (sw.ElapsedTicks / (float)Stopwatch.Frequency) * 1000;
    }

    private NodeDi DijkstrasAlgo(NodeDi start, NodeDi end)
    {
        openList = new List<NodeDi>();
        workList = new List<NodeDi>();
        start.setWeight(0);
        start.isOpen = false;
        openList.Add(start);

        while (openList.Count > 0)
        {
            int index = GetMinFNodeAS();
            if (index < 0)
            {
                //UnityEngine.Debug.Log("index -1");
                break;
            }
            NodeDi current = openList[index];
            openList.RemoveAt(index);
            workList.Add(current);

            if (current.Equals(end))
            {
                return current;
            }

            foreach (NodeDi neighNode in GetNeighbours(current))
            {
                float distance = Vector3.Distance(neighNode.getPosition(), current.getPosition());
                distance = current.weight + distance;

                neighNode.isOpen = false;
                neighNode.setWeight(distance);
                neighNode.setParentNode(current);
                openList.Add(neighNode);
            }
        }
        return null;
    }

    // возвращает соседей указанной ячейки (тайла)
    private List<NodeDi> GetNeighbours(NodeDi node)
    {
        List<NodeDi> retList = new List<NodeDi>();
        NodeDi[] temp = new NodeDi[] {
            matrix[node.i - 1, node.j],
            matrix[node.i + 1, node.j],
            matrix[node.i, node.j - 1],
            matrix[node.i, node.j + 1] };

        foreach (var retVal in temp)
        {
            if (retVal != null && retVal.isWalkable && retVal.isOpen)
                retList.Add(retVal);
        }
        //UnityEngine.Debug.Log("nei count: " + retList.Count.ToString());

        return retList;
    }

    // функция возвращает вершину (клетку) с минимальным весом
    private int GetMinFNodeAS()
    {
        float minF = Mathf.Infinity;
        int index = int.MaxValue;

        for (int i = 0; i < openList.Count; i++)
        {
            if (minF > openList[i].weight)
            {
                minF = openList[i].weight;
                index = i;
            }
        }

        if (index >= 0 && index < openList.Count)
            return index;
        else
            return -1;
    }
}
