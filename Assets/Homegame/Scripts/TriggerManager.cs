using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerManager : MonoBehaviour
{
    /*
     * Handles fetching Data from the trigger object, and then sending it to the UI
     * <3 
     * 
     */


    private bool shitChanged;
    private string text;
    private Collider lastCollider;

    // Update is called once per frame
    void Update()
    {
        if (shitChanged == true)
        {
            shitChanged = false;
            Debug.Log("TEXT: " + text);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Trigger") && lastCollider != other)
        {
            lastCollider = other;
            shitChanged = true;
            text = other.gameObject.GetComponent<TriggerData>().textUI;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Trigger"))
        {
            lastCollider = null;
            text = "";
            shitChanged = true;
        }
    }

}
