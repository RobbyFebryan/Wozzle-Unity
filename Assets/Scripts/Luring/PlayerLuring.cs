using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems; 
using ARP.APR.Scripts;

public class PlayerLuring : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rigidbody3D;
    [SerializeField] private ConfigurableJoint mainJoint;
    [SerializeField] private Animator animator;
    [SerializeField] private HandleGrabHandlerLuring[] handGrabHandlers;
    [SerializeField] private CameraController cameraController;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float turnSpeed = 10f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private float holdJumpForce = 20f;
    [SerializeField] private float maxJumpTime = 0.45f;

    [Header("Floaty Jump")]
    [SerializeField] private float fallMultiplier = 1.4f;
    [SerializeField] private float lowJumpMultiplier = 1.2f;

    [Header("Respawn Settings")]
    [SerializeField] private Vector3 defaultSpawnPosition;
    [SerializeField] private float respawnDelay = 1f;

    private float jumpTimeCounter;
    private bool isGrounded;
    private bool isJumping;
    private bool isJumpHeld;
    private bool jumpPressed;

    private float moveInputX = 0f;

    [HideInInspector] public bool IsGrabingActive = false;
    [HideInInspector] public bool IsActiveRagdoll = true;

    private Quaternion currentLookRotation;
    private SycnPhysicsObject[] syncPhysicsObjects;
    private float startSlerpPositionSpring;

    private void Awake()
    {
        syncPhysicsObjects = GetComponentsInChildren<SycnPhysicsObject>();
        currentLookRotation = transform.rotation;

        if (handGrabHandlers == null || handGrabHandlers.Length == 0)
            handGrabHandlers = GetComponentsInChildren<HandleGrabHandlerLuring>();
    }

    private void Start()
    {
        startSlerpPositionSpring = mainJoint.slerpDrive.positionSpring;

        if (defaultSpawnPosition == Vector3.zero)
            defaultSpawnPosition = transform.position;

        // Setup kamera (tidak lagi free rotate)
        var cam = FindAnyObjectByType<CinemachineVirtualCamera>();
        if (cam != null)
        {
            cam.Follow = transform;
            cam.LookAt = transform;
        }

        // Kursor aktif untuk klik UI
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        if (!IsActiveRagdoll)
            return;

        HandleGravity();
        HandleKeyboardInput(); 
    }

    private void FixedUpdate()
    {
        if (!IsActiveRagdoll) return;

        HandleMovement();
        HandleJump();

        animator.SetFloat("movementSpeed", Mathf.Abs(moveInputX) * 0.5f);

        for (int i = 0; i < syncPhysicsObjects.Length; i++)
        {
            if (IsActiveRagdoll)
                syncPhysicsObjects[i].UpdateJointFromAnimation();
        }
    }

    // === INPUT DARI UI BUTTON ===

    public void SetMoveInput(float value)
    {
        moveInputX = value;
    }

    public void StopMove()
    {
        moveInputX = 0f;
    }

    public void OnJumpButtonDown()
    {
        jumpPressed = true;
        isJumpHeld = true;
    }

    public void OnJumpButtonUp()
    {
        isJumpHeld = false;
    }

    public void OnGrabButtonDown()
    {
        IsGrabingActive = true;
        foreach (var hand in handGrabHandlers)
        {
            if (hand != null)
                hand.UpdateState();
        }
    }

    public void OnGrabButtonUp()
    {
        IsGrabingActive = false;
        foreach (var hand in handGrabHandlers)
        {
            if (hand != null)
                hand.UpdateState();
        }
    }

    // === INPUT DARI KEYBOARD ===
    private void HandleKeyboardInput()
    {
        // Cegah input keyboard saat klik UI (contoh di mobile)
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        float inputX = 0f;

        if (Input.GetKey(KeyCode.A))
            inputX = -1f;
        else if (Input.GetKey(KeyCode.D))
            inputX = 1f;

            moveInputX = inputX;

        // Tombol Lompat (Space)
        if (Input.GetKeyDown(KeyCode.Space))
            OnJumpButtonDown();
        if (Input.GetKeyUp(KeyCode.Space))
            OnJumpButtonUp();

        // Tombol Grab (E)
        if (Input.GetKeyDown(KeyCode.E))
            OnGrabButtonDown();
        if (Input.GetKeyUp(KeyCode.E))
            OnGrabButtonUp();
    }

    // === GERAK DAN JUMP ===

    private void HandleMovement()
    {
        isGrounded = Physics.CheckSphere(transform.position + Vector3.down * 0.5f, 0.15f, LayerMask.GetMask("Ground"));

        Vector3 moveDir = new Vector3(moveInputX, 0, 0);

        if (moveInputX != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(moveInputX) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }

        Vector3 currentVel = rigidbody3D.linearVelocity;
        Vector3 targetVel = moveDir * moveSpeed;
        rigidbody3D.linearVelocity = new Vector3(targetVel.x, currentVel.y, 0);
    }

    private void HandleJump()
    {
        if (isGrounded && jumpPressed)
        {
            Vector3 vel = rigidbody3D.linearVelocity;
            vel.y = jumpForce;
            rigidbody3D.linearVelocity = vel;
            isJumping = true;
            jumpTimeCounter = maxJumpTime;
        }

        jumpPressed = false;

        if (isJumping && isJumpHeld)
        {
            if (jumpTimeCounter > 0)
            {
                rigidbody3D.AddForce(Vector3.up * holdJumpForce, ForceMode.Force);
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        if (!isJumpHeld)
            isJumping = false;
    }

    private void HandleGravity()
    {
        if (rigidbody3D.linearVelocity.y < 0)
        {
            rigidbody3D.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rigidbody3D.linearVelocity.y > 0 && !isJumpHeld)
        {
            rigidbody3D.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    // === RAGDOLL DAN RESPAWN ===

    public void OnPlayerBodyPartHit()
    {
        if (!IsActiveRagdoll)
            return;

        MakeRagdoll();
    }

    private void MakeRagdoll()
    {
        JointDrive jointDrive = mainJoint.slerpDrive;
        jointDrive.positionSpring = 0;
        mainJoint.slerpDrive = jointDrive;

        for (int i = 0; i < syncPhysicsObjects.Length; i++)
        {
            syncPhysicsObjects[i].MakeRagdoll();
        }

        IsActiveRagdoll = false;
        Invoke(nameof(MakeActiveRagdoll), 3f);
    }

    private void MakeActiveRagdoll()
    {
        JointDrive jointDrive = mainJoint.slerpDrive;
        jointDrive.positionSpring = startSlerpPositionSpring;
        mainJoint.slerpDrive = jointDrive;

        for (int i = 0; i < syncPhysicsObjects.Length; i++)
        {
            syncPhysicsObjects[i].MakeActiveRagdoll();
        }

        IsActiveRagdoll = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("CauseDamage"))
        {
            OnPlayerBodyPartHit();
        }
    }

}
