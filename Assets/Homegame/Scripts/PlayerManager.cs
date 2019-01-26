﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerManager : MonoBehaviour
{
    public PlayerMotor playerMotor;

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
    public Renderer selfRenderer;


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
        selfRenderer = this.GetComponent<Renderer>();
    }

    void Update()
    {


        if (canMove == false)
        {
            //cant move
        }
        else
        {
            UpdatePlayerState();
            UpdatePlayerAction();
        }

        switch (playerState)
        {
            case PLAYER_STATE.STILL:
                playerMotor.Idle(input);
                break;
            case PLAYER_STATE.WALK:
                playerMotor.Walk(input);
                break;
            case PLAYER_STATE.RUN:
                playerMotor.Run(input);
                break;
            case PLAYER_STATE.SIDLE:

                RaycastHit RayData;
                Physics.Raycast(transform.position, objectsNearPlayer[0].ClosestPoint(transform.position), out RayData, 2, selfCapsuleLayerMask);
                playerMotor.Sidle(input, RayData.normal, tempFunc);
                Debug.Log("raydata norm = " + RayData.normal);
                break;
        }

        void tempFunc()
        {

        }
    }

    private void UpdatePlayerState()
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
                canThrowRock = true;
            }
        }
        else if (playerState == PLAYER_STATE.STILL)
        {
            canThrowRock = false;
        }

        if (canWalk == true && input.magnitude != 0.0f)
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
                    canKnock = true;
                    canThrowRock = true;
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
        ////DELETE AFTER 
        //else
        //{
        //    if (playerState == PLAYER_STATE.SIDLE)
        //    {
        //        playerState = previousPlayerState;
        //        previousPlayerState = PLAYER_STATE.SIDLE;
        //        canKnock = false;
        //        canThrowRock = false;
        //        canWalk = true;
        //        canRun = true;
        //    }
        //}
    }

    private void UpdatePlayerAction()
    {
        //J for knock 
        if (Input.GetKeyDown(KeyCode.J) && canKnock == true)
        {
            if (playerAction != PLAYER_ACTION.KNOCK)
            {
                playerAction = PLAYER_ACTION.KNOCK;
                canThrowRock = false;
                canTakeDown = false;

                //TURN THESE BACK ON AFTER THE ANIM
            }
        }

        //K for throwing a rock
        if (Input.GetKey(KeyCode.K) && canThrowRock == true)
        {
            if (playerAction != PLAYER_ACTION.THROWROCK)
            {
                playerAction = PLAYER_ACTION.THROWROCK;
                canWalk = false;
                canRun = false;

                canThrowRock = false;
                canTakeDown = false;
                //TURN THESE ON AFTER THE ANIM
            }
        }
    }
}
