using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SplineWalkerBase : MonoBehaviour
{
    public BezierSpline spline;
    public bool lookForward;
    public float duration;
    public float progress;
    public SplineWalkerMode mode;
    protected bool goingForward = false; // true in origianl tutorial


    public virtual void hammerPosition()
    {
        if (goingForward = true && spline != null)
        {
            Vector3 position = spline.GetPoint(progress);
            transform.localPosition = position;
            if (lookForward)
            {
                transform.LookAt(position + spline.GetDirection(progress));
            }
        }
    }

    public void OnDisable()
    {
        //convert spline velocity to rigidbody
        gameObject.GetComponent<Rigidbody>().velocity = spline.GetVelocity(progress);
        //set appropriete physics drivers
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        gameObject.GetComponent<Rigidbody>().useGravity = true;

    }

    public void OnEnable()
    {

        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        gameObject.GetComponent<Rigidbody>().useGravity = false;
    }




}
