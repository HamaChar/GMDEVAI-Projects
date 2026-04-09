using UnityEngine;

public class AvatarController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] GameObject mouseIndicator;

    Camera mainCamera;

    // The PET reads this to know which way the avatar is facing.
    public Vector3 LookDirection => transform.forward;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal"); // A / D
        float v = Input.GetAxis("Vertical");   // W / S

        // Flatten camera axes onto the ground plane so movement is always horizontal.
        Vector3 camForward = mainCamera.transform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = mainCamera.transform.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 moveDir = (camRight * h + camForward * v).normalized;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    void HandleMouseLook()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            // Move the circle indicator to the hit point, sitting just above the surface.
            if (mouseIndicator != null)
                mouseIndicator.transform.position = hit.point + Vector3.up * 0.01f;

            // Rotate avatar on Y axis to face the hit point.
            Vector3 lookDir = hit.point - transform.position;
            lookDir.y = 0f;

            if (lookDir.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(lookDir);
        }
    }
}
