using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum panState
{
    mainMenu,
    isPanning,
    isPanned,
}

public class menuParallax : MonoBehaviour
{
    public RectTransform birb, trees, bg, panPivot;
    [Header("Options Menu Slide")]
    public float SlideSpeed;
    public panState currentPanState = panState.mainMenu;
    public float panDirectionMult;
    public float panEndX, panStartX;
    public float panSnapDistance;
    [Header("Birb Animations")]
    public Animator birbAnim;
    public float peckTimer, peckTime, minCD, maxCD;
    private menuHandler menuHandler;
    [Header("Birb PLX")]
    public float currentEnd;
    public float startingX, finishX, speed, timerOnBorders, timeOnBorders, directionMult = 1, snapDistance;
    public bool hasReachedBorder;
    [Header("Tree PLX")]
    public float treeCurrentEnd;
    public float treeStartingX, treeFinishX;

    private void Start()
    {
        menuHandler = FindObjectOfType<menuHandler>();
        currentEnd = finishX;
        treeCurrentEnd = treeStartingX;
        peckTimer = Random.Range(minCD, maxCD);
        checkMainMenu();
    }

    public void optionsMenuMove()
    {
        panDirectionMult = (currentPanState == panState.mainMenu) ? -1 : 1;
        currentPanState = panState.isPanning;
    }

    public bool isPanning()
    {
        bool returnValue = false;
        return returnValue = (currentPanState == panState.isPanning);
    }

    void panObjects()
    {
        float pivotObjetive = (panDirectionMult == -1) ? panEndX : panStartX;
        bool objectiveReached = false;
        if (Mathf.Abs(panPivot.anchoredPosition.x - pivotObjetive) > panSnapDistance)
        {
            panPivot.anchoredPosition += Vector2.right * Time.deltaTime * SlideSpeed * panDirectionMult;
        }
        else
        {
            panPivot.anchoredPosition = new Vector2(pivotObjetive, panPivot.anchoredPosition.y);
            objectiveReached = true;
        }
        if (objectiveReached)
        {
            currentPanState = (panDirectionMult == -1) ? panState.isPanned : panState.mainMenu;
        }
    }

    private void Update()
    {
        if (currentPanState == panState.isPanning)
        {
            panObjects();
        }
        else if (currentPanState == panState.mainMenu)
        {
            applyParallax();
        }
        checkPeck();
    }

    public void checkMainMenu()
    {
        bool currentlyOnMainMenu = menuHandler.getCurrentMenuId() < 5;
        birb.gameObject.SetActive(currentlyOnMainMenu);
        trees.gameObject.SetActive(currentlyOnMainMenu);
        bg.gameObject.SetActive(currentlyOnMainMenu);
    }

    void checkPeck()
    {
        if (peckTime < peckTimer)
        {
            peckTime += Time.deltaTime;
        }
        else
        {
            string AnimToPlay = "birbMenu" + Random.Range(1, 4);
            birbAnim.Play(AnimToPlay);
            peckTime = 0;
            peckTimer = Random.Range(minCD, maxCD);
        }
    }
    void applyParallax()
    {
        if (!hasReachedBorder)
        {
            Vector2 layerPosition = birb.anchoredPosition;
            float distanceToEnd = Mathf.Abs(layerPosition.x - currentEnd);
            if (distanceToEnd < snapDistance)
            {
                trees.anchoredPosition = new Vector2(treeCurrentEnd, trees.anchoredPosition.y);
                birb.anchoredPosition = new Vector2(currentEnd, birb.anchoredPosition.y);
                hasReachedBorder = true;
                directionMult *= -1;
                currentEnd = (directionMult == -1) ? startingX : finishX;
                treeCurrentEnd = (directionMult == 1) ? treeStartingX : treeFinishX;
            }
            else
            {
                float currentStart = (directionMult == -1) ? finishX : startingX;
                float normalizedUnit = 1 - (distanceToEnd / Mathf.Abs(currentEnd - currentStart));
                float nonLinearMult = 1 / (10 * (normalizedUnit + 0.1f));
                float distanceToAdd = directionMult * speed * Time.deltaTime * (nonLinearMult);
                birb.anchoredPosition += (Vector2)birb.right * distanceToAdd;
                trees.anchoredPosition -= (Vector2)birb.right * distanceToAdd;
            }
        }
        else
        {
            if (timeOnBorders < timerOnBorders)
            {
                timeOnBorders += Time.deltaTime;

            }
            else
            {
                hasReachedBorder = false;
                timeOnBorders = 0;
            }
        }
    }
}
