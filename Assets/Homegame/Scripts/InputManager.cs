using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Vector3 input;

    public bool gamepadA;
    public bool gamepadB;
    public bool gamepadX;
    public bool gamepadY;

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
    }
}
