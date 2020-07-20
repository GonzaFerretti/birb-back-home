using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public enum dashState
{
    cantDash = -1,
    canDash = 0,
    isDashing = 1,
}
public enum windState
{
    cantWind = -1,
    canWind = 0,
    onWind = 1,
}
public class birdBody : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer sr;
    public cameraController camCont;
    private gameManager gm;
    private difficultyManager dM;
    private soundManager soundM;
    private bool canMove = false;
    public int startingLives;
    private bool hasReachedEnd;
    private bool hasSlept = false;
    [Range(0,1)]
    public float finalStopSpeed;

    [Header("Animation")]
    public AnimatorOverrideController animOv;
    public float shootAnimTime;
    public float shootAnimTimer;
    public bool hasOpenBeak;
    private Animator anim;
    private RuntimeAnimatorController baseAnim;
    private RuntimeAnimatorController nextAnim;

    [Header("Spit")]
    public int ammo;
    private int maxAmmo;
    public GameObject projectile;
    public GameObject mouth;

    [Header("Dash")]
    public float dashStrength;
    private GameObject lastbug;
    public bool canControl = true;
    public float dashTime, dashTimer;
    private dashState currentDState = dashState.cantDash;
    [Range(0,1)]
    public float slowmoAmount;

    [Header("Flap")]
    public float flapImpulse;
    public float maxFlapForce;
    public int remainingFlaps;
    public int totalFlaps;
    private GameObject stPosition;

    [Header("Maximums")]
    public float maxspeedX;
    public float maxspeedY;
    private float minMapX, minMapY, maxMapX, maxMapY;

    [Header("Jump")]
    public float jumpForce;
    public float fallSpeed;
    public bool hasStartedJumping = false;
    public float contJumpForce;
    public float jumpStartTime = 0;
    public float jumpMaxTime;

    [Header("Horizontal Movement")]
    public float horSpeed;
    public float flightHorSpeed;
    public float groundFriction;
    public float groundCollisionTime, groundCollisionTimer;
    public float lastcollisionExit, currentCollisionEnter;
    public bool isStepping;

    [Header("States")]
    private bool onAir = false;
    private bool isActivelyMoving = false, canCollideWithGround = true;

    [Header("Wind")]
    private windState currentWState = windState.cantWind;
    private float windCurrentTime;
    public float windTime;
    public float windBoost;
    public float velocityMovementCap;
    public float windControl;

    private void checkAdditionalJumpForce()
    {
        if (hasStartedJumping)
        {
            if (Time.fixedTime - jumpStartTime < jumpMaxTime)
            {
                rb.AddForce(new Vector2(0, contJumpForce * Time.deltaTime), ForceMode2D.Force);
            }
            else
            {
                stopJump();
            }
        }
    }

    private void Awake()
    {
        dM = FindObjectOfType<difficultyManager>();
        applyDifficulty();
    }

    void applyDifficulty()
    {
        float[] settings = dM.getDifficultySettings(difficultyObject.birb);
        totalFlaps = (int)settings[0];
        maxAmmo = (int)settings[1];
        startingLives = (int)settings[2];
    }

    public void stopJump()
    {
        hasStartedJumping = false;
    }

    public void setMovingState(bool isMoving)
    {
        isActivelyMoving = isMoving;
    }
    public void checkAttackAnimTimer()
    {
        if (hasOpenBeak)
        {
            shootAnimTime += Time.deltaTime;
            if (shootAnimTime > shootAnimTimer)
            {
                shootAnimTime = 0;
                hasOpenBeak = false;
                anim.runtimeAnimatorController = nextAnim;
            }
        }
    }

    public void checkCollisionTimer()
    {
        if (!canCollideWithGround)
        {
            groundCollisionTime += Time.deltaTime;
            if (groundCollisionTime > groundCollisionTimer)
            {
                groundCollisionTime = 0;
                canCollideWithGround = true;
            }
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        groundContact(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        stayOnGround(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        windContact(collision);
        checkOnFinalZone(collision);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        leaveGroundContact(collision);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        enterDashArea(collision);
        if (collision.tag == "Wind" && currentDState != dashState.isDashing && canControl)
        {
            soundM.Play(SFX.soarOut);
            soundM.Play(SFX.soar);
            soundM.Play(SFX.soarIn);
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }
        checkLevelFinish(collision);
    }

    void checkWakeStatus()
    {
        if (!canMove)
        {
            canMove = anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.75 && !anim.IsInTransition(0);
        }
        else
        {
            anim.SetBool("canMove", true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Wind")
        {
            currentWState = windState.canWind;
            anim.SetBool("isBoosting", false);
            soundM.stopSFX(SFX.soar);
            soundM.stopSFX(SFX.soarIn);
            soundM.Play(SFX.soarOut);
        }
        exitDashArea(collision);
        if (collision.tag == "Bug" && currentDState == dashState.isDashing)
        {
            collision.gameObject.GetComponentInParent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            sr.color = Color.white;
        }
    }

    private void Start()
    {
        soundM = FindObjectOfType<soundManager>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        remainingFlaps = totalFlaps;
        gm = FindObjectOfType<gameManager>();
        maxMapX = gm.getMapCoordinate("X1");
        minMapX = gm.getMapCoordinate("Xo");
        maxMapY = gm.getMapCoordinate("Y1");
        minMapY = gm.getMapCoordinate("Yo");
        baseAnim = anim.runtimeAnimatorController;
        gm.startHP(startingLives);
        gm.setStartingSt(totalFlaps);
        gm.setStartingAmmo(maxAmmo);
        ammo = FindObjectOfType<sceneManager>().lastLevelBugs;
        gm.setAmmo(ammo, false);
    }

    private void Update()
    {
        checkWakeStatus();
        if (canMove)
        {
            
            checkAnimBool();
            checkLevelFall();
            ClampSpeed();
            checkAttackAnimTimer();
        }
    }

    private void FixedUpdate()
    {
        if (canMove)
        { 
        checkControl();
        dashSlowMo();
        stopMotionOnGround();
        checkCollisionTimer();
        checkAirState();
        checkSleepEnd();
        checkAdditionalJumpForce();
        }
    }

    private void windContact(Collider2D collision)
    {
        if (collision.gameObject.tag == "Wind" && onAir && currentDState != dashState.isDashing && canControl)
        {
            wind closeWind = collision.gameObject.GetComponent<wind>();
            Vector2 windDirection = closeWind.getDirection();
            float windStrength = closeWind.getStrength();
            windImpulse(windDirection, windStrength);
            currentWState = windState.onWind;
            if (windDirection.x < 0)
            {
                anim.Play("birbHit");
            }
            if (soundM.GetCurrentSFX().name == "windIn")
            {
                soundM.stopSFX(SFX.soarIn);
                soundM.Play(SFX.soar);
            }
        }
    }

    private void leaveGroundContact(Collision2D collision)
    {
        if ((collision.collider.tag == "Ground" || collision.collider.tag == "branch") && canCollideWithGround)
        {
            onAir = true;
            anim.SetBool("onAir", true);
            lastcollisionExit = Time.fixedTime;
        }
    }

    private void groundContact(Collision2D collision)
    {
        if ((collision.collider.tag == "Ground" || collision.collider.tag == "branch"))
        {
            Vector2 collisionVector = (Vector2)transform.position - collision.contacts[0].point;
            float collisionAngle = Vector2.SignedAngle(collision.otherCollider.transform.right, collisionVector);
            float collisionArc = collision.gameObject.GetComponent<PlatformEffector2D>().surfaceArc;
            if (collisionAngle >= 180 - collisionArc && collisionAngle <= collisionArc)
            {
                soundM.Play(SFX.move);
                rb.velocity = new Vector2(0, rb.velocity.y);
                onAir = false;
                remainingFlaps = totalFlaps;
                anim.SetBool("onAir", false);
                anim.ResetTrigger("takeOff");
                if (collision.transform.Find("respawn").gameObject.activeSelf)
                {
                setCheckpoint(collision.gameObject);
                }
                gm.setStamina(totalFlaps);
                currentCollisionEnter = Time.fixedTime;
                canCollideWithGround = false;
                hasStartedJumping = false;
            }
        }
    }

    private void stayOnGround(Collision2D collision)
    {
        if ((collision.collider.tag == "Ground" || collision.collider.tag == "branch"))
        {
            groundCollisionTime = 0;
            canCollideWithGround = false;
        }
    }

    public void handleJumpPress()
    {
        if (canMove && !hasReachedEnd)
        { 
        if (!onAir)
        {
            Jump();
            anim.SetTrigger("takeOff");
            canCollideWithGround = true;
            anim.SetBool("onAir", true);
            onAir = true;
            canCollideWithGround = true;
            hasStartedJumping = true;
            jumpStartTime = Time.fixedTime;
        }
        else
        {
            if (remainingFlaps > 0)
            {
                Flap();
            }
        }
        }
    }

    public void ClampSpeed()
    {
        if (Mathf.Abs(rb.velocity.x) > maxspeedX || Mathf.Abs(rb.velocity.y) > maxspeedY)
        {
            float velX = rb.velocity.x;
            float velY = rb.velocity.y;
            rb.velocity = new Vector2(Mathf.Clamp(velX, -maxspeedX, maxspeedX), Mathf.Clamp(velY, -maxspeedY, maxspeedY));
        }
    }

    void checkOnFinalZone(Collider2D collision)
    {
        if (collision.tag == "FinishSlow")
        {
            rb.velocity = new Vector2(rb.velocity.x * Time.deltaTime * finalStopSpeed, rb.velocity.y);
        }
    }

    void Flap()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        remainingFlaps--;
        rb.AddForce(new Vector2(0, flapImpulse), ForceMode2D.Impulse);
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, 0, maxFlapForce));
        anim.SetTrigger("Flap");
        gm.setStamina(remainingFlaps);
        soundM.Play(SFX.flap);
    }

    public void Attack()
    {
        if (!hasOpenBeak && ammo > 0 && canMove && !hasReachedEnd)
        {
            nextAnim = new AnimatorOverrideController(anim.runtimeAnimatorController);
            anim.runtimeAnimatorController = animOv;
            hasOpenBeak = true;
            GameObject currentProjectile;
            ammo = Mathf.Clamp(ammo-1, 0, maxAmmo);
            currentProjectile = Instantiate(projectile, mouth.transform.position, mouth.transform.rotation);
            currentProjectile.GetComponent<projectile>().speed *= Mathf.Sign(transform.localScale.x);
            gm.setAmmo(ammo, false);
            soundM.Play(SFX.spit);
        }
    }
    
    void enterDashArea(Collider2D collision)
    {
        if (collision.tag == "BugV")
        {
            float angle = Vector2.Angle((Vector2)transform.position, (Vector2)collision.transform.position);
            currentDState = dashState.canDash;
            lastbug = collision.gameObject;
        }
    }

    void exitDashArea(Collider2D collision)
    {
        if (collision.tag == "BugV")
        {
            if (currentDState == dashState.isDashing)
            {
                soundM.Play(SFX.gulp);
                lastbug.gameObject.SetActive(false);
                lastbug.GetComponent<bug>().state = bugState.eatenBeforeCP;
            }
            Time.timeScale = 1;
            canControl = false;
            currentDState = dashState.cantDash;
            anim.SetBool("isBoosting", false);
        }
    }

    void Jump()
    {
        rb.AddForce(new Vector2(0, jumpForce));
        soundM.Play(SFX.jump);
    }

    void checkControl()
    {
        if (!canControl)
        {
            if (dashTime < dashTimer)
            {
                dashTime += Time.deltaTime;
            }
            else
            {
                dashTime = 0;
                canControl = true;
            }
        }
    }

    public void moveHorizontal(float inputForce)
    {
        if (canMove && !hasReachedEnd)
        {
        if (currentDState != dashState.isDashing && currentWState != windState.onWind && !onAir)
        {
            rb.velocity = new Vector2(inputForce * horSpeed, rb.velocity.y);
            setFacingDirection(inputForce);
            if (isStepping && !soundM.isPlayingSFX(SFX.move))
            { 
                soundM.Play(SFX.move);
                isStepping = false;
            }
        }
        else if (onAir)
        {
            if (Mathf.Sign(rb.velocity.x) != Mathf.Sign(inputForce))
            {
                if (canControl)
                {
                    rb.velocity = new Vector2(0, rb.velocity.y);
                }
            }
            if (Mathf.Abs(rb.velocity.x) < velocityMovementCap)
            {
                rb.velocity += new Vector2(inputForce * flightHorSpeed * Time.deltaTime, 0);
                setFacingDirection(inputForce);
            }
        }
        }
    }

    public void dashSlowMo()
    {
        if (currentDState == dashState.canDash && !gm.getPauseState())
        {
            Time.timeScale = slowmoAmount;
        }
    }

    public void Dash()
    {
        if (currentDState == dashState.canDash && !gm.getPauseState())
        {
            Time.timeScale = 1;
            Vector2 direction;
            direction = ((Vector2)lastbug.transform.position - (Vector2)transform.position).normalized;
            rb.velocity = direction * dashStrength;
            ammo= Mathf.Clamp(ammo+ 1,0,maxAmmo);
            gm.setAmmo(ammo, true);
            currentDState = dashState.isDashing;
            setFacingDirection(direction.x);
            anim.SetBool("isBoosting", true);
            soundM.Play(SFX.dash);
        }
    }

    public void handleWindMovement(float input)
    {
        if (currentWState == windState.onWind && onAir)
        {
            rb.velocity += new Vector2(0, Time.deltaTime * input * windControl);
        }
    }

    void windImpulse(Vector2 direction, float windStrength)
    {
        rb.velocity += direction * windBoost * Time.deltaTime;
        anim.SetBool("isBoosting", true);
        setFacingDirection(direction.x);
    }

    void checkAirState()
    {
        if (rb.velocity.y < fallSpeed && canCollideWithGround)
        {
            anim.SetBool("onAir", true);
            onAir = true;
            anim.SetTrigger("takeOff");
        }
    }

    void checkAnimBool()
    {
        anim.SetBool("isIdle", !isActivelyMoving && !onAir);
    }

    void setFacingDirection(float inputForce)
    {
        transform.localScale = new Vector3(Mathf.Sign(inputForce), transform.localScale.y, transform.localScale.z);
    }

    public void stopMotionOnGround()
    {
       if (!onAir && !isActivelyMoving)
       {
           rb.velocity = new Vector2(rb.velocity.x / groundFriction, rb.velocity.y);
       }
    }

    private void setCheckpoint(GameObject CP)
    {
        gm.setLastCheckpoint(CP, ammo);
    }

    public void goToCheckpoint()
    {
        Vector3 CPposition = gm.getLastCheckpoint().transform.GetChild(0).position;
        transform.position = new Vector3(CPposition.x, CPposition.y, transform.position.z);
        rb.velocity = Vector2.zero;
        ammo = gm.getLastBugAmount();
        gm.setAmmo(ammo, false);
    }

    private void checkLevelFall()
    {
        if (transform.position.y < gm.getMapCoordinate("Yo"))
        {
            if (SceneManager.GetActiveScene().buildIndex != (int)scene.tutorial) gm.setLives(-1);
            gm.triggerGoToCP();
        }
    }

    void checkLevelFinish(Collider2D collision)
    {
        if (collision.tag == "Finish")
        {
            rb.velocity = Vector2.zero;
            jumpForce = 0;
            horSpeed = 0;
            hasReachedEnd = true;
            anim.Play("birbSleep");
        }
    }

    void checkSleepEnd()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && hasReachedEnd && !hasSlept)
        {
            soundM.Play(SFX.birbSnore);
            if (SceneManager.GetActiveScene().buildIndex == (int)scene.tutorial) FindObjectOfType<sceneManager>().stageSpecificMenuSwitch(0);
            gm.fadeOut(ammo);
            hasSlept = true;
        }
    }
}
