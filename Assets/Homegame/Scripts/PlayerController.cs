using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController charController;
    public Collider coll;
    public float speed = 5.0f;
    public float groundCheckLength = 1.5f;
    public LayerMask groundCheckLayer;

    void Start()
    {
        charController = GetComponent<CharacterController>();
        coll = GetComponent<Collider>();
    }

    void Update()
    {
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"),
                                    0.0f,
                                    Input.GetAxisRaw("Vertical"));

        input *= speed * Time.deltaTime;

        if(input.magnitude > 0)
        {
            charController.Move(input);
        }

        var potentialGround = Physics.RaycastAll(transform.position, Vector3.down,groundCheckLength, groundCheckLayer, QueryTriggerInteraction.Ignore);
        foreach(var ground in potentialGround)
        {
            if(ground.collider != coll)
            {

            }
        }
    }

    void OnDrawGizmos()
    {

    }
}
