using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfRotate : MonoBehaviour
{
    public float rotateSpeedX = 2f;
    public float rotateSpeedY = 2f;
    public float rotateSpeedZ = 2f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotateSpeedX * Time.deltaTime, 
                         rotateSpeedY * Time.deltaTime,
                         rotateSpeedZ * Time.deltaTime);
;    }
}
