using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class SenseBase : MonoBehaviour
{
    public bool isKill;

    public Vector3 lastPosition;
    [SerializeField] UnityEvent ThingToDo;

    protected virtual void Start()
    {

    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (isKill == false)
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
