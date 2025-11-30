using UnityEngine;

public class CameraFollowHorizontalVertical : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform player; // Drag player object di Inspector

    [Header("Camera Settings")]
    public float smoothSpeed = 0.125f; // Kecepatan smooth camera
    public Vector3 offset = new Vector3(0, 5, -10); // Offset kamera

    [Header("Follow Settings")]
    public bool smoothFollow = true; // Aktifkan smooth follow

    [Header("Vertical Bounds")]
    public float minY = 2f; // Batas bawah kamera
    public float maxY = 10f; // Batas atas kamera

    void LateUpdate()
    {
        if (player == null)
        {
            Debug.LogWarning("Player tidak di-assign! Drag player ke slot di Inspector.");
            return;
        }

        // Hitung target Y dengan batas
        float targetY = Mathf.Clamp(player.position.y + offset.y, minY, maxY);

        // Target mengikuti semua axis sesuai offset (X, Y, dan Z)
        Vector3 targetPosition = new Vector3(
            player.position.x + offset.x,
            targetY,
            player.position.z + offset.z
        );

        // Smooth atau tidak
        if (smoothFollow)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
        }
        else
        {
            transform.position = targetPosition;
        }
    }
}
