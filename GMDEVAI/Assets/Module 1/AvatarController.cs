using UnityEngine;

public class AvatarController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] GameObject mouseIndicator;

    Camera mainCamera;
    
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
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        
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
            if (mouseIndicator != null)
                mouseIndicator.transform.position = hit.point + Vector3.up * 0.01f;

            // rotate y axis
            Vector3 lookDir = hit.point - transform.position;
            lookDir.y = 0f;

            if (lookDir.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(lookDir);
        }
    }
}
