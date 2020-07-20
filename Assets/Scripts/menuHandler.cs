using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Audio;
public class menuHandler : MonoBehaviour
{
    public Transform[][] menuList;
    private int[] lastSelectedOption;
    public Vector2[] initialPos;
    private int currentItem = 0;
    private int panId;
    public Sprite sliderThick, sliderThin;
    public Sprite knobThick, knobThin;
    private menuParallax plxmenu;
    private sceneManager sM;
    private soundManager soundM;
    public AudioMixerGroup sfx, bgm;
    private bool bgMenuPan = false;
    private int currentMenu = 0;
    public Font selectedFont, baseFont;
    public float[] streamSize;
    public float[] margin;
    public GameObject stream;
    private difficultyManager dM;
    private void Awake()
    {
        plxmenu = FindObjectOfType<menuParallax>();
        getStartingActiveMenu();
        loadMenus();
        soundM = FindObjectOfType<soundManager>();
        updateMenu();
        sM = FindObjectOfType<sceneManager>();
        dM = FindObjectOfType<difficultyManager>();
    }

    public void checkIfMainMenu()
    {
        plxmenu.checkMainMenu();
    }
    void getStartingActiveMenu()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            currentMenu = (transform.GetChild(i).gameObject.activeSelf) ? i : -1;
            if (currentMenu != -1)
            {
                break;
            }
        }
    }

    public int getCurrentMenuId()
    {
        print(currentMenu);
        return currentMenu;
    }

    public void loadSpecificMenu(int menuId)
    {
        currentMenu = menuId;
        currentItem = 0;
        SwitchMenus(currentMenu);
        if (menuId == 6)
        {
            soundM.Play(BGM.win);
        }
        if (menuId == 5)
        {
            soundM.Play(BGM.lose);
        }
        updateMenu();
    }

    void loadMenus()
    {
        menuList = new Transform[transform.childCount][];
        lastSelectedOption = new int[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            menuList[i] = transform.GetChild(i).Cast<Transform>().Where(c => c.gameObject.tag == "menuOption" || c.gameObject.tag == "menuSlider" || c.gameObject.tag == "menuSelector").ToArray();
            lastSelectedOption[i] = 0;
        }
    }

    void SwitchMenus(int menuIndex)
    {
        if (menuIndex == 3 || menuIndex == 4 || (menuIndex == 0 && currentItem == 0))
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            stageMenuPan();
            panId = menuIndex;
        }
        else
        {
            lastSelectedOption[currentMenu] = currentItem;
            currentMenu = menuIndex;
            currentItem = lastSelectedOption[menuIndex];
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            transform.GetChild(menuIndex).gameObject.SetActive(true);
            updateMenu();
        }
    }

    private void stageMenuPan()
    {
        bgMenuPan = true;
        stream.transform.localScale = Vector3.zero;
    }

    private void checkMenuPan()
    {
        if (!plxmenu.isPanning())
        {
            lastSelectedOption[panId] = currentItem;
            currentMenu = panId;
            currentItem = lastSelectedOption[panId];
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            transform.GetChild(panId).gameObject.SetActive(true);
            updateMenu();
            bgMenuPan = false;
        }
    }

    private void Start()
    {
        soundM.Play(BGM.Menu);
        Time.timeScale = 1;
    }

    private void Update()
    {
        if (bgMenuPan)
        {
            checkMenuPan();
        }
        else
        {
            if (currentMenu != 0)
            {
                checkMenuReturn();
            }
            checkverticalInput();
            checkHorizontalInput();
            checkActionButton();
        }
    }

    void checkMenuReturn()
    {
        if (Input.GetButtonDown("exitMenu"))
        {
            if (currentMenu == 3 || currentMenu == 4) plxmenu.optionsMenuMove();
            SwitchMenus(0);
            currentMenu = 0;
            checkIfMainMenu();
            soundM.Play(SFX.menuSelect);
        }
    }

    void setVolume(float step, bool isBGM)
    {
        float currentVolume = soundM.getVolumeLevels(isBGM);
        float initialVol = currentVolume;
        currentVolume = Mathf.Clamp(currentVolume + step, 0, 1);
        float finalVol = currentVolume;
        if (finalVol != initialVol)
        {
            soundM.Play(SFX.menuChange);
            soundM.updateVolumeLevels(isBGM, finalVol);
        }
    }

    void updateVolumeBars(float step)
    {
        if (currentItem == 0)
        {
            setVolume(step, false);
            updateSliderPos(false);
        }
        else if (currentItem == 1)
        {
            setVolume(step, true);
            updateSliderPos(true);
        }
    }

    void checkHorizontalInput()
    {
        if (menuList[currentMenu][currentItem].tag == "menuSlider")
        {
            if (Input.GetButtonDown("Horizontal"))
            {
                updateVolumeBars(0.1f * Mathf.Sign(Input.GetAxis("Horizontal")));
            }
        }
        if (menuList[currentMenu][currentItem].tag == "menuSelector")
        {
            if (Input.GetButtonDown("Horizontal"))
            {
                switchOption((int)Mathf.Sign(Input.GetAxis("Horizontal")), menuList[currentMenu][currentItem].name);
            }
        }
    }

    void switchOption(int direction, string setting)
    {
        Animator Anim;
        soundM.Play(SFX.menuChange);
        Text text = menuList[currentMenu][currentItem].GetChild(0).GetComponent<Text>();
        if (direction < 0)
        {
            Anim = menuList[currentMenu][currentItem].GetChild(1).GetComponent<Animator>();
            Anim.Play("arrowInteractLeft");
        }
        else
        {
            Anim = menuList[currentMenu][currentItem].GetChild(2).GetComponent<Animator>();
            Anim.Play("arrowInteractRight");
        }
        switch (setting)
        {
            case "difficulty":
                dM.modifyDifficulty(direction);
                updateDifficultyText();
                break;
            case "fullscreen":
                switchFullScreen();
                break;

        }
    }

    void updateDifficultyText()
    {
        menuList[1][3].GetChild(0).GetComponent<Text>().text = dM.getDifficulty().ToString();
    }

    void updateSliderPos(bool isBGM, bool isFirstUpdate = false, int index = 1)
    {
        Transform currentSelection = (isFirstUpdate) ? menuList[1][index] : menuList[currentMenu][currentItem];
        RectTransform currentLine = currentSelection.GetChild(0).GetComponent<RectTransform>();
        RectTransform currentSlider = currentSelection.GetChild(0).GetChild(0).GetComponent<RectTransform>();
        float halfSize = 35;
        float sliderMinPosition = currentLine.rect.min.x + halfSize;
        float sliderMaxPosition = currentLine.rect.max.x - halfSize;
        float sliderCurrentPos = soundM.getVolumeLevels(isBGM);
        sliderCurrentPos = Mathf.Lerp(sliderMinPosition, sliderMaxPosition, sliderCurrentPos);
        currentSlider.anchoredPosition = new Vector2(sliderCurrentPos, currentSlider.anchoredPosition.y);
    }
    void checkverticalInput()
    {
        if (menuList[currentMenu].Length > 1)
        {
            if (Input.GetButtonDown("Vertical"))
            {
                soundM.Play(SFX.menuChange);
                currentItem -= (int)Mathf.Sign(Input.GetAxis("Vertical"));
                updateMenu();
            }
        }
    }

    void checkActionButton()
    {
        if (Input.GetButtonDown("AcceptMenu"))
        {
            triggerAction();
        }
    }

    void mute(bool isBGM)
    {
        float currentVol = soundM.getVolumeLevels(isBGM);
        soundM.updateVolumeLevels(isBGM, (currentVol != 0) ? 0 : 1);
        updateSliderPos(isBGM);
    }

    void switchFullScreen(bool firstLoad = false)
    {
        if (!firstLoad)
        {
            Screen.fullScreen = !Screen.fullScreen;
            menuList[currentMenu][currentItem].GetChild(0).GetComponent<Text>().text = (Screen.fullScreen) ? "Windowed" : "Fullscreen";
        }
        else
        {
            menuList[1][2].GetChild(0).GetComponent<Text>().text = (Screen.fullScreen) ? "Fullscreen" : "Windowed";
        }

    }

    void triggerAction()
    {
        sM.checkFirstIntro();
        switch (currentMenu)
        {
            case 0:
                soundM.Play(SFX.menuSelect);
                switch (currentItem)
                {
                    case 0:
                        SwitchMenus(2);
                        break;
                    case 1:
                        SwitchMenus(1);
                        updateSliderPos(true, true, 1);
                        updateSliderPos(false, true, 0);
                        switchFullScreen(true);
                        updateDifficultyText();
                        break;
                    case 2:
                        SwitchMenus(3);
                        plxmenu.optionsMenuMove();
                        break;
                    case 3:
                        SwitchMenus(4);
                        plxmenu.optionsMenuMove();
                        break;
                    case 4:
                        Application.Quit();
                        break;
                }
                break;
            case 1:
                switch (currentItem)
                {
                    case 0:
                        mute(false);
                        soundM.Play(SFX.menuSelect);
                        break;
                    case 1:
                        mute(true);
                        soundM.Play(SFX.menuSelect);
                        break;
                    case 4:
                        SwitchMenus(0);
                        soundM.Play(SFX.menuSelect);
                        break;
                }
                break;
            case 2:
                soundM.Play(SFX.menuSelect);
                if (currentItem <= 3)
                {
                    sM.currentLevel = currentItem;
                    sM.loadScene(scene.loadingScreen);
                }
                else
                {
                    SwitchMenus(0);
                }
                break;
            case 3:
                switch (currentItem)
                {
                    case 0:
                        SwitchMenus(0);
                        plxmenu.optionsMenuMove();
                        soundM.Play(SFX.menuSelect);
                        break;
                }
                break;
            case 4:
                switch (currentItem)
                {
                    case 0:
                        soundM.Play(SFX.menuSelect);
                        plxmenu.optionsMenuMove();
                        SwitchMenus(0);
                        break;
                }
                break;
            case 5:
            case 6:
                soundM.Play(SFX.menuSelect);
                switch (currentItem)
                {
                    case 0:
                        sM.ChangeLevel();
                        break;
                    case 1:
                        SwitchMenus(0);
                        checkIfMainMenu();
                        soundM.Play(BGM.Menu);
                        break;
                }
                break;
            case 7:
                if (menuList[currentMenu][0].parent.GetComponent<introManager>().hasFinishedIntro())
                {
                    soundM.Play(SFX.menuSelect);
                    switch (currentItem)
                    {
                        case 0:
                            currentMenu = 0;
                            checkIfMainMenu();
                            SwitchMenus(0);
                            break;
                    }
                }
                break;
        }
    }

    public void showStream()
    {
        streamSize[7] = 30;
        updateMenu();
    }

    void updateMenu()
    {
        Transform[] menuToUpdate = menuList[currentMenu];
        if (currentItem < 0)
        {
            currentItem = menuToUpdate.Length - 1;
        }
        if (currentItem > menuToUpdate.Length - 1)
        {
            currentItem = 0;
        }
        for (int i = 0; i < menuToUpdate.Length; i++)
        {
            Text currentText = menuToUpdate[i].GetComponent<Text>();
            currentText.font = (i != currentItem) ? baseFont : selectedFont;
            if (currentMenu != 7)
            {
                currentText.color = (i != currentItem) ? new Color(0.972549f, 0.8705882f, 0.5529412f, 0.5f) : new Color(0.972549f, 0.8705882f, 0.5529412f, 1f);
            }
            if (i == currentItem)
            {
                stream.GetComponent<RectTransform>().anchoredPosition = new Vector2(initialPos[currentMenu].x, initialPos[currentMenu].y + i * margin[currentMenu]);
                stream.transform.localScale = new Vector3(-1, 1, 1) * streamSize[currentMenu];
            }
        }
        if (menuList[currentMenu][0].parent.name == "howTo")
        {
            Animator[] anims = menuList[currentMenu][0].parent.GetComponentsInChildren<Animator>();
            foreach (Animator anim in anims)
            {
                anim.SetBool("start", true);
            }
        }
        if (menuList[currentMenu][0].parent.name == "options")
        {
            for (int i = 0; i < menuToUpdate.Length; i++)
            {
                Transform trans = menuToUpdate[i];
                if (trans.tag == "menuSlider")
                {
                    Image slider = trans.GetChild(0).GetComponent<Image>();
                    slider.sprite = (i != currentItem) ? sliderThin : sliderThick;
                    slider.color = (i != currentItem) ? new Color(0.972549f, 0.8705882f, 0.5529412f, 0.5f) : new Color(0.972549f, 0.8705882f, 0.5529412f, 1f);
                    slider.SetNativeSize();
                    Image knob = trans.GetChild(0).GetChild(0).GetComponent<Image>();
                    knob.sprite = (i != currentItem) ? knobThin : knobThick;
                    knob.color = (i != currentItem) ? new Color(0.972549f, 0.8705882f, 0.5529412f, 0.5f) : new Color(0.972549f, 0.8705882f, 0.5529412f, 1f);
                    knob.SetNativeSize();
                }
                else if (trans.tag == "menuSelector")
                {
                    for (int j = 0; j < trans.childCount; j++)
                    {
                        trans.GetChild(j).GetComponent<Text>().font = (i != currentItem) ? baseFont : selectedFont;
                        trans.GetChild(j).GetComponent<Text>().color = (i != currentItem) ? new Color(0.972549f, 0.8705882f, 0.5529412f, 0.5f) : new Color(0.972549f, 0.8705882f, 0.5529412f, 1f);
                    }
                }
            }
        }
    }
}
