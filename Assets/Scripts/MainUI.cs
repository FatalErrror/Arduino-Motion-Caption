using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    public VirtualSkeleton MainVirtualSkeleton;

    public Text LogField;

    public Text Content;

    public static MainUI Self;

    delegate void f(string message);
    private static f  _log, _addToPrevLog, _logerror;

    /*[Header("Debug and test area")]
    V4AverageFilter Testy = new V4AverageFilter(120);
    public Vector4 InputV4;
    public Vector4 OutputV4;

    private void Update()
    {
        Testy.NewValue(InputV4);
        OutputV4 = Testy.GetAverageValue();
    }*/


    // Start is called before the first frame update
    private void Awake()
    {
        _log = Log1;
        _addToPrevLog = AddToPrevLog1;
        _logerror = LogError1;
        Self = this;
    }

    public static void Log(string message)
    {
        Debug.Log(message);
        _log(message);
    }

    public static void AddToPrevLog(string message)
    {
        Debug.Log(message);
        _addToPrevLog(message);
    }

    public static void LogError(string message)
    {
        Debug.Log(message);
        _logerror(message);
    }


    private void Log1(string message)
    {
        Content.text += "\n" + message;
    }

    public  void AddToPrevLog1(string message)
    {
        Debug.Log(message);
        Content.text += message;
    }

    private void LogError1(string message)
    {
        Content.text += "\n" + message;
        //log.color = Color.red;
    }

    public void ClearLogboard()
    {
        Content.text = "Logs: ";
    }


    public void UseAutoCalibrate(bool value)
    {
        var a = GameObject.FindObjectsOfType<AutoCalibrateArm>();
        foreach (var item in a)
        {
            item.use = value;
        }
    }


    public void Restart()
    {
        MainVirtualSkeleton.StopPortReader();
        MainVirtualSkeleton.StartPortReader();
    }


    public void UpdateLogField(string text)
    {
        LogField.text = text;
    }




    public void Back()
    {
        MainVirtualSkeleton.StopPortReader();
        MainVirtualSkeleton.StopUseDynamicOffsets();
        SceneManager.LoadScene(0);
    }

    public void Calibrate()
    {
        MainVirtualSkeleton.Calibrate();
    }

    public void StartSettingDynamicOffsets()
    {
        MainVirtualSkeleton.StartSettingDynamicOffsets();
    }

    public void StopUseDynamicOffsets()
    {
        MainVirtualSkeleton.StopUseDynamicOffsets();
    }


    public void IsUseAverageFilter(bool val)
    {
        MainVirtualSkeleton.IsUseAverageFilter = val;
    }

}
