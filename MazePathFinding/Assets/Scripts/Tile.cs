using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    public Point point;
    public GameObject tileObj;
    public GameObject wallObj;
    public GameObject emptyObj;
    public int type = 0;

    private void Awake()
    {
        tileObj = gameObject;
        wallObj = transform.GetChild(0).gameObject;
        emptyObj = transform.GetChild(1).gameObject;
        SetType(type);
    }

    void Start()
    {
        emptyObj.GetComponent<Renderer>().material.color = Color.green;
    }

    void Update()
    {

    }

    public void SetPoint(Point p)
    {
        point = p;
        transform.position = new Vector3(p.j, 0, p.i);
        TileSetManager.tileMap[p.i, p.j] = this;
    }

    public void SetColor(Color color)
    {
        if(type == 1)
        {
            emptyObj.GetComponent<MeshRenderer>().material.color = color;
        }
    }

    public void SetType(int type)
    {
        this.type = type;

        if(type == 1)
        {
            emptyObj.GetComponent<MeshRenderer>().material.color = Color.white;
            emptyObj.SetActive(true);
            wallObj.SetActive(false);
        }
        else
        {
            emptyObj.SetActive(false);
            wallObj.SetActive(true);
        }
    }
}
