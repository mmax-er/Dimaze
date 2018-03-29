using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public class PathFindingJPF
{
    public List<NodeJPS> workList;
    public List<NodeJPS> pathList;
    public float time;

    private NodeJPS[,] matrix;
    private NodeJPS endNode;
    private NodeJPS startNode;
    private int width;
    private int height;
    private List<NodeJPS> openList;

    // это конструктор класса, здесь все начинается
    public PathFindingJPF(NodeJPS start, NodeJPS end)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        // размеры лабиринта
        height = TileSetManager.height;
        width = TileSetManager.width;

        // типчасова копия лабиринта
        matrix = new NodeJPS[height, width];

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                matrix[i, j] = TileSetManager.GetNodeJPS(i, j);
            }
        }

        // задается начало и конец пути
        startNode = matrix[start.i, start.j];
        endNode = matrix[end.i, end.j];

        pathList = new List<NodeJPS>();

        // запуск выполнения алгоритма
        FindPath();

        sw.Stop();
        // расчет времени выполнения
        time = (sw.ElapsedTicks / (float)Stopwatch.Frequency) * 1000;
    }

    // здесь выполняется основной цикл алгоритма
    private void FindPath()
    {
        workList = new List<NodeJPS>();
        openList = new List<NodeJPS>();
        NodeJPS tNode;

        startNode.g = 0;
        startNode.f = 0;

        openList.Add(startNode);
        startNode.isOpened = true;

        while (openList.Count > 0)
        {
            tNode = GetMinFNodeJPS(openList);
            tNode.isClosed = true;
            workList.Add(tNode);

            if (tNode.Equals(endNode))
            {
                RetracePath(startNode, endNode);
                break;
            }

            IdentifySuccessors(tNode);
        }
    }

    private void IdentifySuccessors(NodeJPS iNode)
    {
        int tEndI = endNode.i;
        int tEndJ = endNode.j;
        NodeJPS tJumpPoint;
        NodeJPS tJumpNode;

        foreach (var tNeighbor in FindNeighbors(iNode))
        {
            tJumpPoint = Jump(tNeighbor.i, tNeighbor.j, iNode.i, iNode.j);

            if (tJumpPoint != null)
            {
                tJumpNode = matrix[tJumpPoint.i, tJumpPoint.j];
                if (tJumpNode == null)
                {
                    if (endNode.Equals(tJumpPoint))
                        tJumpNode = matrix[tJumpPoint.i, tJumpPoint.j];
                }
                if (tJumpNode.isClosed)
                {
                    continue;
                }

                float tCurNodeToJumpNodeLen = Manhattan(Mathf.Abs(tJumpPoint.i - iNode.i), Mathf.Abs(tJumpPoint.j - iNode.j));
                float tStartToJumpNodeLen = iNode.h + tCurNodeToJumpNodeLen;

                if (!tJumpNode.isOpened || tStartToJumpNodeLen < tJumpNode.h)
                {
                    tJumpNode.h = tStartToJumpNodeLen;
                    tJumpNode.g = (tJumpNode.g == 0 ? Manhattan(Mathf.Abs(tJumpPoint.i - tEndI), Mathf.Abs(tJumpPoint.j - tEndJ)) : tJumpNode.g);
                    tJumpNode.f = tJumpNode.h + tJumpNode.g;
                    tJumpNode.parent = iNode;

                    if (!tJumpNode.isOpened)
                    {
                        openList.Add(tJumpNode);
                        tJumpNode.isOpened = true;
                    }
                }
            }
        }
    }

    private NodeJPS Jump(int iX, int iY, int iPx, int iPy)
    {
        if (!matrix[iX, iY].isWalkable)
        {
            return null;
        }
        else if (matrix[iX, iY].Equals(endNode))
        {
            return matrix[iX, iY];
        }

        int tDx = iX - iPx;
        int tDy = iY - iPy;

        if (tDx != 0)
        {
            // moving along x
            if (!matrix[iX + tDx, iY].isWalkable)
            {
                return matrix[iX, iY];
            }
        }
        else
        {
            if (!matrix[iX, iY + tDy].isWalkable)
            {
                return matrix[iX, iY];
            }
        }

        //  must check for perpendicular jump points
        if (tDx != 0)
        {
            if (Jump(iX, iY + 1, iX, iY) != null)
                return matrix[iX, iY];
            if (Jump(iX, iY - 1, iX, iY) != null)
                return matrix[iX, iY];
        }
        else // tDy != 0
        {
            if (Jump(iX + 1, iY, iX, iY) != null)
                return matrix[iX, iY];
            if (Jump(iX - 1, iY, iX, iY) != null)
                return matrix[iX, iY];
        }

        // keep going
        if (matrix[iX + tDx, iY].isWalkable && matrix[iX, iY + tDy].isWalkable)
        {
            return Jump(iX + tDx, iY + tDy, iX, iY);
        }
        else
        {
            return null;
        }
    }


    private List<NodeJPS> FindNeighbors(NodeJPS iNode)
    {
        int ti = iNode.i;
        int tj = iNode.j;
        List<NodeJPS> tNeighbors = new List<NodeJPS>();

        if (iNode.parent != null)
        {
            int tPi = iNode.parent.i;
            int tPj = iNode.parent.j;
            // get the normalized direction of travel
            int tDi = (ti - tPi) / Mathf.Max(Mathf.Abs(ti - tPi), 1);
            int tDj = (tj - tPj) / Mathf.Max(Mathf.Abs(tj - tPj), 1);

            if (tDi != 0)
            {
                if (ti + tDi >= 0 && ti + tDi < height && matrix[ti + tDi, tj].isWalkable)
                {
                    tNeighbors.Add(matrix[ti + tDi, tj]);
                }
                if (matrix[ti, tj + 1].isWalkable)
                {
                    tNeighbors.Add(matrix[ti, tj + 1]);
                }
                if (matrix[ti, tj - 1].isWalkable)
                {
                    tNeighbors.Add(matrix[ti, tj - 1]);
                }
            }
            else
            {
                if (tj + tDj > 0 && tj + tDj < width && matrix[ti, tj + tDj].isWalkable)
                {
                    tNeighbors.Add(matrix[ti, tj + tDj]);
                }
                if (matrix[ti + 1, tj].isWalkable)
                {
                    tNeighbors.Add(matrix[ti + 1, tj]);
                }
                if (matrix[ti - 1, tj].isWalkable)
                {
                    tNeighbors.Add(matrix[ti - 1, tj]);
                }
            }
        }
        else
        {
            tNeighbors = GetNeighbours(iNode);
        }

        return tNeighbors;
    }

    // прохождения пути в обратном направлении и запоминания этого пути
    private void RetracePath(NodeJPS startNodeAS, NodeJPS endNodeAS)
    {
        var pathL = new List<NodeJPS>();
        pathList = new List<NodeJPS>();
        NodeJPS currentNodeAS = endNodeAS;

        while (true)
        {
            pathL.Add(currentNodeAS);
            if (currentNodeAS == startNodeAS)
                break;
            currentNodeAS = currentNodeAS.parent;
        }

        // шлях йде від кінця до початку
        // тому потрібно відобразити список в зворотньому порядку, від старту до кінця
        pathL.Reverse();

        // цей алгоритм запамятовує шлях лише по точка поворотів
        // тому я ще додаю точки що є між точками поворотів, потрібно для анімації
        for (int i = 0; i < pathL.Count - 1; i++)
        {
            var n1 = pathL[i];
            var n2 = pathL[i + 1];

            int iR = n2.i - n1.i;
            int jR = n2.j - n1.j;

            if((Mathf.Abs(iR) == 1 && jR == 0) || (Mathf.Abs(jR) == 1 && iR == 0))
            {
                pathList.Add(n1);
                continue;
            }

            if (iR != 0 && jR == 0)
            {
                if (iR > 0)
                {
                    for (int k = 0; k < iR; k++)
                    {
                        pathList.Add(matrix[n1.i + k, n1.j]);
                    }
                }
                else
                {
                    for (int k = 0; k < iR * (-1); k++)
                    {
                        pathList.Add(matrix[n1.i - k, n1.j]);
                    }
                }
            }
            else if (iR == 0 && jR != 0)
            {
                if (jR > 0)
                {
                    for (int k = 0; k < jR; k++)
                    {
                        pathList.Add(matrix[n1.i, n1.j + k]);
                    }
                }
                else
                {
                    for (int k = 0; k < jR * (-1); k++)
                    {
                        pathList.Add(matrix[n1.i, n1.j - k]);
                    }
                }
            }
        }
        pathList.Add(pathL.Last());
    }

    private List<NodeJPS> GetNeighbours(NodeJPS node)
    {
        List<NodeJPS> retList = new List<NodeJPS>();
        NodeJPS[] temp = new NodeJPS[] {
            matrix[node.i - 1, node.j],
            matrix[node.i + 1, node.j],
            matrix[node.i, node.j - 1],
            matrix[node.i, node.j + 1] };

        foreach (var retVal in temp)
        {
            if (retVal != null && retVal.isWalkable)
                retList.Add(retVal);
        }

        return retList;
    }

    // Полученные клетки с минимальным значением f
    private NodeJPS GetMinFNodeJPS(List<NodeJPS> openSet)
    {
        float minF = Mathf.Infinity;
        int index = int.MaxValue;

        for (int i = 0; i < openSet.Count; i++)
        {
            if (minF > openSet[i].f)
            {
                minF = openSet[i].f;
                index = i;
            }
        }

        if (index >= 0 && index < openSet.Count)
        {
            var node = openSet[index];
            openSet.RemoveAt(index);
            return node;
        }
        else
            return null;
    }


    private static float Manhattan(int iDx, int iDy)
    {
        return (float)iDx + iDy;
    }


    private class JumpSnapshot
    {
        public int iX;
        public int iY;
        public int iPx;
        public int iPy;
        public int tDx;
        public int tDy;
        public int stage;

        public JumpSnapshot()
        {
            iX = 0;
            iY = 0;
            iPx = 0;
            iPy = 0;
            tDx = 0;
            tDy = 0;
            stage = 0;
        }
    }
}


