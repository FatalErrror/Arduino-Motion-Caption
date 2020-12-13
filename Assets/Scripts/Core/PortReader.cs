using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class PortReader
{
    public string Port = "COM8";
    public int Frequency = 115200;//частота

    public delegate void DataParser(string a);
    public event DataParser ParseData;

    private SerialPort _serialPort;

    private bool _isReading;

    private Thread _thread;
    public PortReader(string port, int frequency)
    {
        Port = port;
        Frequency = frequency;
        ParseData += EmptyDataParser;
    }

    private void EmptyDataParser(string a)
    {

    }


    private void Reading()
    {
        while (_isReading)
        {
            string a = Read();
            if (a != null)
            {
                ParseData(a);
            }
        }
    }

    public string Read()
    {
        if (_serialPort.IsOpen)
        {
            try
            {
                return _serialPort.ReadLine();
            }
            catch (System.Exception)
            {
            }
        }
        return null;
    }

    public void WriteChar(char v)
    {
        if (_serialPort.IsOpen)
        {
            try
            {
                _serialPort.Write(""+v);
            }
            catch (System.Exception)
            {}
        }
    }

    
    public void Begin()
    {
        _serialPort = new SerialPort(Port, Frequency);
        _serialPort.Open();
        _serialPort.ReadTimeout = 50;
        _thread = new Thread(Reading);
        _isReading = true;
        _thread.Start();
        MainUI.Log("PortReader began");
    }

    public void End()
    {
        _isReading = false;
        _serialPort.Close();
    }
}
