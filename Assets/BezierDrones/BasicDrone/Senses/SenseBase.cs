using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class SenseBase : MonoBehaviour
{
    Vector3 lastPosition;
    [SerializeField] UnityEvent ThingToDo;
    [SerializeField] Material indicator;

    protected virtual void Start()
    {
        if (indicator != null)
        {
            indicator = GetComponent<Renderer>().material;
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            lastPosition = other.transform.position;
            ThingToDo.Invoke();
            if (indicator != null)
            {
                indicator.color = Color.red;
            }
        }
    }
}
