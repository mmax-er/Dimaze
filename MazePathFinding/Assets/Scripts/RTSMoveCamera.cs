using UnityEngine;

// перемещения камеры и зум
// перемещения маркеров указывающие начало и конец пути
public class RTSMoveCamera : MonoBehaviour
{
    public int SpeedMoveArrow = 10;
    public int SpeedMoveMouse = 10;
    public GameObject StartPath;
    public GameObject EndPath;
    int border = 10;
    float camPosY = 20;

    void Update()
    {
        //CheckMoveCameraMouse();
        CheckMoveCameraArrow();

        var d = Input.GetAxis("Mouse ScrollWheel");
        camPosY -= d * 8.0f;

        if (Mathf.Abs(camPosY - transform.position.y) >= 0.1)
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, camPosY, transform.position.z), Time.deltaTime * 10f);

        if(Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonUp(0))
        {
            GetTilePosition(StartPath);
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonUp(1))
        {
            GetTilePosition(EndPath);
        }
    }


    // при наведении мышки в край экрана камера будет двигаться в этом направлении
    private void CheckMoveCameraMouse()
    {
        float delta = Time.deltaTime * SpeedMoveMouse;

        if (Mathf.Abs(Screen.width - Input.mousePosition.x) < border)
        {
            transform.position += new Vector3(delta, 0, 0);
        }
        else if (Input.mousePosition.x < border)
        {
            transform.position += new Vector3(-delta, 0, 0);
        }

        if (Mathf.Abs(Screen.height - Input.mousePosition.y) < border)
        {
            transform.position += new Vector3(0, 0, delta);
        }
        else if (Input.mousePosition.y < border)
        {
            transform.position += new Vector3(0, 0, -delta);
        }
    }

    // перемещения камеры с помощью стрелок на клавиатуре
    private void CheckMoveCameraArrow()
    {
        float delta = Time.deltaTime * SpeedMoveArrow;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.position += new Vector3(0, 0, delta);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position += new Vector3(0, 0, -delta);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position += new Vector3(-delta, 0, 0);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position += new Vector3(delta, 0, 0);
        }
    }

    private void GetTilePosition(GameObject markerObj)
    {
        var pointRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(pointRay, out hit, 1000.0f))
        {
            if(hit.collider != null)
            {
                var pos = hit.collider.gameObject.transform.position;
                markerObj.transform.position = pos;
            }
        }
    }
}
