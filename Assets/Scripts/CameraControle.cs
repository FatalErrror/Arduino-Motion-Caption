using UnityEngine;

public class CameraControle : MonoBehaviour
{
    public Transform Camera;
    public float MovementSpeed = 0.03f;
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

        if (Input.GetMouseButtonDown(1))
        {
            _previosMousePosition = Input.mousePosition;
            //GameManager._GameManager.Log("pressed left button");
        }
        else if (Input.GetMouseButton(1))
        {
            var rotate = Input.mousePosition - _previosMousePosition;
            _previosMousePosition = Input.mousePosition;

            rotate = new Vector3(-rotate.y * 0.2f, rotate.x * 0.2f, 0);
            rotate = new Vector3(Camera.eulerAngles.x + rotate.x, Camera.eulerAngles.y + rotate.y, 0);
            Camera.eulerAngles = rotate;

            if (Input.GetKey(KeyCode.A)) Camera.Translate(Vector3.left * MovementSpeed);
            if (Input.GetKey(KeyCode.S)) Camera.Translate(Vector3.back * MovementSpeed);
            if (Input.GetKey(KeyCode.W)) Camera.Translate(Vector3.forward * MovementSpeed);
            if (Input.GetKey(KeyCode.D)) Camera.Translate(Vector3.right * MovementSpeed);
            if (Input.GetKey(KeyCode.Q)) Camera.Translate(Vector3.down * MovementSpeed);
            if (Input.GetKey(KeyCode.E)) Camera.Translate(Vector3.up * MovementSpeed);

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
