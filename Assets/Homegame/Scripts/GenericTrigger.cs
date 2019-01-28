using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenericTrigger : MonoBehaviour
{
    public UnityEventTrigger OnTriggerEntered;
    public UnityEventTrigger OnTriggerStayed;
    public UnityEventTrigger OnTriggerExited;

    void OnTriggerStart(Collider coll)
    {
        OnTriggerEntered.Invoke(coll, TriggerType.START);
    }

    void OnTriggerStay(Collider coll)
    {
        OnTriggerStayed.Invoke(coll, TriggerType.STAY);
    }

    void OnTriggerExit(Collider coll)
    {
        OnTriggerExited.Invoke(coll, TriggerType.EXIT);
    }
}

public enum TriggerType
{
    START,
    STAY,
    EXIT
}

[System.Serializable]
public class UnityEventTrigger : UnityEvent<Collider, TriggerType> {}