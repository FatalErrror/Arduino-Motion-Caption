﻿using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class VirtualSkeleton : MonoBehaviour
{
    public Skeleton Skeleton;
    public GameObject Container;
    public Hand Hand;

    public bool[] IsContole;
    public int[] NumberMap = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };

    public float SettingDynamicOffestsTime = 120, UseDinamicOffsetsDeltaTime = 5;

    public GameObject Warrning;

    private string _data = "Started";
    private Queue<string> _logs = new Queue<string>();


    public float[] _startRotation, _offsets, _dynamicOffsets;


    private PortReader _portReader;

    void Start()
    {
        for (int i = 0; i < (int)Skeleton.Indexs.Lenght; i++)
        {
            IsContole[i] = PlayerPrefs.GetInt("IsContoleable" + i, 0) > 0;
            NumberMap[i] = PlayerPrefs.GetInt("NumberMap" + i, i);
        }

        Transform[] transforms = Skeleton.GetBonesArray();
        _startRotation = new float[transforms.Length];
        _offsets = new float[transforms.Length];
        _dynamicOffsets = new float[transforms.Length];


        for (int i = 0; i < transforms.Length; i++)
        {
            Transform item = transforms[i];
            Transform container = Instantiate(Container, item).transform;
            container.parent = item.parent;
            item.parent = container;
            _startRotation[i] = item.rotation.eulerAngles.y;
            if (IsContole.Length > i)
            {
                if (IsContole[i]) container.parent = Skeleton.GetRoot();
            }
        }

        StartPortReader();
    }

    public void StartPortReader()
    {
        _portReader = new PortReader(PlayerPrefs.GetString("Port", "COM11"), PlayerPrefs.GetInt("Freqancy", 115200));
        _portReader.ParseData += DataParserForLog;


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
        _portReader.ParseData -= DataParserForLog;
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
        UpdateBonesAngles();
        MainUI.Self.UpdateLogField(_data);
        WriteLogs();
    }

    public void Calibrate()
    {
        Invoke(nameof(CalibrateImmediately), 3);
    }

    public void CalibrateImmediately()
    {
        Transform[] transforms = Skeleton.GetBonesArray();
        for (int i = 0; i < transforms.Length; i++)
        {
            _offsets[i] = _startRotation[i] - (transforms[i].rotation.eulerAngles.y - _offsets[i]);
        }
    }


    public void StartSettingDynamicOffsets()
    {
        if (!Warrning.active)
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
            Warrning.GetComponent<UnityEngine.UI.Text>().text = "DONT MOVE OR TOUCH SENSORS " + (SettingDynamicOffestsTime-i) + " sec";
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



    private void UpdateBonesAngles()//data example: |0=10.2_-12.5_102.0_102.0|1=10.2_-12.5_102.0_102.0|HF
    {
        
        string a = _portReader.GetData();
        if (a != null) _data = a;
        _portReader.UpdateData();


        string[] datas = _data.Split('|');
        for (int i = 0; i < datas.Length; i++)
        {
            string[] data1 = datas[i].Split('=');
            if (data1.Length >= 2)
            {
                int index = NumberMap[int.Parse(data1[0])];
                string[] data2 = data1[1].Split('_');
                Vector4 Data = new Vector4(
                    float.Parse(data2[0], CultureInfo.GetCultureInfo("en-GB")),// .Replace('.', ',')
                    float.Parse(data2[1], CultureInfo.GetCultureInfo("en-GB")),// .Replace('.', ',')
                    float.Parse(data2[2], CultureInfo.GetCultureInfo("en-GB")),// .Replace('.', ',')
                    float.Parse(data2[3], CultureInfo.GetCultureInfo("en-GB")) // .Replace('.', ',')
                    );
                Quaternion q = new Quaternion(Data.x, Data.y, -Data.z, Data.w);

                Transform item = Skeleton.GetBonesArray()[index];
                item.localRotation = q;
                item.Rotate(0, _offsets[index], 0, Space.World);

            }
            else
            {
                if (data1[0].Length > 0)
                    if (data1[0][0] == 'H')
                    {
                        Hand.DoHand(data1[0][1] == 'T');
                    }
            }
        }
    }

    
    public void DataParserForLog(string data)
    {
        if (data[0] != '|')
            _logs.Enqueue(data);
    }

    public void WriteLogs()
    {
        for (int i = 0; i < _logs.Count; i++)
        {
            var m = _logs.Dequeue();
            if (m[0] == '>' || m[0] == '.' || m[0] == '*') MainUI.AddToPrevLog(m);
            else MainUI.Log(m);
        }
    }


    public void AddOffset(int boneIndex, float offset)
    {
        _offsets[boneIndex] += offset;
    }

}