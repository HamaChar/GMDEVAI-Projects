using UnityEngine;

public class PetController : MonoBehaviour
{
    [SerializeField] Transform avatarTransform;
    [SerializeField] AvatarController avatarController;

    [SerializeField] float moveSpeed      = 4f;
    [SerializeField] float acceleration   = 3f;
    [SerializeField] float followRadius   = 2f;
    [SerializeField] float arriveThreshold = 0.25f;
    [SerializeField] float leashDistance  = 4f;
    [SerializeField] float rotationSpeed  = 4f;

    enum PetState { Following, Offsetting, Aligning, Idle }

    PetState state = PetState.Following;
    Vector3 currentVelocity = Vector3.zero;
    Vector3 offsetTargetPos;

    void Update()
    {
        if (avatarTransform == null || avatarController == null) return;

        switch (state)
        {
            case PetState.Following:  UpdateFollowing();  break;
            case PetState.Offsetting: UpdateOffsetting(); break;
            case PetState.Aligning:   UpdateAligning();   break;
        }
    }

    void UpdateFollowing()
    {
        MoveToward(avatarTransform.position);
        SlerpToward(avatarTransform.position);

        if (Vector3.Distance(transform.position, avatarTransform.position) < arriveThreshold)
        {
            offsetTargetPos = PickOffsetPosition();
            state = PetState.Offsetting;
        }
    }

    void UpdateOffsetting()
    {
        MoveToward(offsetTargetPos);
        SlerpToward(avatarTransform.position); // keep looking at avatar while moving

        if (Vector3.Distance(transform.position, offsetTargetPos) < arriveThreshold)
        {
            currentVelocity = Vector3.zero;
            state = PetState.Aligning;
        }
    }

    void UpdateAligning()
    {
        Vector3 avatarLook = avatarController.LookDirection;
        if (avatarLook.sqrMagnitude < 0.001f) return;

        Quaternion targetRot = Quaternion.LookRotation(avatarLook);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);

        float angleDiff = Quaternion.Angle(transform.rotation, targetRot);
        
        
        if (Vector3.Distance(transform.position, avatarTransform.position) > leashDistance)
            state = PetState.Following;
    }


    void MoveToward(Vector3 targetPos)
    {
        float dist = Vector3.Distance(transform.position, targetPos);
        float slowRadius = arriveThreshold * 4f;

        Vector3 desiredVelocity = (targetPos - transform.position).normalized * moveSpeed;

        // slow down
        if (dist < slowRadius)
            desiredVelocity *= (dist / slowRadius);

        // LERP
        currentVelocity = Vector3.Lerp(currentVelocity, desiredVelocity, acceleration * Time.deltaTime);
        transform.position += currentVelocity * Time.deltaTime;
    }
    
    
    void SlerpToward(Vector3 lookAtPoint)
    {
        Vector3 dir = lookAtPoint - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.001f) return;

        // SLERP
        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
    }

    // Choose a random position on the ground plane around the avatar.
    Vector3 PickOffsetPosition()
    {
        Vector2 rand2D = Random.insideUnitCircle.normalized;
        Vector3 randomDir = new Vector3(rand2D.x, 0f, rand2D.y);
        return avatarTransform.position + randomDir * followRadius;
    }
}
