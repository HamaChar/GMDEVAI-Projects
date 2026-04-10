using UnityEngine;

public class AgentManager : MonoBehaviour
{
    public Transform player;
    public float stoppingDistance = 3.0f;

    GameObject[] agents;

    void Start()
    {
        agents = GameObject.FindGameObjectsWithTag("AI");
    }

    void Update()
    {
        if (player == null) return;

        foreach (GameObject ai in agents)
        {
            AIControlM4 aiControl = ai.GetComponent<AIControlM4>();
            if (aiControl != null && aiControl.agent != null)
            {
                aiControl.agent.stoppingDistance = stoppingDistance;
                aiControl.agent.SetDestination(player.position);
            }
        }
    }
}
