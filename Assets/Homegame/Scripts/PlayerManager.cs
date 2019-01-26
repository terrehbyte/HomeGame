using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{

    public PLAYER_STATE playerState;
    public PLAYER_ACTION playerAction;
    public ARMBAND_STATE armbandState;
    public bool isCrouching;


    public bool autoSidle;
    public LayerMask selfCapsuleLayerMask;
    public float triggerCapsuleRadiusOffset;

    public bool canMove = true;
    [ReadOnlyField]
    public bool canWalk = true;
    [ReadOnlyField]
    public bool canRun = true;
    [ReadOnlyField]
    public bool canCrouch = true;
    [ReadOnlyField]
    public bool canSidle = false;
    [ReadOnlyField]
    public bool canKnock = false;
    [ReadOnlyField]
    public bool canThrowRock = false;
    [ReadOnlyField]
    public bool canTakeDown = false;

    [SerializeField]
    private int numObjectsNearPlayer;
    private Collider[] objectsNearPlayer = new Collider[1];
    private Vector3 triggerCapsuleTop;
    private Vector3 triggerCapsuleBot;

    private CapsuleCollider selfCapsuleCollider;


    [Header("Input")]
    private Vector3 input;
    [ReadOnlyField]
    public PLAYER_STATE previousPlayerState;

    public enum PLAYER_STATE
    {
        STILL,
        WALK,
        RUN,
        SIDLE
    }
    public enum PLAYER_ACTION
    {
        NOACTION,
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
        if (canMove == false)
        {
            //cant move
        }
        else
        {
            triggerCapsuleTop = new Vector3(transform.position.x, transform.position.y + 0.75f, transform.position.z);
            triggerCapsuleBot = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
            //note for future, this can also take in a QueryTriggerInteraction to descide if it works on triggers
            numObjectsNearPlayer = Physics.OverlapCapsuleNonAlloc(triggerCapsuleTop, triggerCapsuleBot, selfCapsuleCollider.radius * transform.localScale.x + triggerCapsuleRadiusOffset, objectsNearPlayer, selfCapsuleLayerMask);


            input = new Vector3(Input.GetAxisRaw("Horizontal"),
                                       0.0f,
                                       Input.GetAxisRaw("Vertical"));

            if (input.magnitude == 0.0f)
            {
                if (playerState != PLAYER_STATE.STILL && playerState != PLAYER_STATE.SIDLE)
                {
                    previousPlayerState = playerState;
                    playerState = PLAYER_STATE.STILL;
                }
            }
            else if (canWalk == true)
            {
                if (playerState != PLAYER_STATE.WALK)
                {
                    previousPlayerState = playerState;
                    playerState = PLAYER_STATE.WALK;
                }

                if (Input.GetKey(KeyCode.LeftShift) && canRun == true)
                {
                    if (playerState != PLAYER_STATE.RUN)
                    {
                        previousPlayerState = playerState;
                        playerState = PLAYER_STATE.RUN;
                    }
                    else
                    {
                        if (playerState == PLAYER_STATE.RUN)
                        {
                            playerState = previousPlayerState;
                            previousPlayerState = PLAYER_STATE.RUN;
                        }
                    }
                }
                else
                {
                    if (playerState == PLAYER_STATE.RUN)
                    {
                        playerState = previousPlayerState;
                        previousPlayerState = PLAYER_STATE.RUN;
                    }
                }
            }

            if (Input.GetKey(KeyCode.C) && canCrouch == true)
            {
                if (isCrouching == false)
                {
                    //redundant but for saftey
                    canRun = false;
                    isCrouching = true;
                }
            }
            else
            {
                if (isCrouching == true)
                {
                    canRun = true;
                    isCrouching = false;
                }
            }


            if (numObjectsNearPlayer >= 1)
            {
                if (autoSidle == true)
                {
                    if (playerState != PLAYER_STATE.SIDLE)
                    {
                        playerState = PLAYER_STATE.SIDLE;
                        canWalk = false;
                        canRun = false;
                    }
                }
                else if (/* TO DO buttonisPressed*/ true)
                {
                    playerState = PLAYER_STATE.SIDLE;
                    //sidle;
                }
            }
            else
            {
                if (playerState == PLAYER_STATE.SIDLE)
                {
                    playerState = previousPlayerState;
                    previousPlayerState = PLAYER_STATE.SIDLE;
                    canWalk = true;
                    canRun = true;
                }
            }


        }

        /*
         * controller input, wall normal vec3
         */






    }

}
