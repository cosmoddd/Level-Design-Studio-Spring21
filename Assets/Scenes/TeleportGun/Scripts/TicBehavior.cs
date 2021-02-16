using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TicBehavior : MonoBehaviour
{
    void Update()
    {
        //resets the scale after the scalaing effect on the arc.
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * .15f, Time.deltaTime * 5f);
    }
}
