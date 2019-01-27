using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    public Vector3 input;

    public bool gamepadA;
    public bool gamepadB;
    public bool gamepadX;
    public bool gamepadY;

    public bool firstInput = false;

    public UnityEvent OnFirstInput; 

    // Update is called once per frame
    void Update()
    {
        input = new Vector3(Input.GetAxisRaw("Horizontal"),
                           0.0f,
                           Input.GetAxisRaw("Vertical"));
        gamepadA = Input.GetButton("GAMEPAD A");
        gamepadB = Input.GetButton("GAMEPAD B");
        gamepadX = Input.GetButton("GAMEPAD X");
        gamepadY = Input.GetButton("GAMEPAD Y");

        if(!firstInput)
        {
            bool inputDetected = input.magnitude > 0 ||
                                 gamepadA ||
                                 gamepadB ||
                                 gamepadX ||
                                 gamepadY;

            if(!inputDetected) return;
            
            OnFirstInput.Invoke();
            firstInput = true;
        }
    }
}
