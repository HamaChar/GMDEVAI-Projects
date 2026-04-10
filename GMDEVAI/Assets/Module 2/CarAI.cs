using UnityEngine;

public class CarAI : MonoBehaviour
{
    public Transform target;

    [Header("Movement")]
    public float speed = 0f;
    public float rotSpeed = 3f;
    public float minSpeed = 0f;
    public float maxSpeed = 8f;

    [Header("Acceleration / Braking")]
    public float acceleration = 5f;
    public float deceleration = 5f;

    public float breakAngle = 30f;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 lookAtGoal = new Vector3(target.position.x,
                                         this.transform.position.y,
                                         target.position.z);
        Vector3 direction = lookAtGoal - this.transform.position;

        // Rotate toward the cube
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation,
                                                    Quaternion.LookRotation(direction),
                                                    Time.deltaTime * rotSpeed);

        // Brake when the cube is already turning away from us AND we have momentum
        if (Vector3.Angle(target.forward, this.transform.forward) > breakAngle && speed > 2f)
        {
            speed = Mathf.Clamp(speed - (deceleration * Time.deltaTime), minSpeed, maxSpeed);
        }
        else
        {
            speed = Mathf.Clamp(speed + (acceleration * Time.deltaTime), minSpeed, maxSpeed);
        }

        this.transform.Translate(0, 0, speed * Time.deltaTime);
    }
}
