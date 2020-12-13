using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class VirtualSkeleton : MonoBehaviour
{
    public const int SENSORS_COUNT = 8;
    public const float TO_RAD = 0.01745329252f;
    public const float TO_DEG_PER_SEC = 16.384f;


    public Skeleton Skeleton;
    public GameObject Container;


    public float SettingDynamicOffestsTime = 120, UseDinamicOffsetsDeltaTime = 5;
    public GameObject Warrning;


    private bool[] _isContole;
    private int[] _numberMap = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
    private float[] _startRotation, _offsets, _dynamicOffsets;


    private PortReader _portReader;
    private Queue<string> _data = new Queue<string>();
    private MadgwickAHRS[] _dmp;
    private Transform[] VirtualSensors;
    

    void Start()
    {
        _isContole = new bool[(int)Skeleton.Indexs.Lenght];
        _numberMap = new int[(int)Skeleton.Indexs.Lenght];
        for (int i = 0; i < (int)Skeleton.Indexs.Lenght; i++)
        {
            _isContole[i] = PlayerPrefs.GetInt("IsContoleable" + i, 0) > 0;
            _numberMap[i] = PlayerPrefs.GetInt("NumberMap" + i, i);
        }


        VirtualSensors = new Transform[SENSORS_COUNT];
        _dmp = new MadgwickAHRS[SENSORS_COUNT];
        _startRotation = new float[SENSORS_COUNT];
        _offsets = new float[SENSORS_COUNT];
        _dynamicOffsets = new float[SENSORS_COUNT];


        Transform[] transforms = Skeleton.GetBonesArray();
        for (int i = 0; i < SENSORS_COUNT; i++)
        {
            VirtualSensors[i] = transforms[i];
            _dmp[i] = new MadgwickAHRS();
        }



        for (int i = 0; i < SENSORS_COUNT; i++)
        {
            Transform item = VirtualSensors[i];
            Transform container = Instantiate(Container, item).transform;
            container.parent = item.parent;
            item.parent = container;
            _startRotation[i] = item.rotation.eulerAngles.y;
            if (_isContole.Length > i)
            {
                if (_isContole[i]) container.parent = Skeleton.GetRoot();
            }
        }



        StartPortReader();
    }

    public void StartPortReader()
    {
        _portReader = new PortReader(PlayerPrefs.GetString("Port", "COM11"), PlayerPrefs.GetInt("Freqancy", 115200));
        _portReader.ParseData += AsyncDataParser;


        if (_portReader != null)
        {
            try
            {
                _portReader.Begin();
            }
            catch (System.Exception e)
            {
                MainUI.LogError("ERROR: " + e.Message);

            }
        }
    }

    public void StopPortReader()
    {
        _portReader.ParseData -= AsyncDataParser;
        try
        {
            _portReader.End();
        }
        catch (System.Exception e)
        {
            MainUI.LogError("ERROR: " + e.Message);
        }
    }


    private void FixedUpdate()
    {
        DataParser();
        UpdateBonesAngles();
    }

    public void Calibrate()
    {
        Invoke(nameof(CalibrateImmediately), 3);
    }

    public void CalibrateImmediately()
    {
        for (int i = 0; i < VirtualSensors.Length; i++)
        {
            _offsets[i] = _startRotation[i] - (VirtualSensors[i].rotation.eulerAngles.y - _offsets[i]);
        }
    }


    public void StartSettingDynamicOffsets()
    {
        if (!Warrning.activeSelf)
        {
            StopUseDynamicOffsets();
            Warrning.SetActive(true);
            Transform[] transforms = Skeleton.GetBonesArray();
            for (int i = 0; i < transforms.Length; i++)
            {
                _dynamicOffsets[i] = transforms[i].rotation.eulerAngles.y;
            }
            Warrning.GetComponent<UnityEngine.UI.Text>().text = "DONT MOVE OR TOUCH SENSORS " + SettingDynamicOffestsTime + " sec";
            Invoke(nameof(SetDynamicOffsets), SettingDynamicOffestsTime);

            StartCoroutine(Timer());
        }
    }

    public IEnumerator<WaitForSecondsRealtime> Timer()
    {
        for (int i = 0; i < SettingDynamicOffestsTime; i++)
        {
            Warrning.GetComponent<UnityEngine.UI.Text>().text = "DONT MOVE OR TOUCH SENSORS " + (SettingDynamicOffestsTime - i) + " sec";
            yield return new WaitForSecondsRealtime(1);
        }
    }


    private void SetDynamicOffsets()
    {
        Transform[] transforms = Skeleton.GetBonesArray();
        for (int i = 0; i < transforms.Length; i++)
        {
            _dynamicOffsets[i] = (transforms[i].rotation.eulerAngles.y - _dynamicOffsets[i]) / (SettingDynamicOffestsTime / UseDinamicOffsetsDeltaTime);
        }
        Warrning.SetActive(false);
        UseDynamicOffsets();
    }

    private void UseDynamicOffsets()
    {
        for (int i = 0; i < _offsets.Length; i++)
        {
            _offsets[i] += _dynamicOffsets[i];
        }
        Invoke(nameof(UseDynamicOffsets), UseDinamicOffsetsDeltaTime);
    }

    public void StopUseDynamicOffsets()
    {
        Warrning.SetActive(false);
        StopAllCoroutines();
        CancelInvoke(nameof(SetDynamicOffsets));
        CancelInvoke(nameof(UseDynamicOffsets));
    }




    private void UpdateBonesAngles()
    {
        for (int i = 0; i < SENSORS_COUNT; i++)
        {
            Vector4 Data = _dmp[i].GetQuaternion();
            //
            Quaternion q = new Quaternion(Data.x, Data.y, -Data.z, Data.w);

            Transform item = Skeleton.GetBonesArray()[i];
            item.localRotation = q;
            item.Rotate(0, _offsets[i], 0, Space.World);
        }
    }



    public void AsyncDataParser(string data)
    {
        _data.Enqueue(data);
    }

    public void DataParser()
    {
        for (int i = 0; i < _data.Count - 1; i++)
        {
            var stream = _data.Dequeue();
            if (stream[0] == '|')
            {
                MainUI.Self.UpdateLogField(stream);
                ParaseSensorsData(stream);
            }
            else
            {
                if (stream[0] == '>' || stream[0] == '.' || stream[0] == '*')
                {
                    MainUI.AddToPrevLog(stream);
                    MainUI.Self.AddToLogField(stream);
                }
                else
                {
                    MainUI.Log(stream);
                    MainUI.Self.UpdateLogField(stream);
                }
            }
        }
        if (_data.Count > 0)
        {
            var stream = _data.Dequeue();
            if (stream[0] == '|')
            {
                MainUI.Self.UpdateLogField(stream);
                ParaseSensorsData(stream);
            }
            else
            {
                if (stream[0] == '>' || stream[0] == '.' || stream[0] == '*')
                {
                    MainUI.AddToPrevLog(stream);
                    MainUI.Self.AddToLogField(stream);
                }
                else
                {
                    MainUI.Log(stream);
                    MainUI.Self.UpdateLogField(stream);
                }
            }
        }
    }

    private void ParaseSensorsData(string data)
    {
        string[] datas = data.Split('|');
        for (int i = 1; i < datas.Length; i++)
        {
            string[] data1 = datas[i].Split('=');
            int index = _numberMap[int.Parse(data1[0])];
            string[] data2 = data1[1].Split('_');

            float gx = StrToF(data2[1]) * TO_RAD / TO_DEG_PER_SEC;
            float gy = StrToF(data2[2]) * TO_RAD / TO_DEG_PER_SEC;
            float gz = StrToF(data2[3]) * TO_RAD / TO_DEG_PER_SEC;
            float ax = StrToF(data2[4]);
            float ay = StrToF(data2[5]);
            float az = StrToF(data2[6]);



            _dmp[index].UpdateIMU(long.Parse(data2[0]) / 1000f, gx, gy, gz, ax, ay, az);
        }
    }



    private float StrToF(string value)
    {
        return float.Parse(value, CultureInfo.GetCultureInfo("en-GB"));
    }




    public void AddOffset(int boneIndex, float offset)
    {
        _offsets[boneIndex] += offset;
    }

}


