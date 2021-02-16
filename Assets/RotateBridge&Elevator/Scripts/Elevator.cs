using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public GameObject movePlatForm;

    private void OnTriggerStay()
    {
        movePlatForm.transform.position += movePlatForm.transform.up * Time.deltaTime;
    }
}
