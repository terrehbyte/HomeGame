using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairMovement : MonoBehaviour
{
    SkinnedMeshRenderer rend;
    [Header("number of animation curves must be at least the same size as the number of blendshapes")]
    [SerializeField] AnimationCurve[] curves;
    [Header("Pass the character speed to the frequency")]

    public float frequencyMultiplier = 1f;

    

    // Use this for initialization
    void Start()
    {
        rend = GetComponent<SkinnedMeshRenderer>();
        StartCoroutine(HairFlow());

    }


    IEnumerator HairFlow()
    {

        int j = curves.Length;
        while (true)
        {
            for (int i = 0; i < j; i++)
            {
                rend.SetBlendShapeWeight(i,  curves[i].Evaluate(Time.time * frequencyMultiplier) * 100);
            }
            yield return null;
        }
    }





}
