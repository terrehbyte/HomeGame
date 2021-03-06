﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour
{
    public AudioLord audioLord;

    public bool showTriggers;

    public PLAYER_STATE playerState;
    public PLAYER_ACTION playerAction;
    public ARMBAND_STATE armbandState;
    public bool isCrouching;

    [Range(0.0f,1.0f)]
    public float runLimit;

    [SerializeField]
    private PlayerMotor playerMotor;
    [SerializeField]
    private InputManager inputManager;

    [Header("ARMBAND SHIT")]

    [Header("Danger Range")]
    public float zone1Radius;
    [Header("Semi-Danger Range")]
    public float zone2Radius;
    [Header("ADD MATERIALS")]
    public Material armbandMatActive;
    public Material armbandMatInactive;
    public Material armbandMatAttack;
    [Header("Blink Speeds")]
    public float slowBlinkSpeed;
    public float medBlinkSpeed;
    public float highBlinkSpeed;
    public float blinkSpeed;

    [ReadOnlyField]
    public bool canMove = true;
    [ReadOnlyField]
    public bool canWalk = true;
    [ReadOnlyField]
    public bool canRun = true;
    [ReadOnlyField]
    public bool canCrouch = true;
    [ReadOnlyField]
    public bool canKnock = false;
    [ReadOnlyField]
    public bool canThrowRock = false;
    [ReadOnlyField]
    public bool canTakeDown = false;

    public Cinemachine.CinemachineFreeLook gameCamera;


    [Header("Sidle Shit")]
    public float sidleAngleThreshold = 15.0f;
    public float sidleProximity = 1.0f;
    public bool autoSidle;
    public LayerMask enviromentLayerMask;
    public float triggerCapsuleRadiusOffset;
    public LayerMask enemyLayerMask;
    //For sidle detection
    public Vector3 sidleWallNormal { get; private set; }
    public Collider sidleWallCollider { get; private set; }
    public Vector3 sidleWallEntryPoint { get; private set; }
    public Transform sidleRaycastOrigin;

    [Header("Takedown Shit")]
    //For Enemy Detection
    private int numEnemiesNearPlayer;
    private Collider[] enemiesNearPlayer = new Collider[1];
    public CapsuleCollider selfCapsuleCollider;
    //For Detecting Enemys Above
    private int numEnemiesAbovePlayer;
    private Collider[] enemiesAbovePlayer = new Collider[1];

    
    [ReadOnlyField]
    public PLAYER_STATE previousPlayerState;
    private bool crouched = false;
    private MeshRenderer armbandMeshRenderer;
    private float blinkTime;
    private float blinkTime2;

    //GAMEPAD SHIT

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
        TAKEDOWN,
        WAKINGUP
    }
    public enum ARMBAND_STATE
    {
        SAFE,
        SEMIDANGER,
        DANGER,
        ATTACK
    }

    void Start()
    {
        playerAction = PLAYER_ACTION.WAKINGUP;
        canMove = false;

        selfCapsuleCollider = this.GetComponent<CapsuleCollider>();
        armbandMeshRenderer = GameObject.FindGameObjectWithTag("Armband").GetComponent<MeshRenderer>();
        playerMotor = this.GetComponent<PlayerMotor>();
        inputManager = this.GetComponent<InputManager>();
    }

    void Update()
    {
        if(inputManager.gamepadStart)
        {
            GameState.instance.TriggerPlayerDeath();
        }

        blinkTime += Time.deltaTime;
        blinkTime2 += Time.deltaTime;

        if (canMove == false)
        {
            inputManager.input = new Vector3();
        }
        else
        {
            UpdatePlayerState();
            UpdatePlayerAction();
        }

        UpdatePlayerArmband();

        if(!inputManager.blockInput)
        {
            gameCamera.m_XAxis.m_InputAxisName = "Mouse X";
            gameCamera.m_YAxis.m_InputAxisName = "Mouse Y";
        }
        else
        {
            gameCamera.m_XAxis.m_InputAxisName = string.Empty;
            gameCamera.m_YAxis.m_InputAxisName = string.Empty;
        }

        switch (playerState)
        {
            case PLAYER_STATE.STILL:
                playerMotor.Idle(inputManager.input);
                break;
            case PLAYER_STATE.WALK:
                if (canMove == true)
                {
                playerMotor.Walk(inputManager.input);
                }
                break;
            case PLAYER_STATE.RUN:
                if (canMove == true)
                {
                    playerMotor.Run(inputManager.input);
                }
                break;
            case PLAYER_STATE.SIDLE:
                playerMotor.Sidle(inputManager.input, sidleWallNormal, StopSidle);
                break;
        }
        if (isCrouching == true)
        {
            if (crouched == false)
            {
                playerMotor.StartCrouch();
                crouched = true;
            }
        }
        else if (isCrouching == false)
        {
            if (crouched == true)
            {
                playerMotor.StopCrouch();
                crouched = false;
            }
        }

        switch (playerAction)
        {
            case PLAYER_ACTION.NOACTION:
                break;
            case PLAYER_ACTION.TAKEDOWN:
                playerMotor.doTakedown(enemiesAbovePlayer[0].gameObject, StopTakeDown);
                break;
            case PLAYER_ACTION.KNOCK:
                playerMotor.doKnock(StopKnock);
                break;
            case PLAYER_ACTION.THROWROCK:
                Debug.Log("THIS WAS SUPPSED TO BE THROW ROCK LOL");
                break;
            case PLAYER_ACTION.WAKINGUP:
                playerMotor.doWakeUp(StopAwake);
                break;
        }

        switch (armbandState)
        {
            case ARMBAND_STATE.SAFE:
                if (blinkTime >= 1 / slowBlinkSpeed)
                {
                    Debug.Log("NOBLINNK");
                    blinkTime = 0.0f;
                    blinkTime2 = 0.0f;
                    audioLord.BlinkCaller();

                    armbandMeshRenderer.materials[1].SetColor("_Color", armbandMatActive.GetColor("_Color"));
                    armbandMeshRenderer.materials[1].SetColor("_EmissionColor", armbandMatActive.GetColor("_EmissionColor"));
                }
                if (blinkTime2 >= 1 / blinkSpeed)
                {
                    
                    Debug.Log("BLINNK");
                    armbandMeshRenderer.materials[1].SetColor("_Color", armbandMatInactive.GetColor("_Color"));
                    armbandMeshRenderer.materials[1].SetColor("_EmissionColor", armbandMatInactive.GetColor("_EmissionColor"));

                }
                break;
            case ARMBAND_STATE.SEMIDANGER:
                if (blinkTime >= 1 / medBlinkSpeed)
                {
                    blinkTime = 0.0f;
                    blinkTime2 = 0.0f;
                    audioLord.BlinkCaller();
                    armbandMeshRenderer.materials[1].SetColor("_Color", armbandMatActive.GetColor("_Color"));
                    armbandMeshRenderer.materials[1].SetColor("_EmissionColor", armbandMatActive.GetColor("_EmissionColor"));
                }
                if (blinkTime2 >= 1 / blinkSpeed)
                {
                    armbandMeshRenderer.materials[1].SetColor("_Color", armbandMatInactive.GetColor("_Color"));
                    armbandMeshRenderer.materials[1].SetColor("_EmissionColor", armbandMatInactive.GetColor("_EmissionColor"));
                }
                break;
            case ARMBAND_STATE.DANGER:
                if (blinkTime >= 1 / highBlinkSpeed)
                {
                    blinkTime = 0.0f;
                    blinkTime2 = 0.0f;
                    audioLord.BlinkCaller();
                    armbandMeshRenderer.materials[1].SetColor("_Color", armbandMatActive.GetColor("_Color"));
                    armbandMeshRenderer.materials[1].SetColor("_EmissionColor", armbandMatActive.GetColor("_EmissionColor"));
                }
                if (blinkTime2 >= 1 / blinkSpeed)
                {
                    armbandMeshRenderer.materials[1].SetColor("_Color", armbandMatInactive.GetColor("_Color"));
                    armbandMeshRenderer.materials[1].SetColor("_EmissionColor", armbandMatInactive.GetColor("_EmissionColor"));
                }
                break;
            case ARMBAND_STATE.ATTACK:
                armbandMeshRenderer.materials[1].SetColor("_Color", armbandMatAttack.GetColor("_Color"));
                armbandMeshRenderer.materials[1].SetColor("_EmissionColor", armbandMatAttack.GetColor("_EmissionColor"));
                break;
        }

        void StopSidle()
        {
            canMove = true;

            canWalk = true;
            canRun = true;
            canKnock = false;
            playerState = PLAYER_STATE.STILL;
            previousPlayerState = PLAYER_STATE.SIDLE;
        }

        void StopTakeDown()
        {
            canWalk = true;
            canRun = true;
            canCrouch = true;
            canMove = true;
            playerAction = PLAYER_ACTION.NOACTION;
        }

        void StopKnock()
        {
            playerAction = PLAYER_ACTION.NOACTION;
            canMove = true;
            canCrouch = true;
        }

        void StopAwake()
        {
            playerAction = PLAYER_ACTION.NOACTION;
            canMove = true;
        }
    }

    RaycastHit[] sidleCandidates;

    private void UpdatePlayerState()
    {
        sidleCandidates = Physics.RaycastAll(sidleRaycastOrigin.position, transform.forward, sidleProximity, enviromentLayerMask, QueryTriggerInteraction.Ignore);


        if (inputManager.input.magnitude == 0.0f)
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

        if (canWalk == true && inputManager.input.magnitude != 0.0f)
        {
            if (playerState != PLAYER_STATE.WALK)
            {
                previousPlayerState = playerState;
                playerState = PLAYER_STATE.WALK;
            }

            if ((Input.GetKey(KeyCode.LeftShift) || inputManager.input.magnitude >= runLimit) && canRun == true)
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

        if ((Input.GetKey(KeyCode.C) || inputManager.gamepadA) && canCrouch == true)
        {
            if (isCrouching == false)
            {
                //redundant but for saftey
                canKnock = false;
                canRun = false;
                isCrouching = true;
            }
            if (isCrouching == true)
            {
                canKnock = false;
                canRun = false;
            }
        }
        else
        {
            if(playerState == PLAYER_STATE.SIDLE)
            {
                canKnock = true;
            }

            isCrouching = false;
            canRun = true;
        }

        if (sidleCandidates.Length > 0)
        {
            if (autoSidle == true)
            {
                if (playerState != PLAYER_STATE.SIDLE)
                {
                    playerState = PLAYER_STATE.SIDLE;
                    canKnock = true;
                    //canThrowRock = true;
                    canWalk = false;
                    canRun = false;

                    sidleWallNormal = sidleCandidates[0].normal;
                    sidleWallCollider = sidleCandidates[0].collider;
                    sidleWallEntryPoint = sidleCandidates[0].point;
                }
            }
        }
    }

    private void UpdatePlayerAction()
    {
        //Shit for take down
        numEnemiesAbovePlayer = Physics.OverlapCapsuleNonAlloc(transform.position,
                                new Vector3(transform.position.x, transform.position.y + 1,
                                transform.position.z), 1, enemiesAbovePlayer, enemyLayerMask);

        if (numEnemiesAbovePlayer <= 0)
        {
            canTakeDown = false;
        }
        else if (/*enemiesAbovePlayer[0].gameObject.getComponent<script>().enemyState == 
            enemiesAbovePlayer[0].gameObject.getComponent<script>().ENEMY_STATE.PATROL*/ true)
        {
            canTakeDown = true;
        }

        //Take down
        if ((Input.GetKeyDown(KeyCode.L) || inputManager.gamepadX) && canTakeDown == true && playerAction == PLAYER_ACTION.NOACTION)
        {
            canMove = false;
            playerAction = PLAYER_ACTION.TAKEDOWN;
        }

        //J for knock 
        if ((Input.GetKeyDown(KeyCode.J) || inputManager.gamepadB) && canKnock == true)
        {
            if (playerAction != PLAYER_ACTION.KNOCK)
            {             
                playerAction = PLAYER_ACTION.KNOCK;
                canCrouch = false;
                canMove = false;
                //TURN THESE BACK ON AFTER THE ANIM
            }
        }

        ////K for throwing a rock
        //if ((Input.GetKey(KeyCode.K) || inputManager.gamepadY) && canThrowRock == true)
        //{
        //    if (playerAction != PLAYER_ACTION.THROWROCK)
        //    {
        //        playerAction = PLAYER_ACTION.THROWROCK;
        //        canWalk = false;
        //        canRun = false;
        //        canThrowRock = false;
        //        canTakeDown = false;
        //        canMove = false;
        //    }
        //}
    }

    private void UpdatePlayerArmband()
    {
        numEnemiesNearPlayer = Physics.OverlapSphereNonAlloc(transform.position, zone2Radius, enemiesNearPlayer, enemyLayerMask);
        if (numEnemiesNearPlayer == 0)
        {
            armbandState = ARMBAND_STATE.SAFE;
        }
        else
        {
            for (int i = 0; i < numEnemiesNearPlayer; i++)
            {
                float distanceFromEnemy = Vector3.Distance(enemiesNearPlayer[i].transform.position, transform.position);

                if (distanceFromEnemy <= zone1Radius)
                {
                    armbandState = ARMBAND_STATE.DANGER;
                }
                else
                {
                    armbandState = ARMBAND_STATE.SEMIDANGER;
                }
            }
        }

        if (canTakeDown == true || playerAction == PLAYER_ACTION.TAKEDOWN)
        {
            armbandState = ARMBAND_STATE.ATTACK;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(sidleRaycastOrigin.position, transform.forward * sidleProximity);
    }
}

[System.Serializable]
public class UnityStringEvent : UnityEvent<string>
{

}