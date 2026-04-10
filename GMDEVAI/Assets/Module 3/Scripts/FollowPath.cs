using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    Transform goal;
    float speed = 5.0f;
    float accuracy = 1.0f;
    float rotSpeed = 2.0f;

    public GameObject wpManager;
    GameObject[] wps;
    GameObject currentNode;
    int currentWaypointIndex = 0;
    Graph graph;
    WaypointManager wpm;

    // TODO: Update these WP names after placing waypoints in the scene
    Dictionary<string, string> destinations = new Dictionary<string, string>();

    void Start()
    {
        wpm = wpManager.GetComponent<WaypointManager>();
        graph = wpm.graph;
        wps = wpm.waypoints;

        destinations["Twin Mountains"]     = "WP011";
        destinations["Barracks"]           = "WP003";
        destinations["Command Center"]     = "WP004";
        destinations["Oil Refinery Pumps"] = "WP006"; //6
        destinations["Tankers"]            = "WP007"; //7
        destinations["Radar"]              = "WP010";
        destinations["Command Post"]       = "WP001";
        destinations["Middle of Map"]      = "WP012";
        destinations["Helipad"]            = "WP015";
    }

    void LateUpdate()
    {
        if (graph.getPathLength() == 0 || currentWaypointIndex == graph.getPathLength())
        {
            return;
        }

        currentNode = graph.getPathPoint(currentWaypointIndex);

        if (Vector3.Distance(graph.getPathPoint(currentWaypointIndex).transform.position,
                            transform.position) < accuracy)
        {
            currentWaypointIndex++;
        }

        if (currentWaypointIndex < graph.getPathLength())
        {
            goal = graph.getPathPoint(currentWaypointIndex).transform;

            Vector3 lookAtGoal = new Vector3(goal.position.x, transform.position.y, goal.position.z);
            Vector3 direction = lookAtGoal - this.transform.position;

            this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                                                    Quaternion.LookRotation(direction),
                                                    Time.deltaTime * rotSpeed);

            this.transform.Translate(0, 0, speed * Time.deltaTime);
        }
    }

    public void GoToDestination(string destinationName)
    {
        if (!destinations.ContainsKey(destinationName))
        {
            Debug.LogWarning("Unknown destination: " + destinationName);
            return;
        }

        graph.ResetNodes();

        GameObject startWP = GetNearestWaypoint(transform.position);
        GameObject endWP = GameObject.Find(destinations[destinationName]);

        if (startWP == null || endWP == null)
        {
            Debug.LogWarning("Could not find start or end waypoint for: " + destinationName);
            return;
        }

        graph.AStar(startWP, endWP);
        currentWaypointIndex = 0;
    }

    GameObject GetNearestWaypoint(Vector3 position)
    {
        GameObject nearest = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject wp in wps)
        {
            float dist = Vector3.Distance(position, wp.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearest = wp;
            }
        }

        return nearest;
    }

    void OnGUI()
    {
        int y = 10;
        string[] destNames = { "Twin Mountains", "Barracks", "Command Center",
                               "Oil Refinery Pumps", "Tankers", "Radar",
                               "Command Post", "Middle of Map", "Helipad" };

        foreach (string dest in destNames)
        {
            if (GUI.Button(new Rect(10, y, 200, 25), "Go to " + dest))
            {
                GoToDestination(dest);
            }
            y += 30;
        }
    }
}
