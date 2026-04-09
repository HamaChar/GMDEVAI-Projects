using UnityEngine;
using UnityStandardAssets.Utility;

public class CarAI : MonoBehaviour
{
    public WaypointCircuit circuit;

    [Header("Movement")]
    public float maxSpeed = 7f;
    public float rotSpeed = 3f;
    public float accuracy = 1f;

    [Header("Braking")]
    [Tooltip("Angle threshold (degrees) to the next waypoint at which the car begins braking. " +
             "Large = cautious (brakes early), Small = daredevil (barely brakes).")]
    public float brakeAngle = 30f;

    [Tooltip("Speed multiplier applied when braking (0–1). Lower = harder braking.")]
    [Range(0f, 1f)]
    public float brakeStrength = 0.5f;

    private int currentWaypointIndex = 0;

    private void LateUpdate()
    {
        if (circuit == null || circuit.Waypoints.Length == 0) return;

        Transform currentWaypoint = circuit.Waypoints[currentWaypointIndex];

        // Direction to waypoint, preserving Y so cars handle slopes correctly
        Vector3 direction = currentWaypoint.position - transform.position;

        // Advance to next waypoint when close enough
        if (direction.magnitude < accuracy)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= circuit.Waypoints.Length)
                currentWaypointIndex = 0;
            return;
        }

        // Rotate toward waypoint
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(direction),
            Time.deltaTime * rotSpeed
        );

        // Brake when the angle to the waypoint exceeds brakeAngle
        float angleToWaypoint = Vector3.Angle(transform.forward, direction);
        float effectiveSpeed = angleToWaypoint > brakeAngle
            ? maxSpeed * brakeStrength
            : maxSpeed;

        transform.Translate(0, 0, effectiveSpeed * Time.deltaTime);
    }
}
