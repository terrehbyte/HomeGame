using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BezierDrone : MonoBehaviour
{
    [SerializeField] SenseBase senses;

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


    DroneBehavior db = DroneBehavior.Patrolling;


    public Vector3 pointOfInterest;
    Rigidbody rb;

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
        pointOfInterest = senses.lastPosition;
        if (canInitiateSearch) {
            canInitiateSearch = false;
            SetInvestigatePath();
            db = DroneBehavior.Cation;
        }
    }



    public void SetInvestigatePath()
    {
        mainProgress = progress;
        loopDuration = (mainloopDuration / mainSpline.points.Length) * spline.points.Length;

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
        StopCoroutine("Patrol");
        Debug.Log("Partol Interupted");
        spline = newPath;
        progress = 0;

    }

    IEnumerator Patrol()
    {

        while (true)
        {
            switch (db)
            {
                case DroneBehavior.Patrolling:
                    progress += (Time.deltaTime / mainloopDuration);
                    //looping
                    if (progress > 1f)
                    {

                        progress -= 1f;

                    }
                    FacePosition();
                    break;
                case DroneBehavior.Cation:

                    progress += (Time.deltaTime / loopDuration);
                    if (progress >= 1f)
                    {
                        spline = mainSpline;
                        progress = mainProgress;
                        db = DroneBehavior.Patrolling;
                        canInitiateSearch = true;
                    }
                    break;
                case DroneBehavior.dead:
                    this.OnDisable();
                    break;
                default:
                    break;
            }


            //AdjustPath(indexToChange);
            FacePosition();

            yield return null;

        }
    }




}


enum DroneBehavior
{
    Patrolling,Cation, dead

}