using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController characterController;

    public float speed = 12f;

    Vector3 y_velocity;
    public float gravity = -9.8f;

    
    public float groundDistance = 0.4f; //radius
    public LayerMask groundMask;
    bool isGrounded;

    public float jumpHeight = 10f;
    private Vector3 hitNormal;
    public WallClimb wallClimb;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //isGrounded = characterController.isGrounded;
        isGrounded = Physics.CheckSphere(transform.position - new Vector3(0,characterController.height/2,0), groundDistance,groundMask);
        

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

       


        

        if (Input.GetButtonDown("Jump") && isGrounded&&!wallClimb.isClimbing)
        {
            y_velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        //RaycastHit hit;
        //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
        /*if (Input.GetButton("Jump") && Physics.Raycast(transform.position - new Vector3(0,3.8f/2,0), transform.TransformDirection(Vector3.forward), out hit, 2))
        {
            characterController.slopeLimit = 180;
            y_velocity.y = 5;
        }
        else
        {
            characterController.slopeLimit = 45;
        }
        */
        
        if (!wallClimb.isClimbing)
        {
            Vector3 move = transform.right * x + transform.forward * z;

            characterController.Move(move * speed * Time.deltaTime);
            y_velocity.y += gravity * Time.deltaTime;
            if (isGrounded && y_velocity.y < 0)
            {
                y_velocity.y = -1f;
            }
            //Debug.Log(y_velocity.y);
            characterController.Move(y_velocity * Time.deltaTime);
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        hitNormal = hit.normal;
    }
}
