using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class birdBrain : MonoBehaviour
{
    public birdBody body;
    public gameManager gm;

    private void Start()
    {
        body = GetComponent<birdBody>();
        gm = FindObjectOfType<gameManager>();
    }

    private void Update()
    {
        checkPause();
        if (!gm.getPauseState())
        {
            checkJump();
            checkHorizontalMovement();
            checkDash();
            checkAttack();
            checkIdle();
            checkWindMovement();
            checkJumpEnd();
            if (SceneManager.GetActiveScene().name != "tutorial")
            { 
            checkFFW();
            }
        }
    }

    void checkIdle()
    {
        if (Input.anyKey)
        {
            body.setMovingState(false);
            if (Input.GetAxis("Horizontal") != 0 || Input.GetButtonDown("Jump") || Input.GetButtonDown("Dash"))
            {
                body.setMovingState(true);
            }            
        }
        else
        {
            body.setMovingState(false);
        }
    }

    void checkAttack()
    {
        if (Input.GetButtonDown("Spit"))
        {
            body.Attack();
        }
    }
    void checkDash()
    {
        if (Input.GetButtonDown("Dash"))
        {
            body.Dash();
        }
    }

    void checkJump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            body.handleJumpPress();
        }
    }

    void checkJumpEnd()
    {
        if (Input.GetButtonUp("Jump"))
        {
            body.stopJump();
        }
    }

    void checkWindMovement()
    {
        if (Input.GetAxis("Vertical") != 0)
        {
            float inputValue = Input.GetAxis("Vertical");
            body.handleWindMovement(inputValue);
        }
    }

    void checkHorizontalMovement()
    {
        if (Input.GetAxis("Horizontal") != 0)
        {
            float inputValue = Input.GetAxis("Horizontal");
            body.moveHorizontal(inputValue);
        }
        if (Input.GetAxis("Horizontal") == 0)
        {
            body.stopMotionOnGround();
        }
    }

    void checkPause()
    {
        if (Input.GetButtonDown("exitMenu"))
        {
            gm.PauseSwitch();
        }
    }
    
    void checkFFW()
    {
        /*
        if (Input.GetKeyDown(KeyCode.P))
        {
            FindObjectOfType<sceneManager>().ChangeLevel();
        }*/
    }
}
