using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleTrigger : MonoBehaviour
{
    //This script can be used to turn off any obstacles for a set time period, after which it turns them on again

    public GameObject Trigger; //The object which will be used as a trigger
    public GameObject Obstacle; //The object which will be turned off for a brief period
    private bool IsItHappening; //private bool that checks if player's collider is touching the trigger gameobject
    public float Timer; //How long will the object be turned off
    private float decreasing; //Internal float that will use the values put into "Timer"


    // Attach this script to your GameManager or any other gameobject in the scene.
    // Start is called before the first frame update
    void Start()
    {
        IsItHappening = false; //Keep the bool false, right from the start
        decreasing = Timer;
    }

    // Update is called once per frame
    void Update() //Checking for Turning Off and Turning On in every frame.
    {
        TurningOff();
        TurningOn();
    }

    public void OnTriggerEnter(Collider other) //Check for collisions. Feel free to remove this and use your own way to trigger
    {
        print(other);
        if (Trigger.GetComponent<Collider>() == other)
        {
            BoolCheck();

            //print(IsItHappening); //Print can be used to check if script is working or not
        }
    }
    public void TurningOff () //Turn off the obstacle and start the timer. Can be called from another script or GameManager.
    {
        if (IsItHappening == true) //Check if Bool is true or not
        {
            if (decreasing > 0f) //If Timer is more than zero
            {
                decreasing -= Time.deltaTime; //decrease time in a countdown

                //print(decreasing);
            }
            Obstacle.gameObject.SetActive(false); //Turn off your obstacle
        }
    }

    public void TurningOn () //Turn on the obstacle again once the timer is complete. Can be called from another script or GameManager.
    {
        if (decreasing <= 0f) //If Timer is at zero
        {
            Obstacle.gameObject.SetActive(true); //Turn on your obstacle
            IsItHappening = false;
            decreasing = Timer; //Bring back the decreased time to it's original set value so it can be triggered again

            //print(IsItHappening);
        }
    }

    public void BoolCheck () //Turn the bool into true
    {
        IsItHappening = true;
    }
}