public class V4AverageFilter
{
    private Queue<Vector4> _data;
    private Vector4 _sum, _prevVal, _newVal;
    public float P;

    public V4AverageFilter(int count)
    {
        _data = new Queue<Vector4>();
        for (int i = 0; i < Mathf.Abs(count); i++)
        {
            _data.Enqueue(Vector4.zero);
        }
    }

    public void NewValue(Vector4 value)
    {
        /*_data.Enqueue(value);
        _sum += value;
        _sum -= _data.Dequeue();*/
        if (Mathf.Abs(value.y - _prevVal.y) > 0.1f || Mathf.Abs(value.z - _prevVal.z) > P) _prevVal = _newVal;
        _newVal = value;
    }

    public Vector4 GetAverageValue()
    {
        /*return _sum / _data.Count;*/
        return new Vector4(
            _newVal.x,
            Mathf.Abs(_newVal.y - _prevVal.y) > P ? _newVal.y : _prevVal.y,
            Mathf.Abs(_newVal.z - _prevVal.z) > P ? _newVal.z : _prevVal.z,
            _newVal.w
        );
    }

    public void SetFilterCapacity(int value)
    {
        var count = value - _data.Count;
        if (count < 0)
            for (int i = 0; i < Mathf.Abs(count); i++)
            {
                _data.Dequeue();
            }
        else
            for (int i = 0; i < count; i++)
            {
                _data.Enqueue(Vector4.zero);
            }
    }

    public int GetFilterCapacity()
    {
        return _data.Count;
    }

}
