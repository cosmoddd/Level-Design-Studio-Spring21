using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingSpot: MonoBehaviour
{
    public bool isFishing = false;

    bool isCaught;

    public GameObject Fish;

    public List<GameObject> fishList;

    // Start is called before the first frame update


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "FishingHook")
        {
            Fish = null;
            isFishing = true;
            Debug.Log("Begin fishing");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "FishingHook" && isFishing == true)
        {
            Debug.Log("Still fishing");
        }

       
    }

    private void OnTriggerExit(Collider other)
    {

        if (isCaught)
        {
            Debug.Log("Caught a fish! Glub glub!");

            Debug.Log(Fish);

        }
        else
        {
            Debug.Log("No fishies. :(");

        }

        isCaught = false;

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            isFishing = false;
            Debug.Log("Begin reeling in");
            bool randomCatch = (Random.value > 0.5f);
            int randomValue = Random.Range(0, fishList.Count);

            isCaught = randomCatch;
            Fish = fishList[randomValue];
        }
    }
}
