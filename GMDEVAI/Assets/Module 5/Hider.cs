using UnityEngine;

public class Hider : AIAgent
{
    void Update()
    {
        if (!agent.isOnNavMesh) return;

        if (TargetInRange())
            print("in range");
        else
            Wander();
    }
}
