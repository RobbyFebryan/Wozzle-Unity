using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Player To Follow")]
    public Transform APRRoot;

    [Header("Follow Properties")]
    public float distance = 10.0f;
    public float smoothness = 0.15f;

    [Header("Rotation Properties")]
    public bool rotateCamera = true;
    public float rotateSpeed = 5.0f;
    public float minAngle = -45.0f;
    public float maxAngle = -10.0f;

    [Header("Camera Collision")]
    public float minDistance = 1.0f;       // jarak terdekat saat terbentur
    public float collisionRadius = 0.3f;   // radius spherecast
    public LayerMask collisionMask;        // layer tembok

    [HideInInspector] public bool isStandby = false;

    private Camera cam;
    private float currentX = 0.0f;
    private float currentY = 0.0f;
    private Quaternion rotation;
    private Vector3 offset;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        cam = Camera.main;
        offset = cam.transform.position - APRRoot.position;
    }

    void Update()
    {
        if (!isStandby)
        {
            currentX += Input.GetAxis("Mouse X") * rotateSpeed;
            currentY += Input.GetAxis("Mouse Y") * rotateSpeed;
            currentY = Mathf.Clamp(currentY, minAngle, maxAngle);
        }
    }

    void FixedUpdate()
    {
        if (isStandby)
            return;

        if (rotateCamera)
        {
            Vector3 desiredDir = new Vector3(0, 0, -distance);
            rotation = Quaternion.Euler(-currentY, currentX, 0);

            // posisi ideal kamera
            Vector3 desiredPos = APRRoot.position + rotation * desiredDir;

            // ---------- CAMERA COLLISION ----------
            float finalDistance = distance;

            RaycastHit hit;
            if (Physics.SphereCast(
                APRRoot.position,
                collisionRadius,
                (desiredPos - APRRoot.position).normalized,
                out hit,
                distance,
                collisionMask
            ))
            {
                finalDistance = Mathf.Clamp(hit.distance, minDistance, distance);
                desiredPos = APRRoot.position + rotation * new Vector3(0, 0, -finalDistance);
            }
            // --------------------------------------

            cam.transform.position = Vector3.Lerp(
                cam.transform.position,
                desiredPos,
                smoothness
            );

            cam.transform.LookAt(APRRoot.position);
        }
        else
        {
            var targetRotation = Quaternion.LookRotation(APRRoot.position - cam.transform.position);
            cam.transform.position = Vector3.Lerp(cam.transform.position, APRRoot.position + offset, smoothness);
            cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, targetRotation, smoothness);
        }
    }

    public void SetStandby(bool standby)
    {
        isStandby = standby;
    }
}
