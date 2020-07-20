using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wind : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D col;
    public GameObject[] winds;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        winds = new GameObject[transform.childCount];
        getWinds();
        varyWinds();
    }
    
    public void getWinds()
    {
        for (int i = 0; i < winds.Length; i++)
        {
            winds[i] = transform.GetChild(i).gameObject;
        }
    }

    public void varyWinds()
    {
        foreach (GameObject wind in winds)
        {
            wind.GetComponent<Animator>().SetFloat("offset", Random.Range(0f, 1f));
        }
    }

    public Vector2 getDirection()
    {
        Vector2 resultVector = transform.right;
        return resultVector;
    }

    public float getStrength()
    {
        float strength = new Vector2(transform.localScale.x, transform.localScale.y).magnitude;
        return strength;
    }
}
