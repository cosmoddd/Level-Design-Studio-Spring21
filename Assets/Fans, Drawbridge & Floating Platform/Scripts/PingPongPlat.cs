using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPongPlat : MonoBehaviour
{
    [SerializeField] float speed, moveDistX, moveDistY, moveDistZ;
    Vector3 StartPoint; 
    Vector3 EndPoint;

    // Start is called before the first frame update
    void Start()
    {
        StartPoint = new Vector3(transform.position.x, transform.position.y, transform.position.z); //choose starting axis
        EndPoint = new Vector3(transform.position.x + moveDistX, transform.position.y + moveDistY, transform.position.z + moveDistZ); //choose end axis
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float time = Mathf.PingPong(Time.time * speed, 1);
        transform.position = Vector3.Lerp(StartPoint, EndPoint, time);
    }
}
