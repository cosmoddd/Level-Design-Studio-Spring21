using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPull : MonoBehaviour
{
    GameObject hookedObj = null;

    public Camera cam;

    public GameObject hook;

    private GameObject temp;

    public GameObject player;

    public bool pulled;

    public float speed = 25;

    private string hitObj;

 





    void FixedUpdate()
    {
        //setting the direction of the hook
        Vector3 fwd = cam.transform.TransformDirection(Vector3.forward) * 30;

        //debugging so I can see where the hook is in the editor
        Debug.DrawRay(transform.position, fwd, Color.red);


        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //This is to pick up/snag the hookable objects with the grappling hook 
        if (Input.GetMouseButtonDown(0) && GenericHookScript.retract == false && hook.transform.childCount == 0)
        {

            if (Physics.Raycast(ray, out hit))
            {

                Debug.Log(hit.collider.gameObject.name);

                if (hit.collider != null && hit.collider.gameObject.tag == "StableObjects")
                {
                    hitObj = hit.collider.gameObject.tag;
                    hook.transform.parent = null;
                    hook.GetComponent<MeshRenderer>().enabled = true;
                    hook.transform.position = hit.point;
                    
                }


            }
        }




        //what to do when the hook is in the OG position again
        if (GenericHookScript.originalPosition == hook.transform.localPosition && hook.transform.childCount == 1)
        {
            hook.transform.DetachChildren();
            hook.GetComponent<MeshRenderer>().enabled = false;
        }

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && hitObj == "StableObjects")
        {
            pulled = true;
        }

        if (pulled == true )
        {
            if (GenericHookScript.originalPosition == hook.transform.localPosition)
            {
                pulled = false;
            }
            player.GetComponent<CharacterController>().enabled = false;
            player.transform.position = Vector3.MoveTowards(
                player.transform.position,
                 hook.transform.position + new Vector3(0f,0f,-1.5f), 
                Time.deltaTime * speed);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                hook.transform.DetachChildren();
                hook.GetComponent<MeshRenderer>().enabled = false;
                hook.transform.parent = player.transform;
                hook.transform.localPosition = GenericHookScript.originalPosition;
                hitObj = null;
                pulled = false;
                player.GetComponent<CharacterController>().enabled = true;
                
            }
        }
    }
}