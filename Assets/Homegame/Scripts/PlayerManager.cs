using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    public PLAYER_STATE playerState;
    public ARMBAND_STATE armbandState;

    public bool autoSidle;
    public LayerMask selfCapsuleLayerMask;
    public float triggerCapsuleRadiusOffset;


    [ReadOnlyField]
    public bool canWalk;
    [ReadOnlyField]
    public bool canRun;
    [ReadOnlyField]
    public bool canCrouch;
    [ReadOnlyField]
    public bool canSidle;
    [ReadOnlyField]
    public bool canKnock;
    [ReadOnlyField]
    public bool canThrowRock;
    [ReadOnlyField]
    public bool canTakeDown;

    [SerializeField]
    private int numObjectsNearPlayer;
    private Collider[] objectsNearPlayer = new Collider[1];
    private Vector3 triggerCapsuleTop;
    private Vector3 triggerCapsuleBot;

    private CapsuleCollider selfCapsuleCollider;

    [Header("Input")]
    public float horizontal;
    public float vertical;


    public enum PLAYER_STATE
    {
        STILL,
        WALK,
        RUN,
        CROUCH,
        SIDLE,
        KNOCK,
        THROWROCK,
        TAKEDOWN
    }

    public enum ARMBAND_STATE
    {
        OFF,
        BLINK,
        SOLID
    }

    void Start()
    {
        selfCapsuleCollider = this.GetComponent<CapsuleCollider>();
    }

    void Update()
    {

        /*
         * controller input, wall normal vec3
         */




    }

    void FixedUpdate()
    {
        triggerCapsuleTop = new Vector3(transform.position.x, transform.position.y + 0.75f, transform.position.z);
        triggerCapsuleBot = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);

        //note for future, this can also take in a QueryTriggerInteraction to descide if it works on triggers
        numObjectsNearPlayer = Physics.OverlapCapsuleNonAlloc(triggerCapsuleTop, triggerCapsuleBot, selfCapsuleCollider.radius * transform.localScale.x + triggerCapsuleRadiusOffset, objectsNearPlayer, selfCapsuleLayerMask);

        if (numObjectsNearPlayer >= 1)
        {
            if (autoSidle == true)
            {
                playerState = PLAYER_STATE.SIDLE;
                //sidle;
            }
            else if (/* TO DO buttonisPressed*/ true)
            {
                playerState = PLAYER_STATE.SIDLE;
                //sidle;
            }
        }




    }


}
