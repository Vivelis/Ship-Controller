using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private GameObject cameraPos;

    void Start()
    {
        if (cameraPos == null)
            cameraPos = GameObject.FindGameObjectWithTag("CameraPos");
    }

    void LastUpdate()
    {
        transform.position = cameraPos.transform.position;
    }
}
