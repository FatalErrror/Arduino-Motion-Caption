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

    public Filters.Filters UseFilter;
    public int RAFCapacity = 10;
    public float TFThrashold = 100;

    private Filters.Vector4Filters.RuningAverageFilter _runingAverageFilter;
    private Filters.Vector4Filters.ThrasholdFilter _thrasholdFilter;
    private Filters.Vector4Filters.ThrasholdRuningAverageFilter _thrasholdRuningAverageFilter;





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

        _runingAverageFilter = new Filters.Vector4Filters.RuningAverageFilter(RAFCapacity);
        _thrasholdFilter = new Filters.Vector4Filters.ThrasholdFilter(TFThrashold);
        _thrasholdRuningAverageFilter = new Filters.Vector4Filters.ThrasholdRuningAverageFilter(TFThrashold, RAFCapacity);
        //Filtrate = Filtrating;
    }

    private Vector4 Filtrating(Vector4 data)
    {
        _runingAverageFilter.SetFilterCapacity(RAFCapacity);
        _thrasholdFilter.Thrashold = TFThrashold;
        _thrasholdRuningAverageFilter.Thrashold = TFThrashold;
        _thrasholdRuningAverageFilter.SetFilterCapacity(RAFCapacity);

        _runingAverageFilter.NewValue(data);
        _thrasholdFilter.NewValue(data);
        _thrasholdRuningAverageFilter.NewValue(data);

        switch (UseFilter)
        {
            case Filters.Filters.None:
                return data;
            case Filters.Filters.RuningAverageFilter:
                return _runingAverageFilter.GetValue();
            case Filters.Filters.ThrasholdFilter:
                return _thrasholdFilter.GetValue();
            case Filters.Filters.ThrasholdRuningAverageFilter:
                return _thrasholdRuningAverageFilter.GetValue();
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

        /*/
        if (Filtrate.Method != null) 
        { 
            var a = Filtrate(new Vector3(ax, ay, az));
            ax = a.x;
            ay = a.y;
            az = a.z;
        }
        /*/

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
