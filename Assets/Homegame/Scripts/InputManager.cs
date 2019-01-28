using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    public Vector3 input;
    public Vector3 secondaryInput;

    public bool gamepadA;
    public bool gamepadB;
    public bool gamepadX;
    public bool gamepadY;
    public bool gamepadStart;

    public bool firstInput = false;

    [SerializeField]
    private bool _blockInput;
    public bool blockInput
    {
        get
        {
            return _blockInput;
        }
        set
        {
            _blockInput = value;
        }
    }

    public UnityEvent OnFirstInput; 

    // Update is called once per frame
    void Update()
    {
        if(blockInput)
        {
            gamepadA = gamepadB = gamepadX = gamepadY = gamepadStart = false;
            input = secondaryInput = Vector3.zero;
            return;
        }

        input = new Vector3(Input.GetAxisRaw("Horizontal"),
                           0.0f,
                           Input.GetAxisRaw("Vertical"));

        secondaryInput = new Vector3(Input.GetAxisRaw("Mouse X"), 0.0f, Input.GetAxisRaw("Mouse Y"));

        gamepadA = Input.GetButton("GAMEPAD A");
        gamepadB = Input.GetButton("GAMEPAD B");
        gamepadX = Input.GetButton("GAMEPAD X");
        gamepadY = Input.GetButton("GAMEPAD Y");
        gamepadStart = Input.GetButton("STARTDEBUG");

        if(!firstInput)
        {
            bool inputDetected = gamepadA ||
                                 gamepadB ||
                                 gamepadX ||
                                 gamepadY;

            if(!inputDetected) return;
            
            OnFirstInput.Invoke();
            firstInput = true;
        }
    }
}
