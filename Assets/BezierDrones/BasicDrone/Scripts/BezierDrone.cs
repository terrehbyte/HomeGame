using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BezierDrone : MonoBehaviour
{

    public BezierSpline spline;
    public bool lookForward;
    public float loopDuration;
    public float progress;

    public Vector3 pointOfInterest;


    Rigidbody rb;


    SenseBase[] senses;
    [SerializeField] public int indexToChange;

    private void Awake()
    {
        if (spline)
        {
            spline = GetComponentInParent<BezierSpline>();
        }
        rb = GetComponent<Rigidbody>();
        StartCoroutine(Patrol());
    }

    private void Update()
    {
        //Old Patrol() //depriciated
    }

  
    /*
    void OldPatrol()
    {
        progress += Time.deltaTime / loopDuration;
        if (progress > 1f)
        {

            progress -= 1f;

        }

        AdjustPath(indexToChange);
        hammerPosition();
    }
    */


    IEnumerator Patrol()
    {
        float startTime = Time.time;

        while (true)
        {


            progress += (Time.deltaTime / loopDuration);
            if (progress > 1f)
            {

                progress -= 1f;

            }

            AdjustPath(indexToChange);
            FacePosition();

            yield return null;

        }
    }

    public virtual void FacePosition()
    {

        Vector3 position = spline.GetPoint(progress);
        transform.localPosition = position;
        if (lookForward)
        {
            transform.LookAt(position + spline.GetDirection(progress));
        }
    }



    public void AdjustPath(int nextPoint)
    {
        spline.points[nextPoint].x += Input.GetAxis("Horizontal");
        spline.points[nextPoint].y += Input.GetAxis("Vertical");
    }


    public void OnDisable()
    {
        //convert spline velocity to rigidbody
        rb.velocity = spline.GetVelocity(progress);
        //set appropriete physics drivers
        rb.isKinematic = false;
        rb.useGravity = true;

    }

    public void OnEnable()
    {

        rb.isKinematic = true;
        rb.useGravity = false;
    }


    public void Investigate()
    {
        GameObject go = new GameObject();
        BezierSpline newPath = go.AddComponent<BezierSpline>();
        newPath.Loop = false;
        newPath.points = new Vector3[4];
        newPath.modes = new BezierControlPointMode[newPath.points.Length];

        newPath.points[0] = spline.GetPoint(progress);
        newPath.modes[0] = BezierControlPointMode.Mirrored;
        newPath.points[1] = pointOfInterest;
        newPath.modes[1] = BezierControlPointMode.Mirrored;
        newPath.points[2] = spline.points[2];
        newPath.modes[2] = BezierControlPointMode.Mirrored;
        //move bot to new spline
        spline = newPath;
        progress = 0;

    }



}
