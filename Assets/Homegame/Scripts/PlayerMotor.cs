using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    public CharacterController charController;
    public Collider coll;
    public float speed = 5.0f;

    public float groundCheckLength = 1.5f;
    public LayerMask groundCheckLayer;
    [HideInInspector]
    public Collider groundCol;
    [ReadOnlyField]
    public Vector3 groundNorm;

    void Update()
    {
        var potentialGround = Physics.RaycastAll(transform.position, Vector3.down,
                                                 groundCheckLength, groundCheckLayer,
                                                 QueryTriggerInteraction.Ignore);
        
        foreach(var ground in potentialGround)
        {
            if(ground.collider != coll)
            {
                groundNorm = ground.normal;
                groundCol = ground.collider;
                break;
            }
        }

        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"),
                                    0.0f,
                                    Input.GetAxisRaw("Vertical"));

        input.Normalize();
        input = Vector3.ProjectOnPlane(input, groundNorm);
        input *= speed * Time.deltaTime;

        if(input.magnitude > 0)
        {
            charController.Move(input);
        }

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * groundCheckLength);
    }

    void Reset()
    {
        charController = GetComponent<CharacterController>();
        coll = GetComponent<Collider>();
    }
}
