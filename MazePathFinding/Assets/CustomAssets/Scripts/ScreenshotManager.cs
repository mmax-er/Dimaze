using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotManager : MonoBehaviour {

    [System.Serializable]
    public class SequenceStep
    {
        public int w = 5;
        public int h = 5;
        public Vector3 startPos;
        public Vector3 endPos;
        public int maze;
        public int path;
    }

    static ScreenshotManager _instance;
    public static ScreenshotManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public UIManager uiManager;
    public Transform startPoint;
    public Transform endPoint;

    public List<SequenceStep> sequence = new List<SequenceStep>();

    void Start () {
        _instance = this;
    }
	
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space))
        {
            SaveSequenceStep();
        }
	}

    public void AdjustCamera(int w, int h)
    {
        int maxDimention = Mathf.Max(w, h);

        float hPos = (maxDimention + .5f) * .5f;

        float vPos = maxDimention * 4;

        transform.position = new Vector3(hPos, vPos, hPos);
    }

    void SaveSequenceStep()
    {
        SequenceStep step = new SequenceStep();
        step.w = int.Parse(uiManager.inputWidth.text);
        step.h = int.Parse(uiManager.inputHeight.text);
        step.startPos = startPoint.position;
        step.endPos = endPoint.position;

        if (uiManager.toggleMaze[0].isOn)
        {
            step.maze = 1;
        }
        else if (uiManager.toggleMaze[1].isOn)
        {
            step.maze = 2;

        }
        else if (uiManager.toggleMaze[2].isOn)
        {
            step.maze = 3;
        }

        if (uiManager.togglePath[0].isOn)
        {
            step.path = 1;
        }
        else if (uiManager.togglePath[1].isOn)
        {
            step.path = 2;

        }
        else if (uiManager.togglePath[2].isOn)
        {
            step.path = 3;
        }

        sequence.Add(step);
    }
}
