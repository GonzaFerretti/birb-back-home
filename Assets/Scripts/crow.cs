using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum crowState
    {
        triggered,
        onPatrol,     
        attacked,
        scared,
    }

public class crow : MonoBehaviour
{
    public GameObject[] waypoints;
    public birdBody player;
    private Vector3 startingPos;
    private Collider2D col;
    public SpriteRenderer sr;
    public GameObject damageFeather;
    private difficultyManager dM;
    public ParticleSystem part;
    private soundManager soundM;
    private Animator anim;
    private int currentWp = 0, step = 1;
    public crowState currentState;
    public float featherTimer, featherTime = 0;
    public float speed, fleeSpeed, snapDistance, attackDistance, approachSpeed, scaredTime, scaredTimer;
    private float lastPosX;

    private void Awake()
    {
        dM = FindObjectOfType<difficultyManager>();
        applyDifficulty();
    }

    void applyDifficulty()
    {
        float[] settings = dM.getDifficultySettings(difficultyObject.crow);
        speed = settings[0];
        approachSpeed = settings[1];
        scaredTimer = settings[2];
        attackDistance = settings[3];
    }

    void Start()
    {
        waypoints = GameObject.FindGameObjectsWithTag("crowWp");
        player = FindObjectOfType<birdBody>();
        anim = GetComponent<Animator>();
        startingPos = transform.position;
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        soundM = FindObjectOfType<soundManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "birdProjectile")
        {
            Hit(collision);
        }
    }

    private void Hit(Collider2D collision)
    {
        Destroy(collision.gameObject);
        currentState = crowState.attacked;
        part.Emit(10);
        soundM.Play(SFX.crowFlee);
        soundM.Play(SFX.crowCaws);
    }

    void Update()
    {
        checkOrientation();
        if (currentState != crowState.attacked && currentState != crowState.scared)
        {
            checkBirdDistance();
            if (currentState == crowState.onPatrol)
            {
                MoveToWaypoint();
            }
            else
            {
                attack();
            }
        }
        else
        {
            flee();
        }
    }

    void checkBirdDistance()
    {
        if (((Vector2)player.transform.position - (Vector2)transform.position).magnitude > attackDistance)
        {
            currentState = crowState.onPatrol;
        }
        else
        {
            currentState = crowState.triggered;
        }
    }

    void checkOrientation()
    {
        float currentPos = transform.position.x;
        float movementDirection = Mathf.Sign(lastPosX - currentPos);
        transform.localScale = new Vector3(movementDirection, 1, 1);
        lastPosX = currentPos;
    }
    void attack()
    {
        Vector3 MovementVector = (player.transform.position - transform.position).normalized;
        transform.position += MovementVector * approachSpeed * Time.deltaTime;
    }

    void MoveToWaypoint()
    {
            Vector3 currentWpPosition = waypoints[currentWp].transform.position;
            if (transform.position != currentWpPosition)
        {
            Vector3 MovementVector = (currentWpPosition - transform.position).normalized;
            transform.position += MovementVector * speed * Time.deltaTime;
            if (Vector3.Distance(currentWpPosition, transform.position) < snapDistance)
            {
                transform.position = currentWpPosition;
            }
        }
        else
        {
            currentWp += step;
            if (currentWp == waypoints.Length - 1 || currentWp == 0)
            {
                step *= -1;
            }
        }
    }

    void flee()
    {
            if (transform.position != startingPos)
            {
                Vector3 MovementVector = (startingPos - transform.position).normalized;
                transform.position += MovementVector * fleeSpeed * Time.deltaTime;
                if (Vector3.Distance(startingPos, transform.position) < snapDistance)
                {
                    transform.position = startingPos;
                }
            if (featherTime < featherTimer)
            {
                featherTime += Time.deltaTime;
            }
            else
            {
                part.Emit(3);
                featherTime = 0;
            }
            col.enabled = false;
        }
            else
            {
                featherTime = 0;
                currentState = crowState.scared;
                beScared();
            }
    }

    void beScared()
    {
        if (scaredTime < scaredTimer)
        {
            scaredTime += Time.deltaTime;
        }
        else
        {
            currentState = crowState.onPatrol;
            scaredTime = 0;
            col.enabled = true;
        }
    }
}
