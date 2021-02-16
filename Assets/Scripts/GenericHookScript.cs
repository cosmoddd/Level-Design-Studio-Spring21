using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericHookScript : MonoBehaviour
{
    public LineRenderer line;
    public Vector3 newPosition;
    public Vector3 lineStart;
    public static Vector3 originalPosition;
    public  Vector3 originalPositionINSPECT;
    public GameObject hook;
    public GameObject gun;

    public static bool retract = false;

    public float speed = 1;
    
    // Start is called before the first frame update
    void Start()
    {
        //This is so we know what place to pull the hook back to at the end of any action
        //it is referenceable everywhere
        
        originalPosition = hook.transform.localPosition;
        line.enabled = true;
        line.positionCount = 2;

    }

    // Update is called once per frame
    void Update()
    {
        originalPositionINSPECT = originalPosition;

        if (Input.GetKeyDown(KeyCode.R))
        {
            retract = true;
        }

        if (retract == true)
        {
            if (originalPosition == hook.transform.localPosition)
            {
                retract = false;
            }
           hook.transform.localPosition = Vector3.MoveTowards(
                hook.transform.localPosition,
                originalPosition,
                Time.deltaTime * speed);
        }

        newPosition = hook.transform.position;
        lineStart = gun.transform.position;
        line.SetPosition(0, lineStart);
        line.SetPosition(1, newPosition);
    }
}
