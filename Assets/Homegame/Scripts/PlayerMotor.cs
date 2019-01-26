using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    [Header("Components")]
    public CharacterController charController;
    public Collider coll;

    [Header("Movement")]
    [ReadOnlyField]
    public Vector3 velocity;
    public Camera playerCamera;
    public float groundFriction = 1;
    public float groundAcceleration = 20;
    public float groundMaxSpeed = 5.0f;

    [Header("Ground Check")]
    public float groundCheckLength = 1.5f;
    public LayerMask groundCheckLayer;
    [HideInInspector]
    public Collider groundCol;
    [ReadOnlyField]
    public Vector3 groundNorm;

    // Returns the player's new velocity when moving on the ground
    // accelDir: world-space direction to accelerate in
    // prevVelocity: world-space velocity
    private Vector3 MoveGround(Vector3 accelDir, Vector3 prevVelocity)
    {
        float speed = prevVelocity.magnitude;
        if (speed != 0)
        {
            float drop = speed * groundFriction * Time.fixedDeltaTime;
            prevVelocity *= Mathf.Max(speed - drop, 0) / speed;
        }

        return Accelerate(accelDir, prevVelocity, groundAcceleration, groundMaxSpeed);
    }

    // Returns the player's new velocity based on the given parameters
    // accelDir: world-space direction to accelerate in
    // prevVelocity: world-space velocity
    // accelerate: amount to accelerate by
    // maxSpeed: max player speed to achieve when accelerating
    private Vector3 Accelerate(Vector3 accelDir, Vector3 prevVelocity, float accelerate, float maxSpeed)
    {
        float projVel = Vector3.Dot(prevVelocity, accelDir);
        float accelVel = accelerate * Time.fixedDeltaTime;

        if(projVel + accelVel > maxSpeed)
        {
            accelVel = maxSpeed - projVel;
        }

        return prevVelocity + accelDir * accelVel;
    }

    private bool PerformGroundCheck(Vector3 position, float maxGroundDistance, out Collider groundCollider, out Vector3 groundNormal)
    {
        var potentialGround = Physics.RaycastAll(position, Vector3.down,
                                                 maxGroundDistance, groundCheckLayer,
                                                 QueryTriggerInteraction.Ignore);
        
        foreach(var ground in potentialGround)
        {
            if(ground.collider != coll)
            {
                groundNormal = ground.normal;
                groundCollider = ground.collider;
                return true;
            }
        }

        groundNormal = Vector3.zero;
        groundCollider = null;
        return false;
    }

    void Update()
    {
        PerformGroundCheck(transform.position, groundCheckLength, out groundCol, out groundNorm);

        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"),
                                    0.0f,
                                    Input.GetAxisRaw("Vertical"));

        input.Normalize();
        input = Quaternion.Euler(0, playerCamera.transform.eulerAngles.y, 0) * input;
        input = Vector3.ProjectOnPlane(input, groundNorm);

        velocity = MoveGround(input, velocity);
        Vector3 delta = transform.position + velocity * Time.deltaTime - transform.position;
        charController.Move(delta);
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
