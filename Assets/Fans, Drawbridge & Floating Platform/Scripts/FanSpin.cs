using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanSpin : MonoBehaviour
{

    [SerializeField]
    private float rotationSpd;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.Rotate(0, rotationSpd * Time.deltaTime, 0);
    }
}
