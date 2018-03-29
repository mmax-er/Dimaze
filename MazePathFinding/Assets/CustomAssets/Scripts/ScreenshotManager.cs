using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenshotManager : MonoBehaviour
{
    public enum MazeType
    {
        KRUSALA,
        EULER,
        GROWING_TREE
    }

    public enum PathType
    {
        A_STAR,
        DIJKSTRY,
        JUMP_POINT
    }

    //public RawImage testImg;

    //[System.Serializable]
    //public class SequenceStep
    //{
    //    public int w = 5;
    //    public int h = 5;
    //    public Vector3 startPos;
    //    public Vector3 endPos;
    //    public int maze;
    //    public int path;
    //}

    static ScreenshotManager _instance;
    public static ScreenshotManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public UIManager uiManager;
    public TileSetManager tileSetManager;
    public Transform startPoint;
    public Transform endPoint;

    //public List<SequenceStep> sequence = new List<SequenceStep>();

    void Start()
    {
        _instance = this;
        Application.runInBackground = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //SaveSequenceStep();
            //Capture();
            if (!_isRunning) StartCoroutine(Run());
        }
    }

    public void AdjustCamera(int w, int h)
    {
        int maxDimention = Mathf.Max(w, h);

        float hPos = (maxDimention + .5f) * .5f;

        float vPos = maxDimention * 4;

        transform.position = new Vector3(hPos, vPos, hPos);
    }

    //void SaveSequenceStep()
    //{
    //    SequenceStep step = new SequenceStep();
    //    step.w = int.Parse(uiManager.inputWidth.text);
    //    step.h = int.Parse(uiManager.inputHeight.text);
    //    step.startPos = startPoint.position;
    //    step.endPos = endPoint.position;

    //    if (uiManager.toggleMaze[0].isOn)
    //    {
    //        step.maze = 1;
    //    }
    //    else if (uiManager.toggleMaze[1].isOn)
    //    {
    //        step.maze = 2;

    //    }
    //    else if (uiManager.toggleMaze[2].isOn)
    //    {
    //        step.maze = 3;
    //    }

    //    if (uiManager.togglePath[0].isOn)
    //    {
    //        step.path = 1;
    //    }
    //    else if (uiManager.togglePath[1].isOn)
    //    {
    //        step.path = 2;

    //    }
    //    else if (uiManager.togglePath[2].isOn)
    //    {
    //        step.path = 3;
    //    }

    //    sequence.Add(step);
    //}

    string GetImageName(MazeType maze, PathType path, int size)
    {
        string name = string.Format("{0}_{1}_{2}x{2}.png", maze.ToString(), path.ToString(), size);
        Debug.Log(name);
        return name;
    }

    bool _isRunning = false;

    IEnumerator Run()
    {
        _isRunning = true;
        MazeType[] mazes = System.Enum.GetValues(typeof(MazeType)) as MazeType[];
        PathType[] paths = System.Enum.GetValues(typeof(PathType)) as PathType[];
        int[] sizes = new int[] { 15, 30, 60 };

        for (int i = 0; i < mazes.Length; i++)
        {
            uiManager.toggleMaze[i].isOn = true;

            for (int j = 0; j < paths.Length; j++)
            {
                uiManager.togglePath[j].isOn = true;

                for (int k = 0; k < sizes.Length; k++)
                {
                    List<List<Texture2D>> captures = new List<List<Texture2D>>();
                    startPoint.position = new Vector3(1, 0, sizes[k] * 2 - 1);

                    uiManager.inputWidth.text = sizes[k].ToString();
                    uiManager.inputHeight.text = sizes[k].ToString();
                    uiManager.OnButtonClick_Generate();

                    int x1 = 1;
                    int y1 = sizes[k] * 2 - 1;

                    endPoint.position = new Vector3(x1, 0, y1);
                    uiManager.OnButtonClick_FindPath();
                    yield return new WaitForSeconds(.2f);

                    for (int l = 0; l < 3; l++)
                    {
                        uiManager.inputWidth.text = sizes[k].ToString();
                        uiManager.inputHeight.text = sizes[k].ToString();
                        uiManager.OnButtonClick_Generate();

                        captures.Add(new List<Texture2D>());

                        //int x1 = 1;
                        //int y1 = sizes[k] * 2 - 1;

                        //endPoint.position = new Vector3(x1, 0, y1);
                        //uiManager.OnButtonClick_FindPath();

                        //yield return new WaitForSeconds(1f);

                        for (int m = 0; m < 3; m++)
                        //for (int m = 0; m < 3; m++)
                        {
                            int x = m < 1 ? 1 : sizes[k] * 2 - 1;
                            int y = m != 1 ? 1 : sizes[k] * 2 - 1;

                            endPoint.position = new Vector3(x, 0, y);
                            uiManager.OnButtonClick_FindPath();

                            //TileSetManager.width = sizes[k];
                            //TileSetManager.height = sizes[k];

                            //if (l == 0 && m == 0) uiManager.OnButtonClick_FindPath();
                            //tileSetManager.StarGenerateMaze(i + 1);
                            //tileSetManager.StartFindingPath(j + 1);

                            yield return new WaitForEndOfFrame();

                            captures[l].Add(Capture());

                            //yield break;
                            //Debug.Log(string.Format("{0} - {1} - {2}x{2} - #{3}", mazes[i].ToString(), paths[j].ToString(), sizes[k], l + 1));
                        }
                    }

                    SaveScreenshot(captures, GetImageName(mazes[i], paths[j], sizes[k]));
                }
            }
        }
        _isRunning = false;
        Debug.Log("DONE!");
    }

    void SaveScreenshot(List<List<Texture2D>> captures, string filename)
    {
        int singleW = captures[0][0].width;
        int singleH = captures[0][0].height;
        //Debug.LogError(singleW + "x" + singleH);
        int w = singleW * captures[0].Count;
        int h = singleH * captures.Count;
        //Debug.LogError(w + "x" + h);

        Texture2D mazeAlgScreenshot = new Texture2D(w, h);

        w = 0;
        h = 0;

        for (int r = 0; r < captures.Count; r++)
        {
            for (int c = 0; c < captures[r].Count; c++)
            {
                //mazeAlgScreenshot.SetPixels(w, h, singleW, singleH, captures[r][c].GetPixels());
                //Debug.LogError(">>>> pic (" + w + ": " + h + ")");

                mazeAlgScreenshot.SetPixels(w, h, singleW, singleH, captures[r][c].GetPixels());

                w += singleW;
            }
            w = 0;
            h += singleH;
        }

        mazeAlgScreenshot.Apply();

        byte[] bytes = mazeAlgScreenshot.EncodeToPNG();
        //Debug.LogError(i + " / " + j);
        //string filename = GetImageName(mazes[i], paths[j], sizes[k]);

        string folderName = "screenshots/" + System.DateTime.Now.ToString().Replace("/", "-").Replace(" ", "-").Replace(":", "-") + "/";

        if (!System.IO.Directory.Exists(folderName))
        {
            System.IO.Directory.CreateDirectory(folderName);
        }

        System.IO.File.WriteAllBytes(folderName + filename, bytes);

        captures.Clear();
        Debug.Log("Screenshot saved");

    }

    Texture2D Capture()
    {
        Texture2D tex = new Texture2D(Screen.width, Screen.height);
        tex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        tex.Apply();

        return tex;
    }
}
