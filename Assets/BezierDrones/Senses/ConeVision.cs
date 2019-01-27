using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeVision : MonoBehaviour
{

   Vector3 lastPosition;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            lastPosition = other.transform.position;
        }
    }


    IEnumerator OnPatrol()
    {
        while (true)
        {







            yield return null;
        }





    }


}
