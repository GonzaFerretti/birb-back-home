using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class boss : MonoBehaviour
{
    private Transform mainCamera;
    private float startXdif, currentYpos, currentMax;
    public float snapDistance, speed;
    public float birdMinY, birdMaxY, margin, spawnerPosDelta;
    private bool willAttack = false;
    private bool goingDown, hasReachedEnd = false;
    public float windSpeed;
    public bool isStunned, isInverted;
    public float stunnedTimer, stunnedTime;
    public GameObject launcher;
    public Animator headAnim, bodyAnim;
    public GameObject projectile;
    private difficultyManager dM;
    public Transform player;
    private soundManager soundM;
    public float windAttackCooldown;
    public float windAttackTime;


    void applyDifficulty()
    {
        float[] settings = dM.getDifficultySettings(difficultyObject.boss);
        speed = settings[0];
        margin = settings[1];
        stunnedTimer = settings[2];
    }

    private void Awake()
    {
        dM = FindObjectOfType<difficultyManager>();
        applyDifficulty();
    }

    void Start()
    {
        player = FindObjectOfType<birdBody>().transform;
        spawnerPosDelta = transform.position.y - launcher.transform.position.y;
        UpdateBirdPos();
        currentYpos = Random.Range(birdMinY, birdMaxY);
        goingDown = (currentYpos < (birdMinY + birdMaxY) / 2);
        mainCamera = GameObject.Find("Main Camera").transform;
        startXdif = Mathf.Abs(transform.position.x - mainCamera.position.x);
        soundM = FindObjectOfType<soundManager>();
        soundM.Play(SFX.bossScreech);
    }
    
    void checkSlow(Collider2D collision)
    {
        if (collision.tag == "bossSlow")
        { 
        hasReachedEnd = true;
        }
    }

    void checkInverted()
    {
        if (player.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            isInverted = true;
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
            isInverted = false;
        }
    }

    void UpdateBirdPos()
    {
        birdMinY = player.position.y - margin + spawnerPosDelta;
        birdMaxY = player.position.y + margin + spawnerPosDelta;
    }

    void checkEscape()
    {
        if (hasReachedEnd && isOutOfScreen())
            {
                soundM.Play(SFX.bossScreech);
                Destroy(gameObject);
            }
    }

    bool isOutOfScreen()
    {
        bool isOutOFScreen = false;
        float camLimit = FindObjectOfType<cameraController>().getBoundaries(CamBoundaries.Xo);
        float spriteXlimit = transform.position.x + transform.GetChild(0).GetComponent<SpriteRenderer>().bounds.extents.x;
        return isOutOFScreen = spriteXlimit < camLimit;
    }
    void checkStunTimer()
    {
        if (stunnedTime < stunnedTimer)
        {
            stunnedTime += Time.deltaTime;
        }
        else
        {
            stunnedTime = 0;
            isStunned = false;
            headAnim.SetBool("isStunned", false);
            soundM.Play(SFX.bossScreech);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "birdProjectile")
        {
            Hit(collision);
        }
        checkSlow(collision);
    }
    
    private void Hit(Collider2D collision)
    {
        if (!isStunned)
        {
        soundM.Play(SFX.bossHit);
        Destroy(collision.gameObject);
        isStunned = true;
        bodyAnim.Play("bossHit");
        headAnim.Play("bossHitHead");
        headAnim.SetBool("isStunned", true);
        }
    }

    void MoveToWaypoint()
    {
        if (transform.position.y != currentYpos)
        {
            float directionMult = (transform.position.y > currentYpos) ? -1 : 1;
            transform.position += transform.up * directionMult * speed * Time.deltaTime;
            if (Mathf.Abs(currentYpos-transform.position.y ) < snapDistance)
            {
                transform.position = new Vector3(transform.position.x,currentYpos, transform.position.z);
                willAttack = true;
            }
        }
        else
        {
            UpdateBirdPos();
            speed = 10;
            if (birdMaxY != transform.position.y && birdMinY != transform.position.y )
            {
                goingDown = !goingDown;
            }
            currentMax = (goingDown) ? birdMinY : birdMaxY;
            currentYpos = Random.Range(transform.position.y, currentMax);
        }
    }

    void Attack()
    {
        if (windAttackTime <= windAttackCooldown)
        {
            windAttackTime += Time.deltaTime;
        }
        else
        {
            GameObject spawnedWind;
            spawnedWind = Instantiate(projectile);
            headAnim.Play("bossHeadAttack");
            spawnedWind.transform.position = launcher.transform.position;
            if (isInverted)
            {
                spawnedWind.transform.GetChild(0).localEulerAngles = Vector3.zero;
                spawnedWind.transform.GetComponent<windProjectile>().speed *= -1;
            }
            soundM.Play(SFX.bossAttack);
            foreach (SpriteRenderer corriente in spawnedWind.GetComponentsInChildren<SpriteRenderer>())
            {
                corriente.color = new Color(0.2111285f, 0.1029726f, 0.245283f);
            }
            windAttackTime = 0;
            willAttack = false;

        }
    }

    void FixedUpdate()
    {
        checkInverted();
        checkEscape();
        if (!hasReachedEnd)
        { 
        transform.position = new Vector3(mainCamera.position.x + startXdif, transform.position.y, transform.position.z);
        }
        if (!isStunned)
        { 
        if (!willAttack)
        {
            MoveToWaypoint();
        }
        else
        {
            Attack();
        }
        }
        else
        {
            checkStunTimer();
        }
    }
}
