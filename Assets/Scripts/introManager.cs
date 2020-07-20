using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class introManager : MonoBehaviour
{
    public float bgspeed;
    public float startTimer, startTime;
    public bool hasStartedRolling, hasEndedFlight;
    public float flyTimer, flyTime;
    public soundManager soundM;

    public GameObject pivot, birb, trees, main;
    // Start is called before the first frame update
    private void Start()
    {
        soundM = FindObjectOfType<soundManager>();
    }
    void Update()
    {
        if (startTime > startTimer && !hasEndedFlight)
        {
            trees.transform.position += Vector3.right * bgspeed * Time.deltaTime;
            if (!hasStartedRolling)
            {
                soundM.Play(SFX.soar);
                birb.GetComponent<Animator>().Play("birbFly");
                pivot.GetComponent<Animator>().Play("introPivotAnim");
                hasStartedRolling = true;
            }
        }
        else
        {
            startTime += Time.deltaTime;
        }
        if (hasStartedRolling && !hasEndedFlight)
        {
            if (flyTime < flyTimer)
            {
                flyTime += Time.deltaTime;
            }
            else
            {
                pivot.GetComponent<Animator>().Play("endPivot");
                birb.GetComponent<Animator>().Play("introBirbStop");
                FindObjectOfType<menuHandler>().showStream();
                soundM.stopSFX(SFX.soar);
                hasEndedFlight = true;
            }

        }
        main.GetComponent<Text>().color = (hasEndedFlight) ? new Color(0.972549f, 0.8705882f, 0.5529412f, 1f) : new Color(0.972549f, 0.8705882f, 0.5529412f, 0);
    }

    public bool hasFinishedIntro()
    {
        return hasEndedFlight;
    }
    // Update is called once per frame
}
