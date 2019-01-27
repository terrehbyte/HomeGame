using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DroneCommander : MonoBehaviour
{
    [SerializeField]
    NavMeshAgent[] agents;


    RaycastHit hit;
    // Use this for initialization
    void Start()
    {
        RayGun();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RayGun();
        }
    }


    void RayGun()
    {
        Debug.Log("Go");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 20f))
        {
            Debug.Log(hit.collider.gameObject.name);
            foreach (NavMeshAgent agent in agents)
            {
                agent.SetDestination(hit.point);
            }
        }
    }
}
