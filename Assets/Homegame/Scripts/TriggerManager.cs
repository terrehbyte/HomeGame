using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TriggerManager : MonoBehaviour
{
    /*
     * Handles fetching Data from the trigger object, and then sending it to the UI
     * <3 
     * 
     */

    public  TextMeshProUGUI textBox;
    private bool shitChanged;
    private string text;
    private Collider lastCollider;

    private void Start()
    {
        text = "";
        shitChanged = true;
        var go = GameObject.FindGameObjectWithTag("TextMeshPro");
        if(go == null)
        {
            Debug.LogWarning("Textbox not present in scene, disabling TriggerMan.");
            enabled = false;
            return;
        }

        textBox = go.GetComponent<TextMeshProUGUI>();

        textBox.GetComponent<TextMeshProUGUI>();
    }
    // Update is called once per frame
    void Update()
    {
        if (textBox == null)
        {
            Debug.Log("MISSING UI ELEMENT");
            return;
        }

        if (shitChanged == true)
        {
            shitChanged = false;
            textBox.text = text;
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
