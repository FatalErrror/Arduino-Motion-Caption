using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LernQuaternion : MonoBehaviour
{
    public Transform test;
    [Range(-1.0f, 1.0f)]
    public float x = 1;
    [Range(-1.0f, 1.0f)]
    public float y;
    [Range(-1.0f, 1.0f)]
    public float z;
    [Range(-1.0f, 1.0f)]
    public float w;

    public bool _isLocal;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_isLocal)
            test.localRotation = new Quaternion(x, y, z, w);
        else
            test.rotation = new Quaternion(x, y, z, w);
    }
}
