using UnityEngine;

public class Evader : AIAgent
{
    void Update()
    {
        if (!agent.isOnNavMesh) return;

        if (TargetInRange())
            Evade();
        else
            Wander();
    }
}
