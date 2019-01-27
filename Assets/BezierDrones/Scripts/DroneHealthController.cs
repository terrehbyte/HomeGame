using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneHealthController : MonoBehaviour {
    public float Health = 10f;

	// Use this for initialization
	void Start () {
	}

    public void getHit(float damage)
    {
        if (Health <= 0)
        {
            return;
        }
        Health -= damage;
        if (Health <= 0)
        {
            GetComponent<SplineWalker>().enabled = false;
        }
    }
}
