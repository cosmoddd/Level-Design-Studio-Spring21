using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script goes on a Key object. Mainly here to identify different keys from each other.
/// </summary>
public class KeyScript : MonoBehaviour
{
    [Tooltip("Particle prefab that is created when you pick up the key")] 
    public GameObject PickupParticles;

    public void PickUp()
    {
        Instantiate(PickupParticles, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }
}
