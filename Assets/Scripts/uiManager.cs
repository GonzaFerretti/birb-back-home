using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class uiManager : MonoBehaviour
{
    public Image[] hpIcons;
    public SpriteRenderer[] stamina;
    public Image[] ammunition;
    public Image ammoIcon;
    public Text ammotext;
    public GameObject sparks;
    public Image hpBaseIcon, fade;
    public int maxAmmo;
    public SpriteRenderer staminaIcon;
    public GameObject pause, staminaPosition;
    public Canvas canvas;
    public Image pauseBg;
    public GameObject startingHpPosition, startingAmmoPosition;
    [Range(0,1)]
    public float marginMult, staminaMarginMult, ammoMarginMult;
    public float fadeInTime = 0, fadeOutTime = 0, fadeInTimer, fadeOutTimer;
    public bool willFadeIn = true;
    public float fadeOutState;
    public int currentHP = 0, currentStamina = 0;

    public void Awake()
    {
        fade.gameObject.SetActive(true);
        staminaPosition = GameObject.Find("staminaPivot");
        canvas.worldCamera = GameObject.Find("UI cam").GetComponent<Camera>();
    }

    void fadeIn()
    {
        if (fadeInTime < fadeInTimer)
        {
            fadeInTime += Time.deltaTime;
            float newAlphaValue = Mathf.Clamp(1 - fadeInTime / fadeInTimer,0,1);
            fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, newAlphaValue);
        }
        else
        {
            fadeInTime = 0;
            willFadeIn = false;
        }
    }

    public void triggerFadeOut()
    {
        if (fadeOutState == 0)
        { 
            fadeOutState = 1;
            currentHP = 0;
            modifyHPIcons(0);
            hideAmmo();
        }
    }

    void fadeOut()
    {
        if (fadeOutTime < fadeOutTimer)
        {
            fadeOutTime += Time.deltaTime;
            float newAlphaValue = Mathf.Clamp(fadeOutTime / fadeOutTimer, 0, 1);
            fade.color = new Color(fade.color.r, fade.color.g, fade.color.b, newAlphaValue);
        }
        else
        {
            fadeOutState = 2;
            currentHP = 0;
            modifyHPIcons(0);
            hideAmmo();
        }
    }

    private void Update()
    {
        if (willFadeIn)
        {
            fadeIn();
        }
            if (fadeOutState == 1)
        {
            fadeOut();
        }
    }

    public void hideAmmo()
    {
        foreach (Image ammo in ammunition)
        {
            ammo.color = new Color(1, 1, 1, 0f);
        }
    }
    public void FixedUpdate()
    {
        updateStaminaPosition();
    }
    public void updateStaminaPosition()
    {
        for (int i = 0; i < stamina.Length; i++)
        {
            stamina[i].transform.position = Vector3.zero;
            stamina[i].transform.position = staminaPosition.transform.position + Vector3.right * staminaMarginMult * i * staminaPosition.transform.parent.localScale.x;
            stamina[i].transform.localScale = new Vector3(60f, 60f, 60f);
        }
    }

    public void initializeHP(int startingHP)
    {
        hpIcons = new Image[startingHP];
        for (int i = 0; i < hpIcons.Length; i++)
        {
            hpIcons[i] = Instantiate(hpBaseIcon);
            hpIcons[i].name = "hp" + (i + 1);
            hpIcons[i].rectTransform.SetParent(canvas.transform);
            hpIcons[i].transform.SetSiblingIndex(2 + i);
            hpIcons[i].rectTransform.anchoredPosition = startingHpPosition.GetComponent<RectTransform>().anchoredPosition + Vector2.right * hpIcons[i].preferredWidth * marginMult * i + Vector2.right;
            hpIcons[i].rectTransform.localScale = new Vector3(1f, 1f, 1f);
        }
        hpIcons.OrderBy(hpIcons => hpIcons.rectTransform.anchoredPosition.x).ToArray();
        modifyHPIcons(startingHP);
    }

    public void initializeStamina(int startingSt)
    {
        stamina = new SpriteRenderer[startingSt];
        for (int i = 0; i < stamina.Length; i++)
        {
            stamina[i] = Instantiate(staminaIcon);
            stamina[i].name = "stamina" + (i + 1);
            stamina[i].transform.SetParent(canvas.transform);
        }
        updateStaminaPosition();
        modifyStamina(startingSt);
        hpIcons.OrderBy(stamina => stamina.rectTransform.anchoredPosition.x).ToArray();
    }

    public void initializeAmmo(int maxAmmo)
    {
        ammunition = new Image[maxAmmo];
        for (int i = 0; i < ammunition.Length; i++)
        {
            ammunition[i] = Instantiate(ammoIcon);
            ammunition[i].name = "ammo" + (i + 1);
            ammunition[i].rectTransform.SetParent(ammotext.transform.parent);
            ammunition[i].rectTransform.anchoredPosition = startingAmmoPosition.GetComponent<RectTransform>().anchoredPosition + Vector2.left * ammoMarginMult * i;
        }
        modifyAmmo(0,false, false);
        ammunition.OrderByDescending(ammo => ammo.rectTransform.anchoredPosition.x).ToArray();
    }

    public void modifyHPIcons(int amount)
    {
        currentHP += amount;
        for (int i= 0; i < hpIcons.Length;i++)
        {
            if (i < currentHP)
            {
                hpIcons[i].color = new Color(1, 1, 1, 1);
            }
            else
            {
                hpIcons[i].color = new Color(1, 1, 1, 0);
            }
        }
    }

    public void modifyAmmo(int ammo, bool isAdding, bool isVisual = false)
    {
        if (isAdding)
        {
            GameObject currentSpark = Instantiate(sparks);
            currentSpark.GetComponent<RectTransform>().SetParent(ammotext.transform.parent);
            currentSpark.GetComponent<RectTransform>().anchoredPosition = startingAmmoPosition.GetComponent<RectTransform>().anchoredPosition + Vector2.left * ammoMarginMult * (ammo - 1);
        }
        for (int i = 0; i < ammunition.Length ; i++)
        {
            if (i < ammo)
            {
                ammunition[i].color = new Color(1, 1, 1, 1);
            }
            else
            {
                ammunition[i].color = new Color(1, 1, 1, 0.5f);
            }
        }
        if (!isVisual) ammotext.text = ammo.ToString();
    }

    public void modifyStamina(int amount)
    {
        currentStamina = amount;
        for (int i = 0; i < stamina.Length; i++)
        {
            if (i < currentStamina)
            {
                stamina[i].color = new Color(1, 1, 1, 1);
            }
            else
            {
                stamina[i].color = new Color(1, 1, 1, 0);
            }
        }
    }

    public void visualPause(bool state)
    {
        pauseBg.gameObject.SetActive(state);
        pause.SetActive(state);
    }
}
