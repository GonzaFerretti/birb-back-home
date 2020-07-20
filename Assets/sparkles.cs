using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sparkles : MonoBehaviour
{
    private float time;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (time > 1.5f)
        {
            Destroy(gameObject);
        }
        else
        {
            time += Time.deltaTime;
        }
    }
}
