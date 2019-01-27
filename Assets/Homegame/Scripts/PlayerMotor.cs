using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    [Header("Components")]
    public CharacterController charController;
    public CapsuleCollider coll;
    public PlayerManager manager;

    [Header("Movement")]
    public float sidleLerpSpeed = 1;
    [ReadOnlyField]
    public Vector3 velocity;
    [ReadOnlyField]
    public Camera playerCamera;
    public float groundFriction = 11;

    public float groundWalkAcceleration = 50;
    public float groundWalkMaxSpeed = 5.0f;

    public float groundRunAcceleration = 50;
    public float groundRunMaxSpeed = 5.0f;

    public float groundSidleAcceleration = 50;
    public float groundSidleMaxSpeed = 5.0f;

    [Header("Crouch")]
    public float crouchHeight = 1;
    public float standHeight = 2;
    public float crouchDuration = 0.5f;
    public bool crouchWish = false;
    private float crouchTimer;
    public float crouchProgress
    {
        get
        {
            return Mathf.Clamp(crouchTimer / crouchDuration, 0, 1);
        }
    }

    [Header("Sidle")]
    public float sidleAlignmentDegreesPerSecond = 90.0f;
    bool sidleAligned = false;
    Vector3 sidleSurfaceNormal;

    [Header("Ground Check")]
    public float groundCheckLength = 1.5f;
    public LayerMask groundCheckLayer;
    [HideInInspector]
    public Collider groundCol;
    [ReadOnlyField]
    public Vector3 groundNorm;


    private void Awake()
    {
        playerCamera = Camera.main;
    }

    public Vector3 ControllerToWorldDirection(Vector3 controllerDir)
    {
        controllerDir.Normalize();
        controllerDir = Quaternion.Euler(0, playerCamera.transform.eulerAngles.y, 0) * controllerDir;
        controllerDir = Vector3.ProjectOnPlane(controllerDir, groundNorm);

        return controllerDir;
    }

    public void Walk(Vector3 walkDir)
    {
        Debug.Log("WALKING");
        Vector3 input = ControllerToWorldDirection(walkDir);

        velocity = MoveGround(input, velocity, groundWalkAcceleration, groundWalkMaxSpeed);
        Vector3 delta = transform.position + velocity * Time.deltaTime - transform.position;
        charController.Move(delta);

        transform.forward = velocity.normalized;
    }

    public void Run(Vector3 runDir)
    {
        Debug.Log("RUNNING");
        Vector3 input = ControllerToWorldDirection(runDir);

        velocity = MoveGround(input, velocity, groundWalkAcceleration, groundWalkMaxSpeed);
        Vector3 delta = transform.position + velocity * Time.deltaTime - transform.position;
        charController.Move(delta);

        transform.forward = velocity.normalized;
    }

    public void Sidle(Vector3 input, Vector3 wallForward, System.Action exitSidleCallback)
    {
        // exit if player pulls away from wall
        if(input.z < 0.0f) { exitSidleCallback.Invoke(); return; }
        input.z = 0.0f;

        sidleAligned = Vector3.Angle(transform.forward, wallForward) < 5.0f;
        if (sidleAligned == false)
        {
            // @anim - player turning!

            sidleSurfaceNormal = wallForward;
            transform.forward = Vector3.RotateTowards(transform.forward, wallForward, Mathf.Deg2Rad * (sidleAlignmentDegreesPerSecond * Time.deltaTime), 0.0f);

            return;
        }

        Vector3 worldWishDir = Quaternion.Euler(0, Quaternion.LookRotation(-wallForward, Vector3.up).eulerAngles.y, 0) * input;
        velocity = MoveGround(worldWishDir, velocity, groundSidleAcceleration, groundSidleMaxSpeed);
        Vector3 delta = transform.position + velocity * Time.deltaTime - transform.position;
        charController.Move(delta);
    }

    public void Idle(Vector3 idleDir)
    {
        Debug.Log("IDLE");
    }

    public void StartCrouch()
    {
        crouchWish = true;
    }

    public void StopCrouch()
    {
        crouchWish = false;
        crouchTimer = 0.0f;
    }

    void doRock()
    {

    }

    void doTakedown()
    {

    }

    void doKnock()
    {

    }

    // Returns the player's new velocity when moving on the ground
    // accelDir: world-space direction to accelerate in
    // prevVelocity: world-space velocity
    private Vector3 MoveGround(Vector3 accelDir, Vector3 prevVelocity, float acceleration, float maxSpeed)
    {
        float speed = prevVelocity.magnitude;
        if (speed != 0)
        {
            float drop = speed * groundFriction * Time.fixedDeltaTime;
            prevVelocity *= Mathf.Max(speed - drop, 0) / speed;
        }

        return Accelerate(accelDir, prevVelocity, acceleration, maxSpeed);
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
        crouchTimer += (crouchWish ? 1.0f : 0.0f) * Time.deltaTime;
        charController.height = coll.height = Mathf.Lerp(standHeight, crouchHeight, crouchProgress);
        charController.center = coll.center = Vector3.up * (charController.height - 2) / 2;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * groundCheckLength);

        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position, sidleSurfaceNormal * 10.0f);
    }

    void Reset()
    {
        charController = GetComponent<CharacterController>();
        coll = GetComponent<CapsuleCollider>();
    }
}
