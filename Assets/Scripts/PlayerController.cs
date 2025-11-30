using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed;
    public float strafeSpeed;
    public float jumpForce;

    [Header("Component References")]
    public Rigidbody hips;
    public bool isGrounded;
    public Animator anim;

    float verticalInput;
    float horizontalInput;

    [Header("Arms")]
    public ConfigurableJoint leftArmJoint;
    public ConfigurableJoint rightArmJoint;
    public float rotateSpeed = 100f;

    private Quaternion leftDefaultRot;
    private Quaternion rightDefaultRot;
    public Quaternion leftRaisedRot;
    public Quaternion rightRaisedRot;

    [Header("Joints")]
    public ConfigurableJoint leftHandJoint;
    public ConfigurableJoint rightHandJoint;
    private Quaternion leftDefaultLocalRot;
    private Quaternion rightDefaultLocalRot;

    [Header("Arm Rotations (Euler Angles)")]
    public Vector3 leftRaisedEuler;
    public Vector3 rightRaisedEuler;


    void Start()
    {

        if (hips == null)
            hips = GetComponent<Rigidbody>();

        // ambil rotasi lokal default joint
        leftDefaultLocalRot = leftHandJoint.transform.localRotation;
        rightDefaultLocalRot = rightHandJoint.transform.localRotation;
    }

    void FixedUpdate()
    {
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        // Arah gerakan berdasarkan kamera
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        // hilangkan efek miring ke atas/bawah (y = 0)
        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        // Hitung arah gerakan
        Vector3 moveDir = (camForward * verticalInput * speed) + (camRight * horizontalInput * strafeSpeed);

        hips.AddForce(moveDir, ForceMode.Force);

        Jump();
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            hips.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            //isGrounded = false;
        }
    }

    void Update()
    {
        // target rotasi relatif terhadap default
        Quaternion leftTarget = Input.GetMouseButton(0)
            ? Quaternion.Euler(leftRaisedEuler) * Quaternion.Inverse(leftDefaultLocalRot)
            : Quaternion.identity;

        Quaternion rightTarget = Input.GetMouseButton(1)
            ? Quaternion.Euler(rightRaisedEuler) * Quaternion.Inverse(rightDefaultLocalRot)
            : Quaternion.identity;

        leftHandJoint.targetRotation = Quaternion.Slerp(
            leftHandJoint.targetRotation,
            leftTarget,
            Time.deltaTime * rotateSpeed
        );

        rightHandJoint.targetRotation = Quaternion.Slerp(
            rightHandJoint.targetRotation,
            rightTarget,
            Time.deltaTime * rotateSpeed
        );
    }

    /*void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }*/
}