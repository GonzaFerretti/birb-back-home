using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class featherFall : MonoBehaviour
{
    public float escalar; 
    private void Update()
    {
        transform.position = new Vector3(-Time.frameCount, Mathf.Pow((-Time.frameCount* escalar), 3f), 0); 
    }
}
