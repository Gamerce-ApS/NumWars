using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMovement : MonoBehaviour
{
    public float speed = 1;
    public float minX = 0;
    public float startX = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position -= new Vector3(speed * Time.deltaTime,0,0);
        if (transform.position.x < minX)
            transform.position = new Vector3(startX, transform.position.y, 0);
    }
}
