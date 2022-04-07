using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position : MonoBehaviour
{
    public Transform target;
    public float xOffset = 10;
    public float yOffset = 10;
    float xPos;
    float yPos;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        xPos = target.position.x + xOffset;
        yPos = target.position.y + yOffset; ;

        Vector3 NewPosition = new Vector3(xPos, yPos);

        transform.position = NewPosition;
    }
}
