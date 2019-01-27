using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollController : MonoBehaviour
{
    [SerializeField]
    Rigidbody[] ragdoll;
    [SerializeField]
    Animator animController;

    // Use this for initialization
    void Start()
    {
        ragdoll = GetComponentsInChildren<Rigidbody>();
        //animController = GetComponent<Animator>();
        initializeRagdoll();

    }

    public void initializeRagdoll()
    {
        for (int i = 0; i < ragdoll.Length; i++)
        {
            ragdoll[i].isKinematic = true;
            animController.enabled = true;
        }

    }

    public void RagdollToggle()
    {
        for (int i = 0; i < ragdoll.Length; i++)
        {
            ragdoll[i].isKinematic = !ragdoll[i].isKinematic;
            animController.enabled = false;

        }
    }
}
