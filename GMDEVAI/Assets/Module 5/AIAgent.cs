using UnityEngine;
using UnityEngine.AI;

public abstract class AIAgent : MonoBehaviour
{
    [SerializeField] protected GameObject target;
    [SerializeField] protected float range = 8f;

    protected NavMeshAgent agent;
    protected WASDMovement playerMovement;
    protected Vector3 wanderTarget;

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player");

        if (target != null)
            playerMovement = target.GetComponent<WASDMovement>();
    }

    protected void Seek(Vector3 location)
    {
        agent.SetDestination(location);
    }

    protected void Flee(Vector3 location)
    {
        Vector3 fleeDirection = location - this.transform.position;
        agent.SetDestination(this.transform.position - fleeDirection);
    }

    protected void Pursue()
    {
        Vector3 targetDirection = target.transform.position - this.transform.position;
        float lookAhead = targetDirection.magnitude / (agent.speed + playerMovement.currentSpeed);
        Seek(target.transform.position + target.transform.forward * lookAhead);
    }

    protected void Evade()
    {
        Vector3 targetDirection = target.transform.position - this.transform.position;
        float lookAhead = targetDirection.magnitude / (agent.speed + playerMovement.currentSpeed);
        Flee(target.transform.position + target.transform.forward * lookAhead);
    }

    protected void Wander()
    {
        float wanderRadius = 20;
        float wanderDistance = 10;
        float wanderJitter = 1;

        wanderTarget += new Vector3(Random.Range(-1.0f, 1.0f) * wanderJitter,
                                    0,
                                    Random.Range(-1.0f, 1.0f) * wanderJitter);

        wanderTarget.Normalize();
        wanderTarget *= wanderRadius;

        Vector3 targetLocal = wanderTarget + new Vector3(0, 0, wanderDistance);
        Vector3 targetWorld = this.gameObject.transform.InverseTransformVector(targetLocal);

        Seek(targetWorld);
    }

    protected void Hide()
    {
        Debug.Log("hiding");
        float distance = Mathf.Infinity;
        Vector3 chosenSpot = Vector3.zero;

        int hidingSpotsCount = World.Instance.GetHidingSpots().Length;

        for (int i = 0; i < hidingSpotsCount; i++)
        {
            Vector3 hideDirection = World.Instance.GetHidingSpots()[i].transform.position - target.transform.position;
            Vector3 hidePosition = World.Instance.GetHidingSpots()[i].transform.position + hideDirection.normalized * 5;

            float spotDistance = Vector3.Distance(this.transform.position, hidePosition);
            if (spotDistance < distance)
            {
                chosenSpot = hidePosition;
                distance = spotDistance;
            }
        }
        Seek(chosenSpot);
    }

    protected void CleverHide()
    {
        float distance = Mathf.Infinity;
        Vector3 chosenSpot = Vector3.zero;
        Vector3 chosenDir = Vector3.zero;
        GameObject chosenGameObject = World.Instance.GetHidingSpots()[0];

        int hidingSpotsCount = World.Instance.GetHidingSpots().Length;

        for (int i = 0; i < hidingSpotsCount; i++)
        {
            Vector3 hideDirection = World.Instance.GetHidingSpots()[i].transform.position - target.transform.position;
            Vector3 hidePosition = World.Instance.GetHidingSpots()[i].transform.position + hideDirection.normalized * 5;

            float spotDistance = Vector3.Distance(this.transform.position, hidePosition);
            if (spotDistance < distance)
            {
                chosenSpot = hidePosition;
                chosenDir = hideDirection;
                chosenGameObject = World.Instance.GetHidingSpots()[i];
                distance = spotDistance;
            }
        }

        Collider hideCol = chosenGameObject.GetComponent<Collider>();
        Ray back = new Ray(chosenSpot, -chosenDir.normalized);
        RaycastHit info;
        float rayDistance = 100.0f;
        hideCol.Raycast(back, out info, rayDistance);

        Seek(info.point + chosenDir.normalized * 5);
    }

    protected bool CanSeeTarget()
    {
        RaycastHit raycastInfo;
        Vector3 rayToTarget = target.transform.position - this.transform.position;
        if (Physics.Raycast(this.transform.position, rayToTarget, out raycastInfo))
        {
            return raycastInfo.transform.gameObject.tag == "Player";
        }
        return false;
    }

    protected bool TargetInRange()
    {
        return Vector3.Distance(this.transform.position, target.transform.position) <= range;
    }
}
