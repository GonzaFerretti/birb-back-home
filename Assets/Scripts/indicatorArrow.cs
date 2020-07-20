using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class indicatorArrow : MonoBehaviour
{
    public GameObject player;
    public float triggerDistance;
    public SpriteRenderer sr;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector2.Distance((Vector2)player.transform.position, (Vector2)transform.position);
        if (distance < triggerDistance)
        {
            sr.enabled = true;
            float angle = -Vector2.SignedAngle((Vector2)transform.position - (Vector2)player.transform.position, Vector2.right);
        transform.eulerAngles = new Vector3(0, 0, angle);
        }
        else
        {
            sr.enabled = false;
        }
    }
}
