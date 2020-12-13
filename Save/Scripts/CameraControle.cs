using UnityEngine;

public class CameraControle : MonoBehaviour
{
    public Transform Camera;
    private Vector3 _previosMousePosition;

    // Start is called before the first frame update
    void Start()
    {
        if (!Camera) Camera = transform;
    }

    // Update is called once per frame
    void Update()
    {
        Camera.Translate(0, 0, (float)(Input.mouseScrollDelta.y * 0.2));
        if (Input.GetMouseButtonDown(0))
        {
            _previosMousePosition = Input.mousePosition;
            //GameManager._GameManager.Log("pressed left button");
        }
        else if (Input.GetMouseButton(0))
        {
            _previosMousePosition = Input.mousePosition - _previosMousePosition;
            _previosMousePosition = new Vector3(-_previosMousePosition.y * 0.2f, _previosMousePosition.x * 0.2f, 0);
            _previosMousePosition = new Vector3(Camera.eulerAngles.x + _previosMousePosition.x, Camera.eulerAngles.y + _previosMousePosition.y, 0);
            Camera.eulerAngles = _previosMousePosition;
            _previosMousePosition = Input.mousePosition;
        }
        if (Input.GetMouseButtonDown(2))
        {
            _previosMousePosition = Input.mousePosition;
            //GameManager._GameManager.Log("pressed middle button");
        }
        else if (Input.GetMouseButton(2))
        {
            _previosMousePosition = (Input.mousePosition - _previosMousePosition) * -0.005f;
            Camera.Translate(_previosMousePosition);
            _previosMousePosition = Input.mousePosition;
        }
    }
}
