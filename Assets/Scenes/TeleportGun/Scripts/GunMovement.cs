using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunMovement : MonoBehaviour
{
    void Update()
    {
        
        //Resets the position, scale, and rotation after the kick from firing the gun. 

        transform.localScale = Vector3.Lerp(transform.localScale,Vector3.one, Time.deltaTime * 8);
        transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * 2);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0,0,0), Time.deltaTime * 8);
    }

    public void Fire() //simulates the kick of a gun
    {
        transform.localPosition = new Vector3(0,-.1f,-.6f);
        transform.localRotation = Quaternion.Euler(-6, 0, 0);
    }
}
