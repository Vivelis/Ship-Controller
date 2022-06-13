using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayValue : MonoBehaviour
{
    private ArduinoConnection connection;
    private TextMeshProUGUI text;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            connection = GameObject.FindGameObjectWithTag("GameController").GetComponent<ArduinoConnection>();        
        }
        catch
        {
            Debug.LogError("Missing GameManager");
        }
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        string value = "value : null";

        if (connection != null)
        {
            value = connection.GetFromArduino();
            if (value != null)
            {
                text.SetText(value);
            }
        }
    }

    public void OnMove()
    {
        connection.SendToArduino("/rouge");
        Debug.Log("ask for rouge");
    }
}
