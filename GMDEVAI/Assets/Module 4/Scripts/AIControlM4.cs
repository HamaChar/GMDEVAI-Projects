using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIControlM4 : MonoBehaviour
{
    public NavMeshAgent agent;

    public float separationRadius = 1.5f;
    public float playerSeparationRadius = 1.5f;
    public float separationForce = 3.0f;
    public float playerSeparationForce = 3.0f;

    static List<AIControlM4> allAgents = new List<AIControlM4>();
    Transform player;

    void OnEnable()  { allAgents.Add(this); }
    void OnDisable() { allAgents.Remove(this); }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        Vector3 separation = Vector3.zero;

        // Separate from other agents
        foreach (AIControlM4 other in allAgents)
        {
            if (other == this) continue;

            Vector3 diff = transform.position - other.transform.position;
            float dist = diff.magnitude;

            if (dist < separationRadius && dist > 0f)
                separation += diff.normalized * (1f - dist / separationRadius);
        }

        // Separate from player
        if (player != null)
        {
            Vector3 diff = transform.position - player.position;
            float dist = diff.magnitude;

            if (dist < playerSeparationRadius && dist > 0f)
                separation += diff.normalized * (1f - dist / playerSeparationRadius);
        }

        if (separation != Vector3.zero)
        {
            separation.y = 0f;
            agent.Move(separation * playerSeparationForce * Time.deltaTime);
        }
    }
}
