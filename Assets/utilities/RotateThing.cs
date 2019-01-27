using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateThing : MonoBehaviour
{
    bool shouldRotate = true;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (shouldRotate)
        {
            transform.Rotate(0, 5f, 0);
        }
    }






}
