using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineWalker : SplineWalkerBase {
    [SerializeField]public int indexToChange;

    /*
    private void Awake()
    {
        StartCoroutine(Patrol());
    }

    */
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
        AdjustPath(indexToChange);
        hammerPosition ();

	}
    

    IEnumerator Patrol()
    {
        float startTime = Time.time;

        while (true) {
            if (goingForward == true)
            {
                progress += (Time.time % startTime) / duration;
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
                progress -= (Time.time % startTime) / duration;
                if (progress < 0f)
                {
                    progress = -progress;
                    goingForward = true;
                }

            }
            AdjustPath(indexToChange);
            hammerPosition();

            yield return null;
            
        }
    }

    public void AdjustPath(int nextPoint)
    {     spline.points[nextPoint].x += Input.GetAxis("Horizontal");
          spline.points[nextPoint].y += Input.GetAxis("Vertical");
    }



}


