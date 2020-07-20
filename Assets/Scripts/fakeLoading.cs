using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fakeLoading : MonoBehaviour
{
    public float minspeed, maxspeed;
    public GameObject loadItem, bug;
    public float loadTime, loadTimer, distanceToEat;
    public bool hasReachedEnd = false;
    public sceneManager sM;
    public loadAnim la;
    public Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        sM = FindObjectOfType<sceneManager>();
    }

    void Update()
    {
        if (!hasReachedEnd)
        { 
        transform.position += transform.right * Time.deltaTime * Random.Range(minspeed, maxspeed);
        loadTime += Time.deltaTime;
        if(loadTime > loadTimer)
        {
            GameObject leftItem = Instantiate(loadItem);
            leftItem.transform.position = transform.position;
            leftItem.transform.rotation = transform.rotation;
            loadTime = 0;
        }
        }
    }
    private void FixedUpdate()
    {
        if ((bug.transform.position - transform.position).magnitude < distanceToEat)
        {
            sM.ChangeLevel();
            Destroy(bug);
            hasReachedEnd = true;
            anim.speed = 0;
            la.hasReached = true;
        }
    }
}
