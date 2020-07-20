using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class gameManager : MonoBehaviour
{
    private GameObject lastCheckpoint;
    private int livesLeft;
    public int livesTotal;
    private GameObject mainCam, bg;
    private sceneManager sM;
    private soundManager soundM;
    private objectReOrder obre;
    private uiManager uim;
    private birdBody bird;
    public int bugsBeforeCP;
    private int ammo = 0;
    private bool isPaused = false, hasLost = false, hasWon = false, hasStartedFading = false;
    private float mapXo, mapX1, mapYo, mapY1;

    private void Update()
    {
        checkLevelEnd();
    }

    public void setStartingAmmo(int maxAmmo)
    {
        uim.initializeAmmo(maxAmmo);
    }
    private void checkLevelEnd()
    {
        if (hasStartedFading)
        {
            bool hasEndedFade = uim.fadeOutState == 2;
            if (hasEndedFade)
            {
                nextLevel(ammo);
            }
        }
    }

    public float getMapCoordinate(string coordinate)
    {
        float returnCoordinate = 0;
        switch (coordinate)
        {
            case "Xo":
                {
                    returnCoordinate = mapXo;
                    break;
                }
            case "X1":
                {
                    returnCoordinate = mapX1;
                    break;
                }
            case "Yo":
                {
                    returnCoordinate = mapYo;
                    break;
                }
            case "Y1":
                {
                    returnCoordinate = mapY1;
                    break;
                }
            default:
                {
                    break;
                }
        }
        return returnCoordinate;
    }

    public bool getPauseState()
    {
        return isPaused;
    }

    void Awake()
    {
        sM = FindObjectOfType<sceneManager>();
        bird = FindObjectOfType<birdBody>();
        uim = FindObjectOfType<uiManager>();
        bg = GameObject.Find("bg");
        mainCam = GameObject.Find("Main Camera");
        SetLimits();
        soundM = FindObjectOfType<soundManager>();
        obre = FindObjectOfType<objectReOrder>();
    }

    public void startHP(int startingHP)
    {
        uim.initializeHP(startingHP);
        livesTotal = startingHP;
        livesLeft = livesTotal;
            
    }

    

    public void setLives(int amount)
    {
        livesLeft += amount;
        uim.modifyHPIcons(amount);
        checkLose();
    }

    void checkLose()
    {
        if (livesLeft == 0)
        {
            hasLost = true;
            sM.stageSpecificMenuSwitch(5);
        }
    }

    public GameObject getLastCheckpoint()
    {
        GameObject groundCP;
        groundCP = lastCheckpoint;
        return groundCP;
    }

    public void setLastCheckpoint(GameObject CP, int currentBugs)
    {
        lastCheckpoint = CP;
        bugsBeforeCP = currentBugs;
        disableBugsBeforeNewCP();
    }

    public int getLastBugAmount()
    {
        return bugsBeforeCP;
    }
    public void disableBugsBeforeNewCP()
    {
        foreach (GameObject bug in obre.getBugs())
        {
            if (bug.GetComponent<bug>().state == bugState.eatenBeforeCP)
            { bug.GetComponent<bug>().state = bugState.eatenAfterCP; }
        }
    }

    public void setAmmo(int ammo, bool isAdding)
    {
        uim.modifyAmmo(ammo, isAdding);
    }
    public void triggerGoToCP()
    {
        if (livesLeft > 0)
        {
            float lastCPX = lastCheckpoint.transform.GetChild(0).position.x;
            float lastCPY = lastCheckpoint.transform.GetChild(0).position.y;
            resetBugs();
            bird.goToCheckpoint();
            mainCam.transform.position = new Vector3(lastCPX, lastCPY, mainCam.transform.position.z);
        }
    }

    public void SetLimits()
    {
        cameraController cam = mainCam.GetComponent<cameraController>();
        SpriteRenderer bgSr = bg.GetComponent<SpriteRenderer>();
        mapXo = bg.transform.position.x - bgSr.sprite.bounds.extents.x * bg.transform.localScale.x;
        mapX1 = bg.transform.position.x + bgSr.sprite.bounds.extents.x * bg.transform.localScale.x;
        mapYo = bg.transform.position.y - bgSr.sprite.bounds.extents.y;
        mapY1 = bg.transform.position.y + bgSr.sprite.bounds.extents.y;
        cam.bgXo = mapXo;
        cam.bgX1 = mapX1;
        cam.bgYo = mapYo;
        cam.bgY1 = mapY1;
    }

    public void fadeOut(int ammoNow)
    {
        ammo = ammoNow;
        uim.triggerFadeOut();
        hasStartedFading = true;
    }

    void resetBugs()
    {
        foreach (GameObject bug in obre.getBugs())
        {
            if (bug.GetComponent<bug>().state == bugState.eatenBeforeCP)
            { 
            bug.SetActive(true);
            bug.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            }
        }
    }

    public void nextLevel(int bugs)
    {
        sM.ChangeLevel();
        sM.lastLevelBugs = bugs;
    }

    public void PauseSwitch()
    {
        if (!hasWon && !hasLost)
        { 
        isPaused = !isPaused;
        Time.timeScale = (isPaused) ? 0 : 1;
        uim.visualPause(isPaused);
            if (isPaused) soundM.PauseAll(); else soundM.ResumeAll();
        }
    }

    public void ExitToMenu()
    {
        if ((isPaused && hasLost == false))
        {
            sM.currentLevel = -1;
            sM.loadScene(scene.menuScreen);
        } else if (hasWon)
        {
            isPaused = false;
            sM.loadScene(scene.menuScreen);
        }
    }

    public void RestartLevel()
    {
        if (isPaused || hasLost)
        {
            PauseSwitch();
            sM.currentLevel--;
            sM.ChangeLevel();
        }
    }

    public void setStartingSt(int flaps)
    {
        uim.initializeStamina(flaps);
    }

    public void setStamina(int flaps)
    {
        uim.modifyStamina(flaps);
    }
}
