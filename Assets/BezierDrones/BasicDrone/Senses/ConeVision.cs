using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//for testing
using UnityEngine.Events;

public class ConeVision : MonoBehaviour
{

    [SerializeField]UnityEvent Thing;



   [SerializeField] Material indicator;
   Vector3 lastPosition;

    private void Start()
    {
        if (indicator != null)
        {
            indicator = GetComponent<Renderer>().material;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            lastPosition = other.transform.position;
            Thing.Invoke();
            if (indicator != null)
            {
                indicator.color = Color.red;
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




