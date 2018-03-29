using UnityEngine;
using UnityEngine.UI;

// все связанное с интерфейсом
// обработка нажатий кнопок
// вывод данных
public class UIManager : MonoBehaviour
{
    public Toggle[] toggleMaze;
    public Toggle[] togglePath;
    public Slider sliderSpeed;
    public Text textSpeed;
    public Text textGenMazeTime;
    public Text textPathFindTime;
    public Text lengthShotPath;
    public Text lengthAllPath;

    public TileSetManager tileSetManager;
    public InputField inputWidth;
    public InputField inputHeight;

    void Start()
    {
        sliderSpeed.value = 0.01f;
        tileSetManager.uiManager = this;
    }

    public void SetGenerateMazeTime(float time)
    {
        textGenMazeTime.text = "Generate Maze Time: " + time.ToString() + " ms";
    }

    public void SetPathFindingTime(float time)
    {
        textPathFindTime.text = "Path Finding Time: " + time.ToString() + " ms";
    }

    public void SetLengthPath(int shortPath, int allPath)
    {
        lengthShotPath.text = "Length short path: " + shortPath.ToString() + " m";
        lengthAllPath.text = "Length all path: " + allPath.ToString() + " m";
    }


    public void OnValueChanged_SpeedAnim()
    {
        textSpeed.text = "Animation speed: " + sliderSpeed.value.ToString("0.0000");
        TileSetManager.SpeedAnimation = sliderSpeed.value;
    }

    public void OnButtonClick_Generate()
    {
        OnButtonClick_Reset();

        int w = int.Parse(inputWidth.text);
        int h = int.Parse(inputHeight.text);
        ScreenshotManager.Instance.AdjustCamera(w, h);

        if (toggleMaze[0].isOn)
        {
            tileSetManager.StarGenerateMaze(1);
        }
        else if (toggleMaze[1].isOn)
        {
            tileSetManager.StarGenerateMaze(2);

        }
        else if (toggleMaze[2].isOn)
        {
            tileSetManager.StarGenerateMaze(3);

        }
    }

    public void OnButtonClick_FindPath()
    {
        if (togglePath[0].isOn)
        {
            tileSetManager.StartFindingPath(1);
        }
        else if (togglePath[1].isOn)
        {
            tileSetManager.StartFindingPath(2);

        }
        else if (togglePath[2].isOn)
        {
            tileSetManager.StartFindingPath(3);
        }
    }


    public void OnButtonClick_Reset()
    {
        OnButtonClick_Stop();
        textGenMazeTime.text = "Generate Maze Time: 0";
        textPathFindTime.text = "Path Finding Time: 0";

        int w = int.Parse(inputWidth.text);
        int h = int.Parse(inputHeight.text);

        if (w != TileSetManager.widthMaze || h != TileSetManager.heightMaze)
        {
            TileSetManager.ChangeSize(w, h);
            tileSetManager.CreateTileGrid();
        }

        tileSetManager.ResetGrid();
    }

    public void OnButtonClick_Stop()
    {
        //tileSetManager.StopAnimation();
    }
}
