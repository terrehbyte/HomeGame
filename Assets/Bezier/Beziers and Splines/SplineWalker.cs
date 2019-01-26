using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineWalker : MonoBehaviour {

	public BezierSpline spline;
	public bool lookForward;
	public float duration;
	public float progress;
	public SplineWalkerMode mode;
	private bool goingForward = false; // true in origianl tutorial




	private void Update() {
		
		if (goingForward == true) {
			progress += Time.deltaTime / duration;
			if (progress > 1f) {
				if (mode == SplineWalkerMode.Once) {
					progress = 1f;
				} else if (mode == SplineWalkerMode.Loop) {
					progress -= 1f;
				} else {
					progress = 2f - progress;
					goingForward = false;
				}
			}
		} else {
			progress -= Time.deltaTime / duration;
			if (progress < 0f) {
				progress = -progress;
				goingForward = true;
			}

		}
		hammerPosition ();

	}
		
	private void hammerPosition(){
		if (goingForward = true && spline != null) {
			Vector3 position = spline.GetPoint (progress);
			transform.localPosition = position;
			if (lookForward) {
				transform.LookAt (position + spline.GetDirection (progress));
			}
		}
	}

    public void OnDisable()
    {
		gameObject.GetComponent<Rigidbody> ().velocity = spline.GetVelocity (progress);
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        gameObject.GetComponent<Rigidbody>().useGravity = true;

    }

    public void OnEnable()
    {
		
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        gameObject.GetComponent<Rigidbody>().useGravity = false;
    }


}
