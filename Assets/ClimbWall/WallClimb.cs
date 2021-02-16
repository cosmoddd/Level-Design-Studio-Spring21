using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallClimb : MonoBehaviour
{
    private CharacterController characterController;
    private Vector3 y_velocity;
    private float defaultSlopeLimit;
    [Tooltip("speed of the character climbs the wall")]
    [SerializeField]private float climbSpeed;
    [SerializeField] private KeyCode climbKey;
    [Tooltip("set it slightly over the radius of your character controller")]
    [Range(0.1f, 3.0f)] [SerializeField] private float WallDetectRaycastDistance;
    [Tooltip("set it to -1 to set every surface climbable")]
    [SerializeField] private int climbableLayerMaskIndex;
    [SerializeField] private string horizontalInputAxis;
    [SerializeField] private string verticalInputAxis;
    private enum climbState {notClimbing, climbing, grabLedge}
    [HideInInspector]public bool isClimbing; //use this public bool to turn off gravity during climbing in your first person controller 
    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        defaultSlopeLimit = characterController.slopeLimit;
        climbableLayerMaskIndex = 1 << climbableLayerMaskIndex;
        if (climbableLayerMaskIndex == -1)
        {
            
            climbableLayerMaskIndex = ~climbableLayerMaskIndex;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis(horizontalInputAxis);
        float z = Input.GetAxis(verticalInputAxis);
        
        if (Input.GetKey(climbKey) && CheckClimbState() == climbState.climbing)
        {
            characterController.slopeLimit = 180;
            Vector3 move = transform.right * x + transform.forward * z;
            characterController.Move(move * climbSpeed/2 * Time.deltaTime);
            y_velocity.y = climbSpeed;
            characterController.Move(y_velocity * Time.deltaTime);
            isClimbing = true;
        }
        else if (Input.GetKey(climbKey) && CheckClimbState() == climbState.grabLedge)
        {
            characterController.slopeLimit = 180;
            Vector3 move = transform.right * x + transform.forward * z;
            characterController.Move(move * climbSpeed/2 * Time.deltaTime);
            y_velocity.y = climbSpeed;
            
            characterController.Move(y_velocity * Time.deltaTime);
            isClimbing = true;
        }
        else 
        {
            characterController.slopeLimit = defaultSlopeLimit;
            y_velocity.y = 0;
            isClimbing = false;
        }
        
    }

    private climbState CheckClimbState()
    {
        RaycastHit hit;
        bool UpperRayHit = Physics.Raycast(transform.position + new Vector3(0, characterController.height / 2, 0), transform.TransformDirection(Vector3.forward), out hit, WallDetectRaycastDistance,climbableLayerMaskIndex);
        bool BottomRayHit = Physics.Raycast(transform.position - new Vector3(0, characterController.height / 2, 0), transform.TransformDirection(Vector3.forward), out hit, WallDetectRaycastDistance, climbableLayerMaskIndex);

        if (!UpperRayHit && !BottomRayHit)
        {
            return climbState.notClimbing;
        }
        else if (!UpperRayHit && BottomRayHit)
        {
            return climbState.grabLedge;
        }
        else return climbState.climbing;
    }
}
