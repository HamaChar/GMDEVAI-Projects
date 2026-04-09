using UnityEngine;

public class PetController : MonoBehaviour
{
    [SerializeField] Transform avatarTransform;
    [SerializeField] AvatarController avatarController;

    [SerializeField] float moveSpeed      = 4f;
    [SerializeField] float acceleration   = 3f;   // Vector3.Lerp rate on velocity
    [SerializeField] float followRadius   = 2f;   // distance of the offset position from avatar
    [SerializeField] float arriveThreshold = 0.25f; // "close enough" to a target position
    [SerializeField] float leashDistance  = 4f;   // how far before IDLE → FOLLOWING again
    [SerializeField] float rotationSpeed  = 4f;   // Quaternion.Slerp rate
    [SerializeField] float alignThreshold = 3f;   // degrees — when considered "aligned"

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
            case PetState.Idle:       UpdateIdle();        break;
        }
    }

    // ── FOLLOWING ────────────────────────────────────────────────────────────
    // Move toward the avatar. When close enough, pick an offset and switch state.
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

    // ── OFFSETTING ───────────────────────────────────────────────────────────
    // Move to the randomly chosen spot beside the avatar.
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

    // ── ALIGNING ─────────────────────────────────────────────────────────────
    // Slerp rotation to match the avatar's look direction.
    void UpdateAligning()
    {
        Vector3 avatarLook = avatarController.LookDirection;
        if (avatarLook.sqrMagnitude < 0.001f) return;

        Quaternion targetRot = Quaternion.LookRotation(avatarLook);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);

        float angleDiff = Quaternion.Angle(transform.rotation, targetRot);
        if (angleDiff < alignThreshold)
            state = PetState.Idle;
    }

    // ── IDLE ─────────────────────────────────────────────────────────────────
    // Stay put. Re-follow if the avatar wanders too far.
    void UpdateIdle()
    {
        if (Vector3.Distance(transform.position, avatarTransform.position) > leashDistance)
            state = PetState.Following;
    }

    // ── HELPERS ──────────────────────────────────────────────────────────────

    // Velocity-based Vector3.Lerp movement — accelerates away from rest,
    // decelerates as the pet enters the slow-down radius near the target.
    void MoveToward(Vector3 targetPos)
    {
        float dist = Vector3.Distance(transform.position, targetPos);
        float slowRadius = arriveThreshold * 4f;

        Vector3 desiredVelocity = (targetPos - transform.position).normalized * moveSpeed;

        // Scale speed down linearly inside the slow radius → natural deceleration.
        if (dist < slowRadius)
            desiredVelocity *= (dist / slowRadius);

        // Lerp current velocity toward desired — this IS the acceleration.
        currentVelocity = Vector3.Lerp(currentVelocity, desiredVelocity, acceleration * Time.deltaTime);
        transform.position += currentVelocity * Time.deltaTime;
    }

    // Quaternion.Slerp rotation toward a world-space point (Y-axis only).
    void SlerpToward(Vector3 lookAtPoint)
    {
        Vector3 dir = lookAtPoint - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.001f) return;

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
