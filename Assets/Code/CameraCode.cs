using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target & Smoothness")]
    public Transform player;
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0f, 1f, -10f);

    [Header("Level Bounds")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        if (player == null) return;

        // Desired position based on player tracking
        Vector3 targetPosition = player.position + offset;

        // Calculate camera half-width and half-height so it stops right at the wall edge
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        // Clamp the camera position between your level min and max limits
        float clampedX = Mathf.Clamp(targetPosition.x, minX + camWidth, maxX - camWidth);
        float clampedY = Mathf.Clamp(targetPosition.y, minY + camHeight, maxY - camHeight);

        // Apply clamped position while keeping the camera's Z depth
        Vector3 clampedPosition = new Vector3(clampedX, clampedY, offset.z);

        // Smoothly transition to the clamped position
        transform.position = Vector3.Lerp(transform.position, clampedPosition, smoothSpeed);
    }
}