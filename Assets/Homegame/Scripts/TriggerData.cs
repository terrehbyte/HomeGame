using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerData : MonoBehaviour
{
    private void Start()
    {
        if(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>().showTriggers == false)
        {
            this.GetComponent<MeshRenderer>().enabled = false;
        }
    }
    
    public string textUI;

}
