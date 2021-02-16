using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookMoveableObjects : MonoBehaviour
{
    GameObject hookedObj = null;

    public Camera cam;
    
    public GameObject hook;

    private GameObject temp;

    public GameObject player;

    
    //Fishing variables
    private RigidbodyConstraints originalConstraints;

    public GameObject fishingHook;

    public Rigidbody rbfishingHook;


    //Enemy variables
    public int damageDealt = 10;
  
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
            /*RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);*/


            if (Physics.Raycast(ray, out hit))
            {

                Debug.Log(hit.collider.gameObject.name);

                if (hit.collider != null && hit.collider.gameObject.tag == "HookableObjects")
                {
                    hook.transform.parent = null;
                    hook.GetComponent<MeshRenderer>().enabled = true;
                    hook.transform.position = hit.point;
                    hit.collider.transform.SetParent(hook.transform);
                    //hit.collider.enabled = false;
                }

                //FishingScript!
                if (hit.collider != null && hit.collider.gameObject.tag == "FishingSpot")
                {
                    hook.transform.parent = null;

                    player.GetComponent<CharacterController>().enabled = false;
                    hook.GetComponent<MeshRenderer>().enabled = true;
                    hook.transform.position = hit.collider.gameObject.transform.position;
                    hit.collider.gameObject.GetComponent<FishingSpot>().isFishing = true;

                }

                //Enemy Script!
                if (hit.collider != null && hit.collider.gameObject.tag == "Enemy")
                {
                    hook.transform.parent = null;

                    //hook.GetComponent<MeshRenderer>().enabled = true;
                    hook.transform.position = hit.collider.gameObject.transform.position;
                    hit.collider.gameObject.GetComponent<EnemyHealth>().AdjustCurrentHealth(damageDealt);

                    


                }
            }
        }

      

        
        //what to do when the hook is in the OG position again
        if (GenericHookScript.originalPosition == hook.transform.localPosition && hook.transform.childCount == 1)
        {
            hook.transform.GetChild(0).GetComponent<Collider>().enabled = true;
            hook.transform.DetachChildren();
            hook.GetComponent<MeshRenderer>().enabled = false;
        }
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {

            hook.transform.parent = player.transform;

            player.GetComponent<CharacterController>().enabled = true;

        }
    }
}
