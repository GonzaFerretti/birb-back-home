using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    private GameObject bird;
    private Camera main;
    public float bgXo, bgX1, bgYo, bgY1;
    public ParticleSystem part;
    public Transform[] plxLevels;
    public float[] plxSpeeds;
    private GameObject parallax;
    public bool hasCPed;
    private void Awake()
    {
        main = GetComponent<Camera>();
        bird = FindObjectOfType<birdBody>().gameObject;
        generateParallaxLayers();
    }

    private void Start()
    {
        part.Play();
    }

    void Update()
    {
       followBird(bird);
       applyParallax();
    }

    private void generateParallaxLayers()
    {
        parallax = GameObject.FindGameObjectWithTag("parallax");
        for (int i = 0; i < plxLevels.Length; i++)
        {
            plxLevels[i] = parallax.transform.GetChild(i);
        }
    }

    public void applyParallax()
    {
        for (int i = 0; i < plxLevels.Length; i++)
        {
            plxLevels[i].position = new Vector3(plxSpeeds[i] * bird.transform.position.x, plxLevels[i].position.y, -1.5f);
        }
    }
    private void followBird(GameObject _bird)
    {
        float camYsize = main.orthographicSize;
        float camXsize = camYsize * Screen.width / Screen.height;
        float clampedX = Mathf.Clamp(_bird.transform.position.x, bgXo + camXsize, bgX1 - camXsize);
        float clampedY = Mathf.Clamp(_bird.transform.position.y, bgYo + camYsize, bgY1 - camYsize);
        transform.position = new Vector3(clampedX, clampedY,transform.position.z);
    }

    public float getBoundaries(CamBoundaries selectedBoundary)
    {
        float returnValue = 0;
        float vertExtent = main.orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;
        
        switch (selectedBoundary)
        {
            case CamBoundaries.Xo:
                returnValue = transform.position.x - horzExtent;
                break;
            case CamBoundaries.X1:
                returnValue = transform.position.x + horzExtent;
                break;
            case CamBoundaries.Yo:
                returnValue = transform.position.y - vertExtent;
                break;
            case CamBoundaries.Y1:
                returnValue = transform.position.y + vertExtent;
                break;
        }
        return returnValue;
    }
}
public enum CamBoundaries
{
    Xo = 1,
    X1 = 2,
    Yo = 3,
    Y1 = 4,
}
