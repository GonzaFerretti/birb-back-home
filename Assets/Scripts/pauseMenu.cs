using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class pauseMenu : MonoBehaviour
{
    public Transform[] options;
    public int currentItem = 0;
    public sceneManager sM;
    public soundManager soundM;
    public Font selectedFont, baseFont;
    private void Awake()
    {
        loadMenus();
        soundM = FindObjectOfType<soundManager>();
        updateMenu();
        sM = FindObjectOfType<sceneManager>();
    }

    void loadMenus()
    {
        options = transform.Cast<Transform>().Where(c => c.gameObject.tag == "menuOption").ToArray();
    }

    private void Update()
    {
        checkverticalInput();
        checkActionButton();
    }

    void checkverticalInput()
    {
        if (Input.GetButtonDown("Vertical"))
        {
            soundM.Play(SFX.menuChange);
            currentItem -= (int)Mathf.Sign(Input.GetAxisRaw("Vertical"));
            updateMenu();
        }
    }

    void checkActionButton()
    {
        if (Input.GetButtonDown("AcceptMenu"))
        {
            triggerAction();
        }
    }

    void triggerAction()
    {
        soundM.Play(SFX.menuSelect);
            switch (currentItem)
            {
            case 0:
                FindObjectOfType<gameManager>().PauseSwitch();
                break;
            case 1:
                    sM.currentLevel -= 1;
                    sM.ChangeLevel();
                    break;
            case 2:
                     sM.stageSpecificMenuSwitch(2);
                    break;
            case 3:
                    sM.loadScene(scene.menuScreen);
                    sM.currentLevel = 0;
                    break;
            }
    }

    void updateMenu()
    {
        if (currentItem < 0)
        {
            currentItem = options.Length - 1;
        }
        if (currentItem > options.Length - 1)
        {
            currentItem = 0;
        }
        for (int i = 0; i < options.Length; i++)
        {
            Text currentText = options[i].GetComponent<Text>();
            currentText.font = (i != currentItem) ? baseFont : selectedFont;
            currentText.color = (i != currentItem) ? new Color(0.972549f, 0.8705882f, 0.5529412f, 0.5f) : new Color(0.972549f, 0.8705882f, 0.5529412f, 1f);
        }
    }
}
