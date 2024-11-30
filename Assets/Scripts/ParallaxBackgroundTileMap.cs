using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ParallaxBackgroundTileMap : MonoBehaviour
{
    private GameObject cam;
    [SerializeField] private float parallaxEffect;
    private float length;
    private float xPosition;

    void Start()
    {
        cam = GameObject.Find("Main Camera");
        length = GetComponent<TilemapRenderer>().bounds.size.x;
        xPosition = transform.position.x;
    }
    void Update()
    {
        float distanceMoved = cam.transform.position.x * (1 - parallaxEffect);
        float distanceToMove = cam.transform.position.x * parallaxEffect;

        transform.position = new Vector3(xPosition + distanceToMove, transform.position.y);

        if (distanceMoved > xPosition + length)
            xPosition = xPosition + length;
        else if (distanceMoved < xPosition - length)
            xPosition = xPosition - length;
    }
}
