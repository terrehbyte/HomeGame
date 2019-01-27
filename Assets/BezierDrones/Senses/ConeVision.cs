using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//for testing
using UnityEngine.Events;

public class ConeVision : MonoBehaviour
{

    [SerializeField]UnityEvent Thing;

   [SerializeField] Image indicator;
   Vector3 lastPosition;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            lastPosition = other.transform.position;
            Thing.Invoke();
            if (indicator != null)
            {
                indicator.color = Color.red;
                transform.LookAt(lastPosition);
            }
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




