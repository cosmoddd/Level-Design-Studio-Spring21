using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachPlatform : MonoBehaviour
{
    public GameObject Player;

    private void OnTriggerEnter (Collider other)
    {
        if (other.gameObject == Player) {
            Player.transform.parent = transform;
            Debug.Log("attached");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == Player)
        {
            Player.transform.parent = null;
            Debug.Log("unattached");
        }
    }
}
