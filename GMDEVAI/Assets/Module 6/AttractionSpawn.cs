using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttractionSpawn : MonoBehaviour
{
    public GameObject attraction;
    GameObject[] agents;

    void Start()
    {
        agents = GameObject.FindGameObjectsWithTag("agent");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out hit))
            {
                Instantiate(attraction, hit.point, attraction.transform.rotation);
                foreach (GameObject a in agents)
                {
                    a.GetComponent<AIControlM6>().DetectNewAttraction(hit.point);
                }
            }
        }
    }
}
