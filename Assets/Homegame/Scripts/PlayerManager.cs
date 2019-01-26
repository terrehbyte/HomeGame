using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int playerState;
    public int armbandState;

    public bool canWalk;
    public bool canRun;
    public bool canCrouch;
    public bool canSidle;
    public bool canKnock;
    public bool canThrowRock;
    public bool canTakeDown;


    
    private enum PLAYER_STATE
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

    private enum ARMBAND_STATE
    {
         OFF,
         BLINK,
         SOLID
    }

    void Start()
    {



    }


    void Update()
    {

    }
}
