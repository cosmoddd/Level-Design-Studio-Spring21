using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HazardZone : MonoBehaviour
{
    [Header("Insert Empty GameObject to act as respawn point")]
    public Transform respawnPoint;

    [Header("Make True if you want hitting the trigger to reset the scene")]
    public bool resetScene;

    [Header("How many Fixed Update frames to wait before reset")]
    //This also corresponds to how long it takes for UI elements to appear
    public int waitTimer;

    GameObject player;
    bool beginTeleport;
    int timer;

    AudioSource aS;
    [Header("Insert UI Text object here for message that appears on death")]
    public Text tm;
    Vector2 tmScale;
    Color tmEnd; //These both pull from the tm object on Scene Awake and then reset it to invisible. Change the text object to change these. 

    public void Awake() //Gets AudioSource and Text Data before resetting it so its invisible for the player
    {
        aS = GetComponent<AudioSource>();
        if (tm != null)
        {
            tmScale = tm.gameObject.transform.localScale;
            tmEnd = tm.color;
            ResetText();
        }
    }

        

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") //If the player hits the trigger
        {
            player = other.gameObject;
            player.GetComponent<CharacterController>().enabled = false; //We stop the player from moving
            beginTeleport = true; //We begin the countdown for resetting them
            aS.Play(); //We play a noise to signal they fell to their death (or whatever your narrative justification may be)
        }
    }

    void FixedUpdate()
    {
        if(beginTeleport)
        {
            timer++; //We start ticking up a timer till we reset the player

            if (tm != null)
            {
                tm.gameObject.transform.localScale = Vector2.Lerp(tmScale * 0.25f, tmScale, timer / (waitTimer * 0.5f)); //We scale the text up so that it reaches its original scale halfway through the rest process, so the player has time to read it
                tm.color = Color.Lerp(Color.clear, tmEnd, timer / (waitTimer * 0.5f)); //We make the color of the text transition from invisible to its original color so that it can be fully visible by halfway through the reset process
            }

            if(timer > waitTimer) //Once our timer reaches waitTimer
            {
                beginTeleport = false; //Stop the above
                timer = 0; //Reset the timer
                ResetPlayer(); //Reset the player
                if (tm != null)
                {
                    ResetText(); //Reset the text
                }
            }
        }
    }

    public void ResetPlayer()
    {
        if (!resetScene) //If we arn't resetting the scene
        {
            player.transform.position = respawnPoint.position; //Move the player to the respawn point
            player.GetComponent<CharacterController>().enabled = true; //Let the player move again
        }

        else //If we are resetting the scene
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); //Grab the name of the current scene and load it
        }
    }

    void ResetText() //Sets the text color to clear and the scale to zero, just to ensure its out of the way.
    {
        tm.color = Color.clear;
        tm.gameObject.transform.localScale = Vector2.zero;
    }
}
