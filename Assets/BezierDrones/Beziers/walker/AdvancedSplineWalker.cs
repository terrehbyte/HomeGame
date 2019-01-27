using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedSplineWalker : SplineWalker
{
    [SerializeField] Rigidbody rb;
    bool isFollowingSpline = true;
    

    private void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
    }


    private void FixedUpdate()
    {
        if (isFollowingSpline)
        {
            if (goingForward == true)
            {
                progress += Time.deltaTime / duration;
                if (progress > 1f)
                {
                    if (mode == SplineWalkerMode.Once)
                    {
                        progress = 1f;
                    }
                    else if (mode == SplineWalkerMode.Loop)
                    {
                        progress -= 1f;
                    }
                    else
                    {
                        progress = 2f - progress;
                        goingForward = false;
                    }
                }
            }
            else
            {
                progress -= Time.fixedDeltaTime / duration;
                if (progress < 0f)
                {
                    progress = -progress;
                    goingForward = true;
                }

            }
            AdjustPath(indexToChange);
            hammerPosition();
        }
    }


    public override void hammerPosition()
    {

        if (goingForward = true && spline != null)
        {
            Vector3 position = spline.GetPoint(progress);
            transform.localPosition = position;
            if (lookForward)
            {

                transform.LookAt(transform.forward + spline.GetDirection(progress));
            }
        }
    }

    



}
