using UnityEngine;

public class Hider : AIAgent
{
    void Update()
    {
        if (!agent.isOnNavMesh) return;

        if (TargetInRange() && CanSeeTarget())
            CleverHide();
        else
            Wander();
    }
}
