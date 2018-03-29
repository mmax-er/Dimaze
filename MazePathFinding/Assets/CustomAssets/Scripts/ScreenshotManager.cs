using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotManager : MonoBehaviour {

    static ScreenshotManager _instance;
    public static ScreenshotManager Instance
    {
        get
        {
            return _instance;
        }
    }

    void Start () {
        _instance = this;
    }
	
	void Update () {
		
	}

    public void AdjustCamera(int w, int h)
    {
        int maxDimention = Mathf.Max(w, h);

        float hPos = (maxDimention + .5f) * .5f;

        float vPos = maxDimention * 4;

        transform.position = new Vector3(hPos, vPos, hPos);
    }
}
