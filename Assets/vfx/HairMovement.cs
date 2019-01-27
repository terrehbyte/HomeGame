using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairMovement : MonoBehaviour
{
    SkinnedMeshRenderer rend;
    [Header("number of animation curves must be at least the same size as the number of blendshapes")]
    [SerializeField] AnimationCurve[] curves;
    [Header("Pass the character speed to the frequency")]

    public float characterSpeed = 0f;
    [Range(.01f,10)]
    public float frequencyMultiplier = 1f; 
    

    // Use this for initialization
    void Start()
    {
        rend = GetComponent<SkinnedMeshRenderer>();
        StartCoroutine(HairFlow());

    }


    IEnumerator HairFlow()
    {
        float startTime = Time.time;

        while (true)
        {
            for (int i = 0; i < 3; i++)
            {
                rend.SetBlendShapeWeight(i, curves[i].Evaluate(Time.time % startTime * (1 + (characterSpeed * frequencyMultiplier))));
            }
        }
    }





}
