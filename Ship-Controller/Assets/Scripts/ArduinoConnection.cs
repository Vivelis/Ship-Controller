using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;
using System.Threading;

public class ArduinoConnection : MonoBehaviour
{
    SerialPort stream;
    bool isConnected = false;

    // Start is called before the first frame update
    void Awake()
    {
        stream = new SerialPort("COM5", 9600);
        stream.ReadTimeout = 50;
        stream.Open();
    }

    private void Start()
    {
        EstablishConnection();
    }

    private void FixedUpdate()
    {
        if (isConnected)
        {
            StartCoroutine(AsynchronousReadFromArduino((string s) => Debug.Log(s),
                    () => { isConnected = false; Debug.LogError("Error!"); }, 10000f));
        }
    }

    private void EstablishConnection()
    {
        WriteToArduino("PING");
        StartCoroutine(AsynchronousReadFromArduino(
            (string s) => { Debug.Log(s); isConnected = true; },         // Callback
            () => { isConnected = false; Debug.LogError("Error!"); },     // Error callback
            10000f));                           // Timeout (milliseconds)
    }

    public void WriteToArduino(string message)
    {
        stream.WriteLine(message);
        stream.BaseStream.Flush();
    }

    /* brief    if something is send int the stream, exectute callback and exit
     *          else execute fail and exit.
     * 
     * return   null
    */
    public IEnumerator AsynchronousReadFromArduino(Action<string> callback, Action fail = null, float timeout = float.PositiveInfinity)
    {
        DateTime initialTime = DateTime.Now;
        DateTime nowTime;
        TimeSpan diff = default(TimeSpan);
        string dataString = null;

        do
        {
            try
            {
                dataString = stream.ReadLine();
            }
            catch (TimeoutException)
            {
                dataString = null;
            }
            if (dataString != null)
            {
                callback(dataString);
                yield break; // Terminates the Coroutine
            }
            else
            {
                yield return null; // Wait for next frame
            }
            nowTime = DateTime.Now;
            diff = nowTime - initialTime;
        } while (diff.Milliseconds < timeout);
        if (fail != null)
            fail();
        yield return null;
    }
}

public class ArduinoThread : MonoBehaviour
{
    private Queue outputQueue;    // From Unity to Arduino
    private Queue inputQueue;    // From Arduino to Unity
    private Thread thread;
    private bool looping = true;

    public SerialPort stream;
    [SerializeField]
    private string port = "COM5";
    [SerializeField]
    private int baudRate = 9600;

    public void StartThread()
    {
        outputQueue = Queue.Synchronized(new Queue());
        inputQueue = Queue.Synchronized(new Queue());

        thread = new Thread(ThreadLoop);
        thread.Start();
    }

    public void SendToArduino(string command)
    {
        outputQueue.Enqueue(command);
    }

    private string GetFromArduino()
    {
        if (inputQueue.Count != 0)
        {
            string message = (string)inputQueue.Dequeue();
            return message;
        }
        return null;
    }

    private string ReadFromArduino()
    {
        if (inputQueue.Count == 0)
            return null;
        return (string)inputQueue.Dequeue();
    }

    private void WriteToArduino(string message)
    {
        stream.WriteLine(message);
        stream.BaseStream.Flush();
    }

    private void StopThread()
    {
        lock (this)
        {
            looping = false;
        }
    }

    private bool IsLooping()
    {
        lock (this)
        {
            return looping;
        }
    }

    private void ThreadLoop()
    {
        // Opens the connection on the serial port
        stream = new SerialPort(port, baudRate);
        stream.ReadTimeout = 50;
        stream.Open();

        // Looping
        while (IsLooping())
        {
            // Send to Arduino
            if (outputQueue.Count != 0)
            {
                string command = (string)outputQueue.Dequeue();
                WriteToArduino(command);
            }

            // Read from Arduino
            string result = ReadFromArduino();
            if (result != null)
                inputQueue.Enqueue(result);
        }
    }
}
