using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawbridgeDrop : MonoBehaviour
{
    [SerializeField] float ZrotationAngle, YrotationAngle, smoothTime;

    private bool lowerBridge;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (lowerBridge)
        {
            //sets rotation
            Quaternion desiredRotation = Quaternion.Euler(0, YrotationAngle, ZrotationAngle);

            //lerps to desired rotation
            transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, smoothTime * Time.deltaTime);
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            lowerBridge = true;
            Debug.Log(other.name + "lower bridge");
        }
    }

}
