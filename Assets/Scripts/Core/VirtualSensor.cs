using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class VirtualSensor
{
    public const float TO_RAD = 0.01745329252f;
    public const float TO_DEG_PER_SEC = 16.384f;

    public delegate Vector3 func(Vector3 vector);
    public func Filtrate;

    private MadgwickAHRS _dmp;
    private Transform _transform;

    private float _startYRotation;
    private float _yRotationOffset;
    private Vector3 _dynamicGyroOffset;

    public bool UseFilter = true;
    public float TFThrashold = 0.015f;


    public bool UseFilter1 = true;
    public float TFThrashold1 = 0.015f;
    public bool UseFilter2 = true;
    public float TFThrashold2 = 0.015f;


    private Filters.Vector4Filters.ThrasholdFilterL2 _thrasholdFilter;

    private Filters.Vector3Filters.ThrasholdFilter _thrasholdFilter1;
    private Filters.Vector3Filters.ThrasholdFilter _thrasholdFilter2;



    // Start is called before the first frame update
    public VirtualSensor(Transform transform, GameObject container, Transform root, bool isControle)
    {
        _transform = transform;
        Transform container1 = GameObject.Instantiate(container, transform).transform;
        container1.parent = transform.parent;
        transform.parent = container1;
        _startYRotation = transform.rotation.eulerAngles.y;
         if (isControle) container1.parent = root;
        _dmp = new MadgwickAHRS();

        _thrasholdFilter = new Filters.Vector4Filters.ThrasholdFilterL2(TFThrashold);

        _thrasholdFilter1 = new Filters.Vector3Filters.ThrasholdFilter(TFThrashold1);
        _thrasholdFilter2 = new Filters.Vector3Filters.ThrasholdFilter(TFThrashold2);
        //Filtrate = Filtrating;
    }

    public void ResetDMP()
    {
        _dmp = new MadgwickAHRS();
    }

    private Vector4 Filtrating(Vector4 data)
    {
        _thrasholdFilter.Thrashold = TFThrashold;
        _thrasholdFilter.NewValue(data);

        if (UseFilter)
        {
            return _thrasholdFilter.GetValue();
        }
        return data;
    }

    private Vector3 Filtrating1(Vector3 data)
    {
        _thrasholdFilter1.Thrashold = TFThrashold1;
        _thrasholdFilter1.NewValue(data);

        if (UseFilter1)
        {
            return _thrasholdFilter1.GetValue();
        }
        return data;
    }

    private Vector3 Filtrating2(Vector3 data)
    {
        _thrasholdFilter2.Thrashold = TFThrashold2;
        _thrasholdFilter2.NewValue(data);

        if (UseFilter2)
        {
            return _thrasholdFilter2.GetValue();
        }
        return data;
    }

    public void UpdateData(long deltaTime, float[] data)
    {
        float gx = data[0];
        float gy = data[1];
        float gz = data[2];
        float ax = data[3];
        float ay = data[4];
        float az = data[5];

        //
        //if (Filtrate.Method != null) 
        //{
        var g = Filtrating1(new Vector3(gx, gy, gz));
        gx = g.x;
        gy = g.y;
        gz = g.z;

        var a = Filtrating2(new Vector3(ax, ay, az));
        ax = a.x;
        ay = a.y;
        az = a.z;

        //}
        //

        gx = gx * TO_RAD / TO_DEG_PER_SEC;
        gy = gy * TO_RAD / TO_DEG_PER_SEC;
        gz = gz * TO_RAD / TO_DEG_PER_SEC;
        _dmp.UpdateIMU(deltaTime / 1000f, gx, gy, gz, ax, ay, az);
    }

    public void UpdateTransform()
    {
        Vector4 Data = _dmp.GetQuaternion(); 
        Data = Filtrating(Data);
        Quaternion q = new Quaternion(Data.x, Data.y, -Data.z, Data.w);

        _transform.localRotation = q;
        _transform.Rotate(0, _yRotationOffset, 0, Space.World);
    }

    public void Calibrate()
    {
        _yRotationOffset = _startYRotation - (_transform.rotation.eulerAngles.y - _yRotationOffset);
    }

    public void AddOffset(float value)
    {
        _yRotationOffset += value;
    }
}
