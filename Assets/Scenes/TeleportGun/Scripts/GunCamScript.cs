using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunCamScript : MonoBehaviour
{



    void LateUpdate()
    {
        if (!transform.parent.GetComponent<Camera>()) { return; }

        GetComponent<Camera>().fieldOfView = transform.parent.GetComponent<Camera>().fieldOfView;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);

        GetComponentInChildren<SpriteRenderer>().color = Color.Lerp(GetComponentInChildren<SpriteRenderer>().color, new Color(1,1,1,0), Time.deltaTime * 4);
    }
}
