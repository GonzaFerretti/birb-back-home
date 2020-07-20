using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class snippetTeleporter : MonoBehaviour
{
    GameObject birb;
    private void Awake()
    {
        birb = GameObject.FindGameObjectWithTag("Player");
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            birb.transform.position = transform.GetChild(0).position;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            birb.transform.position = transform.GetChild(1).position;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            birb.transform.position = transform.GetChild(2).position;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            birb.transform.position = transform.GetChild(3).position;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            birb.transform.position = transform.GetChild(4).position;
        }
    }
}
