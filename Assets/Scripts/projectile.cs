using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile : MonoBehaviour
{
    public float speed;
    public SpriteRenderer sr;
    private void Start()
    {

    }
    void Update()
    {
        transform.localPosition += transform.right * Time.deltaTime * speed;
        sr.flipY = (speed > 0);
        if (transform.position.x < FindObjectOfType<cameraController>().getBoundaries(CamBoundaries.Xo) || transform.position.x > FindObjectOfType<cameraController>().getBoundaries(CamBoundaries.X1))
        {
            Destroy(gameObject);
        }
    }
}
