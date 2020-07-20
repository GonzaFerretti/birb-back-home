using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;

public class sceneManager : MonoBehaviour
{
    public int lastLevelBugs = 0;
    public int currentLevel;
    private bool hasPlayedFirstIntro = false;
    private bool isLoadingSpecificMenu = false;
    private int specificMenuId;
    private float lastCursorPosition;
    public int cursorHiderTimer;
    private bool cursorIdle;
    private float cursorHideTime;
    private soundManager soundM;
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void checkFirstIntro()
    {
        hasPlayedFirstIntro = true;
    }

    void checkCursorHider()
    {
        if (cursorIdle)
        {
            cursorIdle = false;
            lastCursorPosition = Input.GetAxis("Mouse X");
        }
        if (Input.GetAxis("Mouse X") == lastCursorPosition)
        {

            cursorHideTime -= Time.deltaTime;
            if (cursorHideTime < 0)
            {
                cursorHideTime = cursorHiderTimer;
                Cursor.visible = false;
                cursorIdle = true;
            }
        }
        else
        {
            cursorHideTime = cursorHiderTimer;
            Cursor.visible = true;
        }
    }

    private void Awake()
    {
        if (FindObjectsOfType<sceneManager>().Length > 1)
        {
            Destroy(gameObject);
        }
        soundM = GetComponent<soundManager>();
    }

    public void loadScene(scene scene)
    {
        Time.timeScale = 1;
        if ((int)scene == 0 && hasPlayedFirstIntro)
        {
            hasPlayedFirstIntro = false;
            int id = (specificMenuId == -1) ? 0 : specificMenuId;
            stageSpecificMenuSwitch(id);
        }
        SceneManager.LoadScene((int)scene);
        soundM.stopAllSfx();
    }

    private void Update()
    {
        checkCursorHider();
        if (Input.GetButtonDown("Screenshot"))
        {
            TakeScreenShot();
        }
        if (isLoadingSpecificMenu)
        {
            if (FindObjectOfType<menuHandler>() != null)
            {
                changeToSpecificMenu(specificMenuId);
            }
        }
    }

    void changeToSpecificMenu(int menuId)
    {
        menuHandler menu = FindObjectOfType<menuHandler>();
        menu.loadSpecificMenu(menuId);
        isLoadingSpecificMenu = false;
        menu.checkIfMainMenu();
        specificMenuId = -1;
    }

    void TakeScreenShot()
    {
        ScreenCapture.CaptureScreenshot(DateTime.Now.ToString("MM-dd-yyyy HH-mm-ss") + ".png");
    }

    public void stageSpecificMenuSwitch(int menuId)
    {
        specificMenuId = menuId;
        isLoadingSpecificMenu = true;
        currentLevel = 0;
        lastLevelBugs = 0;
        loadScene(scene.menuScreen);
    }
    
    public void ChangeLevel()
    {
        Time.timeScale = 1;
        currentLevel = currentLevel + 1;
        if (currentLevel >= 0 && currentLevel <= 4)
        {
            SceneManager.LoadScene(currentLevel);
            soundM.stopAllSfx();
            soundM.Play((BGM)currentLevel);
        }
        else
        {
            stageSpecificMenuSwitch(6);
        }
    }
}



public enum scene
{
    menuScreen = 0,
    firstLevel = 1,
    secondLevel = 2,
    thirdLevel = 3,
    boss = 4,
    loadingScreen = 5,
    tutorial = 6,
}
