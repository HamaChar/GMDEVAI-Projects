using UnityEngine;

public class Pursuer : AIAgent
{
    void Update()
    {
        if (!agent.isOnNavMesh) return;

        if (TargetInRange())
            Pursue();
        else
            Wander();
    }
}
