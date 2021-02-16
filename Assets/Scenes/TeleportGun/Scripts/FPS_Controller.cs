using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS_Controller : MonoBehaviour
{
    public Transform Cam;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        CC = GetComponent<CharacterController>();
        yRot = Cam.localRotation.eulerAngles.y;
        xRot = Cam.localRotation.eulerAngles.x;
    }

    float xRot;
    float yRot;

    public float sensitivity;
    public float speed;
    public float gravity;
    public float jumpSpeed;

    Vector3 fDir;
    Vector3 rDir;
    Vector3 uDir;

    Vector3 moveDirection;

    CharacterController CC;

    

    // Update is called once per frame
    void Update()
    {
        xRot -= Input.GetAxis("Mouse Y") * Time.deltaTime * sensitivity;
        yRot += Input.GetAxis("Mouse X") * Time.deltaTime * sensitivity;
        xRot = Mathf.Clamp(xRot,-90,90);
        Cam.localRotation = Quaternion.Euler(xRot,yRot,0);

        fDir = Cam.forward;
        fDir.y = 0;
        rDir = Cam.right;
        rDir.y = 0;
        uDir = Vector3.up;

        moveDirection = new Vector3(Input.GetAxis("Horizontal"), moveDirection.y, Input.GetAxis("Vertical"));
        moveDirection.x *= speed;
        moveDirection.z *= speed;

        if (CC.isGrounded)
        {
            moveDirection.y = -.01f;
            if (Input.GetKey(KeyCode.Space))
            {
                moveDirection.y = jumpSpeed;
            }
        }
        else
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        moveDirection = (fDir*moveDirection.z) + (rDir*moveDirection.x) + (uDir*moveDirection.y);

        CC.Move(moveDirection*Time.deltaTime);

    }
}
