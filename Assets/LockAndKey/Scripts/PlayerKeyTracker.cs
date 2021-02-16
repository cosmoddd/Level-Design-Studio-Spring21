using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Usage: Put this script on the player GameObject to detect and pick up Keys.
/// </summary>
public class PlayerKeyTracker : MonoBehaviour
{
    [HideInInspector] public List<KeyScript> HeldKeys = new List<KeyScript>();
    

    private void OnTriggerEnter(Collider other)
    {
        KeyScript key = other.GetComponent<KeyScript>();
        
        //If the hit object does not have a key script, don't pick it up
        if (key == null) return;
        
        //Pick it up
        HeldKeys.Add(key);
        key.PickUp();
    }
}
