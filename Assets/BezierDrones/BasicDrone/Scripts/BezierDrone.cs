using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BezierDrone : MonoBehaviour
{

    public BezierSpline mainSpline;
    BezierSpline spline;
    public bool lookForward;
    public float mainloopDuration;
    float loopDuration;

    [SerializeField] bool canInitiateSearch = true;
    [SerializeField] float InvestigationCooldown = 4f;

    public float progress;
    private float mainProgress;
    [SerializeField]int returnPoint;


    public Vector3 pointOfInterest;


    Rigidbody rb;


    SenseBase[] senses;
    [SerializeField] public int indexToChange;

    private void Awake()
    {
        if (mainSpline != null)
        {
            spline = mainSpline;
        }
        loopDuration = mainloopDuration;
        rb = GetComponent<Rigidbody>();
        StartCoroutine(Patrol());
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


    //depriciated
    /*
    public void AdjustPath(int nextPoint)
    {
        spline.points[nextPoint].x += Input.GetAxis("Horizontal");
        spline.points[nextPoint].y += Input.GetAxis("Vertical");
    }
    */

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


    public void SpottedSomething()
    {
        if (canInitiateSearch) {
            canInitiateSearch = false;
            SetInvestigatePath();
            StartCoroutine(SearchCooldown());
        }
    }



    public void SetInvestigatePath()
    {
        mainProgress = progress;


        GameObject go = new GameObject();
        BezierSpline newPath = go.AddComponent<BezierSpline>();
        newPath.Loop = false;
        newPath.points = new Vector3[4]; //NEEDS 4 points
        newPath.modes = new BezierControlPointMode[newPath.points.Length];
        spline.GetLastPoint(progress);


        ////current point
        newPath.points[0] = spline.GetPoint(progress);  
        newPath.modes[0] = BezierControlPointMode.Mirrored;
        //current point
        newPath.points[1] = spline.GetPoint(spline.GetLastPoint(progress));
        newPath.modes[1] = BezierControlPointMode.Mirrored;
        //point of interest

        newPath.points[2] = pointOfInterest;
        newPath.modes[2] = BezierControlPointMode.Mirrored;
        spline.GetNextPoint(progress);
        //next point
        returnPoint = spline.GetNextPoint(progress);
        newPath.points[3] = spline.GetPoint(progress);
        newPath.modes[3] = BezierControlPointMode.Mirrored;

        //move bot to new spline
        StopCoroutine(Patrol());
        spline = newPath;
        progress = 0;
        StartCoroutine(Investigate());

    }

    IEnumerator Patrol()
    {
        StopCoroutine(Investigate());
        float startTime = Time.time;

        while (true)
        {


            progress += (Time.deltaTime / loopDuration);
            if (progress > 1f)
            {

                progress -= 1f;

            }

            //AdjustPath(indexToChange);
            FacePosition();

            yield return null;

        }
    }




    IEnumerator Investigate()
    {
        bool isInvestigating = true;
        float startTime = Time.time;
        loopDuration = (mainloopDuration / mainSpline.points.Length) * spline.points.Length;

        while (isInvestigating)
        {

            progress += (Time.deltaTime / loopDuration);
            if (progress >= 1f)
            {

                isInvestigating = false;
            }

            //AdjustPath(indexToChange);
            FacePosition();

            yield return null;

        }
        spline = mainSpline;
        progress = mainProgress;
        loopDuration = mainloopDuration;
        StartCoroutine(Patrol());



    }

    IEnumerator SearchCooldown()
    {
        yield return new WaitForSeconds(InvestigationCooldown);
        canInitiateSearch = true;
    }
    


    }
