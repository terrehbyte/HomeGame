using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class SenseBase : MonoBehaviour
{
    public Vector3 lastPosition;
    [SerializeField] UnityEvent ThingToDo;

    public bool killZone;

    protected virtual void Start()
    {

    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {

            if (killZone == false)
            {

                lastPosition = other.transform.position;
                ThingToDo.Invoke();
            }
            else
            {
                GameState.instance.TriggerPlayerDeath();
            }
            
        }
    }





}
