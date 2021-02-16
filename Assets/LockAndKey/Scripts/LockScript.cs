using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockScript : MonoBehaviour
{
	//reference to the PlayerKeyTracker script on the player
	public PlayerKeyTracker keyTracker;

	//reference to the key that will unlock this door
	public List<KeyScript> keys = new List<KeyScript>();

	//reference to the player object
	public GameObject player;

	//distance for the door to unlock if you have the key
	public float distanceToUnlock = 10f;

	//Particles that explode after the door opens
	public GameObject OpenParticles;
	

	// Update is called once per frame
	void Update()
	{
		if (Vector3.Distance(transform.position, player.transform.position) < distanceToUnlock)
		{
			int keysFound = 0;
			foreach (KeyScript key in keys)
			{
				if(keyTracker.HeldKeys.Contains(key))
				{
					keysFound++;
				}
			}

			if (keysFound == keys.Count)
			{
				Unlock();
			}
		}

	}

	void Unlock()
	{
		Instantiate(OpenParticles, transform.position, Quaternion.identity);
		this.gameObject.SetActive(false);
	}
}