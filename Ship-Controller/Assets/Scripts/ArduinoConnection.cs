using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class ArduinoConnection : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        stream = new SerialPort("COM4", 9600);
        stream.ReadTimeout = 50;
        stream.Open();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
