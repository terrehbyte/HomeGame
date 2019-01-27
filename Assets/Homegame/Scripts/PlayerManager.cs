using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerManager : MonoBehaviour
{
    public PlayerMotor playerMotor;

    public PLAYER_STATE playerState;
    public PLAYER_ACTION playerAction;
    public ARMBAND_STATE armbandState;
    public bool isCrouching;

    public float sidleAngleThreshold = 15.0f;

    [Header("DANGER")]
    public float zone1Radius;
    [Header("Semi-Danger")]
    public float zone2Radius;

    public Color BlinkColor1;
    public Color BlinkColor2;

    public float slowBlinkSpeed;
    public float medBlinkSpeed;
    public float highBlinkSpeed;
    public float blinkSpeed;


    public bool autoSidle;
    public LayerMask enviromentLayerMask;
    public float triggerCapsuleRadiusOffset;

    public LayerMask enemyLayerMask;

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
    //FOR WALL DETECTION
    private int numWallsNearPlayer;
    private Collider[] wallsNearPlayer = new Collider[1];
    private Vector3 triggerCapsuleTop;
    private Vector3 triggerCapsuleBot;

    //For Enemy Detection
    private int numEnemiesNearPlayer;
    private Collider[] enemiesNearPlayer = new Collider[1];



    private CapsuleCollider selfCapsuleCollider;
    public Renderer selfRenderer;

    [Header("Input")]
    private Vector3 input;
    [ReadOnlyField]
    public PLAYER_STATE previousPlayerState;
    private bool crouched = false;
    private Material armbandMaterial;
    private float blinkTime;
    private float blinkTime2;

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
        SAFE,
        SEMIDANGER,
        DANGER
    }

    void Start()
    {
        selfCapsuleCollider = this.GetComponent<CapsuleCollider>();
        selfRenderer = this.GetComponent<Renderer>();
        armbandMaterial = GameObject.FindGameObjectWithTag("Armband").GetComponent<Renderer>().material;
    }

    void Update()
    {
        blinkTime += Time.deltaTime;
        blinkTime2 += Time.deltaTime;
        if (canMove == false)
        {
            //cant move
        }
        else
        {
            UpdatePlayerState();
            UpdatePlayerAction();
        }

        UpdatePlayerArmband();

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
                Physics.Raycast(transform.position, wallsNearPlayer[0].ClosestPoint(transform.position), out RayData, 2, enviromentLayerMask);
                playerMotor.Sidle(input, RayData.normal, tempFunc);
                Debug.Log("raydata norm = " + RayData.normal);
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

        switch (armbandState)
        {
            case ARMBAND_STATE.SAFE:
                if (blinkTime >= 1 / slowBlinkSpeed)
                {
                    blinkTime = 0.0f;
                    blinkTime2 = 0.0f;
                    armbandMaterial.color = BlinkColor1;
                }
                if(blinkTime2 >= 1/blinkSpeed)
                {
                    armbandMaterial.color = BlinkColor2;
                }
                break;
            case ARMBAND_STATE.SEMIDANGER:
                if (blinkTime >= 1 / medBlinkSpeed)
                {
                    blinkTime = 0.0f;
                    blinkTime2 = 0.0f;
                    armbandMaterial.color = BlinkColor1;
                }
                if (blinkTime2 >= 1 / blinkSpeed)
                {
                    armbandMaterial.color = BlinkColor2;
                }
                break;
            case ARMBAND_STATE.DANGER:
                if (blinkTime >= 1 / highBlinkSpeed)
                {
                    blinkTime = 0.0f;
                    blinkTime2 = 0.0f;
                    armbandMaterial.color = BlinkColor1;
                }
                if (blinkTime2 >= 1 / blinkSpeed)
                {
                    armbandMaterial.color = BlinkColor2;
                }
                break;
        }

        void tempFunc()
        {
            Debug.Log("exit sidle");
            canWalk = true;
            canRun = true;
            playerState = previousPlayerState;
            previousPlayerState = PLAYER_STATE.SIDLE;
        }
    }

    private void UpdatePlayerState()
    {
        triggerCapsuleTop = new Vector3(transform.position.x, transform.position.y + 0.75f, transform.position.z);
        triggerCapsuleBot = new Vector3(transform.position.x, transform.position.y + 0.75f, transform.position.z);
        //note for future, this can also take in a QueryTriggerInteraction to descide if it works on triggers
        numWallsNearPlayer = Physics.OverlapCapsuleNonAlloc(triggerCapsuleTop, triggerCapsuleBot, selfCapsuleCollider.radius * transform.localScale.x + triggerCapsuleRadiusOffset, wallsNearPlayer, enviromentLayerMask);


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


        if (numWallsNearPlayer >= 1)
        {
            Vector3 dirToWall = (wallsNearPlayer[0].transform.position - transform.position).normalized;
            Vector3 worldMove = playerMotor.ControllerToWorldDirection(input).normalized;

            float wallAttraction = 1 - Vector3.Dot(worldMove, dirToWall);

            Debug.Log("wallAttraction " + wallAttraction);
            if (autoSidle == true && (wallAttraction < sidleAngleThreshold))
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

    private void UpdatePlayerArmband()
    {
        numEnemiesNearPlayer = Physics.OverlapSphereNonAlloc(transform.position, zone2Radius, enemiesNearPlayer,enemyLayerMask);
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
    }
}
