using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class loadAnim : MonoBehaviour
{
    public float changeTime;
    public float changeTimer;
    private Text text;
    private int amountOfPoints = 0;
    public bool hasReached = false;

    private void Start()
    {
        text = GetComponent<Text>();
    }
    // Update is called once per frame
    void Update()
    {
        changeTime += Time.deltaTime;
        if (changeTime> changeTimer && !hasReached)
        {
            changeTime = 0;
            amountOfPoints++;
            if (amountOfPoints > 3 )
            {
                amountOfPoints = 0;
            }
            text.text = "Loading" + new string('.',amountOfPoints);
        }
        if (hasReached)
        {
            text.text = "Loading";
        }
    }
}
