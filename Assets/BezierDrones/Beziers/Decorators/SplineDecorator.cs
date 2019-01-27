using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineDecorator : MonoBehaviour {

	public BezierSpline spline;
	public int frequency;
	public bool lookForward;
	public Transform[] drones;


	private void Awake(){
		if (frequency <= 0 || drones == null || drones.Length == 0) {
			return;
		}
		float stepSize = frequency * drones.Length;
		if (spline.Loop || stepSize == 1) {
			stepSize = 1f / stepSize;
		} else {
			stepSize = 1f / (stepSize - 1);
		}
		for (int p = 0, f = 0; f < frequency; f++){
			for (int i = 0; i < drones.Length; i++, p++) {
				Transform item = Instantiate (drones [i]) as Transform;
                //custom
                SplineWalkerBase walker = item.GetComponent<SplineWalkerBase>();
                walker.spline = spline;
                //Spaceing along the bezier: f is incrementor, Don't devide by zero!!!!
                item.GetComponent<SplineWalkerBase>().progress = 1f/(frequency * (drones.Length +1)) * f ;
                Debug.Log("f" + f.ToString() + " is " + (stepSize * f).ToString());
                //Vector3 position = spline.GetPoint (p * stepSize);
				//item.transform.localPosition = position;
				if (lookForward) {
					item.transform.LookAt (spline.GetPoint(p * stepSize) + spline.GetDirection (p * stepSize));
				}
                walker = null;
            }
		}
	}

}
