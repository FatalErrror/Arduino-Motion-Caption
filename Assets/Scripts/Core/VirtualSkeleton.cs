﻿using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class VirtualSkeleton : MonoBehaviour
{
    public const int SENSORS_COUNT = 8;
    
    public Skeleton Skeleton;
    public GameObject Container;


    public float SettingDynamicOffestsTime = 120, UseDinamicOffsetsDeltaTime = 5;
    public GameObject Warrning;


    private bool[] _isContole;
    private int[] _numberMap = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };

    private PortReader _portReader;
    private Queue<string> _data = new Queue<string>();

    private VirtualSensor[] VirtualSensors;
    

    void Start()
    {
        _isContole = new bool[(int)Skeleton.Indexs.Lenght];
        _numberMap = new int[(int)Skeleton.Indexs.Lenght];
        for (int i = 0; i < (int)Skeleton.Indexs.Lenght; i++)
        {
            _isContole[i] = PlayerPrefs.GetInt("IsContoleable" + i, 0) > 0;
            _numberMap[i] = PlayerPrefs.GetInt("NumberMap" + i, i);
        }

        VirtualSensors = new VirtualSensor[SENSORS_COUNT];

        Transform[] transforms = Skeleton.GetBonesArray();
        for (int i = 0; i < SENSORS_COUNT; i++)
        {
            VirtualSensors[i] = new VirtualSensor(transforms[i], Container, Skeleton.GetRoot(), _isContole[i]);
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
    }

    public void Calibrate()
    {
        StartCoroutine(CalibrateImmediately());
    }

    public IEnumerator<WaitForSecondsRealtime> CalibrateImmediately()
    {
        yield return new WaitForSecondsRealtime(3f);
        for (int i = 0; i < VirtualSensors.Length; i++)
        {
            VirtualSensors[i].Calibrate();
        }
    }


    public void StartSettingDynamicOffsets()
    {
        if (!Warrning.activeSelf)
        {
            StopUseDynamicOffsets();
            Warrning.SetActive(true);
            for (int i = 0; i < VirtualSensors.Length; i++)
            {
                
            }
            Warrning.GetComponent<UnityEngine.UI.Text>().text = "DONT MOVE OR TOUCH SENSORS " + SettingDynamicOffestsTime + " sec";

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
        SetDynamicOffsets();
    }


    private void SetDynamicOffsets()
    {
        Transform[] transforms = Skeleton.GetBonesArray();
        for (int i = 0; i < transforms.Length; i++)
        {
            //_dynamicOffsets[i] = (transforms[i].rotation.eulerAngles.y - _dynamicOffsets[i]) / (SettingDynamicOffestsTime / UseDinamicOffsetsDeltaTime);
        }
        Warrning.SetActive(false);
    }

    public void StopUseDynamicOffsets()
    {
        Warrning.SetActive(false);
        StopAllCoroutines();
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
                for (int i = 0; i < VirtualSensors.Length; i++)
                {
                    VirtualSensors[i].UpdateTransform();
                }
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
            float[] data3 = new float[data2.Length - 1];
            for (int j = 0; j < data3.Length; j++)
            {
                data3[j] = StrToF(data2[j+1]);
            }

            VirtualSensors[index].UpdateData(long.Parse(data2[0]), data3);
        }
    }


    private float StrToF(string value)
    {
        return float.Parse(value, CultureInfo.GetCultureInfo("en-GB"));
    }


    public void AddOffset(int boneIndex, float offset)
    {
        VirtualSensors[boneIndex].AddOffset(offset);
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