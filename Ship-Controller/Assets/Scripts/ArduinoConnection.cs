using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;
using System.Threading;

public class ArduinoConnection : MonoBehaviour
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
    [SerializeField]
    private int timeout = 1000;

    private void Awake()
    {
        StartThread();
    }

    private void Start()
    {
        SendToArduino("PING");
    }

    private void Update()
    {
        string message = GetFromArduino();
        if (message != null)
            Debug.Log(message);
    }

    private void OnApplicationQuit()
    {
        StopThread();
    }

    // Self methodes

    public void SendToArduino(string command)
    {
        outputQueue.Enqueue(command);
    }

    public string GetFromArduino()
    {
        if (inputQueue.Count == 0)
            return null;
        return (string)inputQueue.Dequeue();
    }

    private string ReadFromArduino()
    {
        try
        {
            return stream.ReadLine();
        }
        catch (TimeoutException e)
        {
            return null;
        }
    }

    private void WriteToArduino(string message)
    {
        stream.WriteLine(message);
        stream.BaseStream.Flush();
    }

    public void StartThread()
    {
        outputQueue = Queue.Synchronized(new Queue());
        inputQueue = Queue.Synchronized(new Queue());

        thread = new Thread(ThreadLoop);
        thread.Start();
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
        stream.ReadTimeout = timeout;
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
