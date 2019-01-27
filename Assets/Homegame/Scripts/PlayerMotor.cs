using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour, IAnimatorStateNotifyReciever
{
    [Header("Components")]
    public CharacterController charController;
    public CapsuleCollider coll;
    public PlayerManager manager;
    [SerializeField] Animator animator;

    [Header("Movement")]
    public float sidleLerpSpeed = 1;
    public float sidleAlignmentFudgeAmount = 0.0f;
    [ReadOnlyField]
    public Vector3 velocity;
    [ReadOnlyField]
    public Camera playerCamera;
    public Transform sidleCamera;
    public Transform peekCamera;
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
    public float sidleAlignmentTranslationPerSecond = 2.0f;
    Vector3 sidleSurfaceNormal;
    public float sidleValidQueryLength = 1.0f;
    public float sidleCameraDistance = 5.0f;
    public float sidleEdgeLimit = 0.5f;
    public float sidleExitZThreshold = 0.3f;

    [Header("Ground Check")]
    [ReadOnlyField]
    public bool grounded;
    public float groundCheckLength = 1.5f;
    public LayerMask groundCheckLayer;
    [HideInInspector]
    public Collider groundCol;
    [ReadOnlyField]
    public Vector3 groundNorm;
    private float DELETEME;

    private void Awake()
    {
        playerCamera = Camera.main;
    }
    private void Start()
    {
        //this should work? sorry if it doesnt. UwU
        animator = this.GetComponentInChildren<Animator>();
    }

    public Vector3 ControllerToWorldDirection(Vector3 controllerDir, float maxMagnitude = 1)
    {
        controllerDir = controllerDir.normalized * Mathf.Min(maxMagnitude, controllerDir.magnitude);
        controllerDir = Quaternion.Euler(0, playerCamera.transform.eulerAngles.y, 0) * controllerDir;
        controllerDir = Vector3.ProjectOnPlane(controllerDir, groundNorm);

        return controllerDir;
    }

    public void Walk(Vector3 walkDir)
    {
        Vector3 input = ControllerToWorldDirection(walkDir);

        velocity = MoveGround(input, velocity, groundWalkAcceleration, groundWalkMaxSpeed);
        Vector3 delta = transform.position + velocity * Time.deltaTime - transform.position;
        charController.Move(delta);

        transform.forward = velocity.normalized;
        animator.SetFloat("Speed", velocity.magnitude);
    }

    public void Run(Vector3 runDir)
    {
        Vector3 input = ControllerToWorldDirection(runDir);

        velocity = MoveGround(input, velocity, groundWalkAcceleration, groundWalkMaxSpeed);
        Vector3 delta = transform.position + velocity * Time.deltaTime - transform.position;
        charController.Move(delta);

        transform.forward = velocity.normalized;
        animator.SetFloat("Speed", velocity.magnitude);
    }

    public void Sidle(Vector3 input, Vector3 wallForward, System.Action exitSidleCallback)
    {
        sidleCamera.transform.position = transform.position - wallForward * sidleCameraDistance;
        sidleCamera.gameObject.SetActive(true);

        // exit if player pulls away from wall
        if(input.z < sidleExitZThreshold)
        {
            animator.SetBool("isSidle", false);
            animator.SetFloat("Speed", velocity.magnitude);

            sidleCamera.gameObject.SetActive(false);
            exitSidleCallback.Invoke();
            return;
        }
        input.z = 0.0f;

        bool proximityCheck = Vector3.Distance(manager.sidleWallEntryPoint, transform.position) < manager.selfCapsuleCollider.radius + 0.1f;
        bool rotationCheck = (Vector3.Angle(transform.forward, wallForward) < 5.0f);
        bool sidleAligned = rotationCheck;//proximityCheck && ;
        if (!sidleAligned)
        {
            // @anim - player turning!

            sidleSurfaceNormal = wallForward;
            transform.forward = Vector3.RotateTowards(transform.forward, wallForward, Mathf.Deg2Rad * (sidleAlignmentDegreesPerSecond * Time.deltaTime), 0.0f);
            transform.position = Vector3.MoveTowards(transform.position, manager.sidleWallEntryPoint + wallForward * manager.selfCapsuleCollider.radius, sidleAlignmentTranslationPerSecond * Time.deltaTime );

            input = Vector3.zero;
        }

        Vector3 worldWishDir = Quaternion.Euler(0, Quaternion.LookRotation(-wallForward, Vector3.up).eulerAngles.y, 0) * input;
        velocity = MoveGround(worldWishDir, velocity, groundSidleAcceleration, groundSidleMaxSpeed);
        Vector3 nextPosition = transform.position + velocity * Time.deltaTime;
        Vector3 delta = nextPosition - transform.position;

        Ray ray = new Ray(transform.position + velocity.normalized * sidleEdgeLimit, -wallForward);

        Debug.DrawRay(ray.origin, ray.direction * sidleValidQueryLength, Color.yellow, 0.0f);
        var continuousSidleCandidates = Physics.RaycastAll(ray, sidleValidQueryLength, manager.enviromentLayerMask, QueryTriggerInteraction.Ignore);

        bool canMove = false;
        foreach(var candidate in continuousSidleCandidates)
        {
            if(candidate.collider != manager.sidleWallCollider) { continue; }
            canMove = true;
            break;
        }

        if(canMove)
        {
            animator.SetBool("isSidle", true);
            animator.SetFloat("Speed", velocity.magnitude);

            charController.Move(delta);
        }
    }

    public void Idle(Vector3 idleDir)
    {
        animator.SetFloat("Speed", 0.0f);
    }

    public void StartCrouch()
    {
        animator.SetBool("isCrouching", true);
        crouchWish = true;
    }

    public void StopCrouch()
    {

        animator.SetBool("isCrouching", false);
        crouchWish = false;
        crouchTimer = 0.0f;
    }

    void doRock()
    {

    }

    public void doTakedown(GameObject enemy, System.Action exitCallback)
    {

        animator.SetTrigger("Takedown");
        //LEGIT JUST TO SIMULATE TAKING DOWN
        DELETEME += Time.deltaTime;
        if (DELETEME >= 2/3)
        {
            GameObject.Destroy(enemy);
            exitCallback();
        }

    }

    public void doKnock(System.Action exitCallback)
    {
        //DO SHIT
        animator.SetTrigger("Knocked");
        exitCallback();
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

    private Vector3 Impulse(Vector3 impulse, Vector3 prevVelocity)
    {
        return prevVelocity + impulse;
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
        bool wasGrounded = grounded;
        grounded = PerformGroundCheck(transform.position, groundCheckLength, out groundCol, out groundNorm);
        crouchTimer += (crouchWish ? 1.0f : 0.0f) * Time.deltaTime;
        charController.height = coll.height = Mathf.Lerp(standHeight, crouchHeight, crouchProgress);
        charController.center = coll.center = Vector3.up * (charController.height - 2) / 2;

        if(!wasGrounded && grounded)
        {
            velocity.y = 0.0f;
        }
        if(!grounded)
        {
            velocity = Accelerate(Vector3.down, velocity, 9.8f, 53.0f );
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * groundCheckLength);

        if(manager.playerState == PlayerManager.PLAYER_STATE.SIDLE)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(transform.position, sidleSurfaceNormal * 10.0f);
        }
    }

    void Reset()
    {
        charController = GetComponent<CharacterController>();
        coll = GetComponent<CapsuleCollider>();
    }

    public void OnStateChanged(AnimatorEventInfo eventInfo)
    {

    }
}
