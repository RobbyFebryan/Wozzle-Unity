using Cinemachine;
using UnityEngine;
using ARP.APR.Scripts;

public class PlayerLuringJoystick : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rigidbody3D;
    [SerializeField] private ConfigurableJoint mainJoint;
    [SerializeField] private Animator animator;
    [SerializeField] private HandleGrabHandlerLuring[] handGrabHandlers;
    [SerializeField] private CameraController cameraController;

    [Header("Joystick Input")]
    [SerializeField] private Joystick joystick;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 30f;
    [SerializeField] private float turnSpeed = 5f;

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

    [HideInInspector] public bool IsGrabingActive = false;
    [HideInInspector] public bool IsActiveRagdoll = true;

    private Quaternion currentLookRotation;
    private Transform camTransform;
    private SycnPhysicsObject[] syncPhysicsObjects;
    private float startSlerpPositionSpring;

    // KEYBOARD MOVEMENT INPUT
    private Vector2 keyboardMoveInput;
    private PlayerLuring player;


    private void Awake()
    {
        syncPhysicsObjects = GetComponentsInChildren<SycnPhysicsObject>();
        currentLookRotation = transform.rotation;

        if (handGrabHandlers == null || handGrabHandlers.Length == 0)
            handGrabHandlers = GetComponentsInChildren<HandleGrabHandlerLuring>();
    }

    private void Start()
    {
        player = FindAnyObjectByType<PlayerLuring>();
        startSlerpPositionSpring = mainJoint.slerpDrive.positionSpring;

        if (defaultSpawnPosition == Vector3.zero)
            defaultSpawnPosition = transform.position;

        var freeLookCam = FindObjectOfType<CinemachineFreeLook>();
        if (freeLookCam != null)
        {
            freeLookCam.Follow = transform;
            freeLookCam.LookAt = transform;
        }

        camTransform = Camera.main ? Camera.main.transform : null;
    }

    private void Update()
    {
        if (!IsActiveRagdoll)
            return;

        HandleInput();
        HandleGravity();
        UpdateGrabState();

        MouseGrabInput();
    }

    private void FixedUpdate()
    {
        if (!IsActiveRagdoll) return;

        HandleMovement();
        HandleJump();

        // ANIMATION (joystick or keyboard)
        Vector2 animInput =
            joystick != null && (Mathf.Abs(joystick.Horizontal) > .1f || Mathf.Abs(joystick.Vertical) > .1f)
            ? new Vector2(joystick.Horizontal, joystick.Vertical)
            : keyboardMoveInput;

        animator.SetFloat("movementSpeed", animInput.magnitude * 0.2f);

        for (int i = 0; i < syncPhysicsObjects.Length; i++)
        {
            if (IsActiveRagdoll)
                syncPhysicsObjects[i].UpdateJointFromAnimation();
        }
    }

    private void HandleInput()
    {
        // ===== JUMP: SPACE =====
        isJumpHeld = Input.GetKey(KeyCode.Space);
        if (Input.GetKeyDown(KeyCode.Space))
            jumpPressed = true;

        // ===== MOVEMENT: WASD / ARROWS =====
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        keyboardMoveInput = new Vector2(h, v);

        // ===== GRAB: E =====
        if (Input.GetKeyDown(KeyCode.E))
        {
            IsGrabingActive = true;
            foreach (var hnd in handGrabHandlers)
                hnd?.UpdateState();
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            IsGrabingActive = false;
            foreach (var hnd in handGrabHandlers)
                hnd?.UpdateState();
        }
    }

    // ========================= UI BUTTON INPUT ========================= //
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
            hand?.UpdateState();
    }
    public void OnGrabButtonUp()
    {
        IsGrabingActive = false;
        foreach (var hand in handGrabHandlers)
            hand?.UpdateState();
    }

    // ========================= MOVEMENT ========================= //
    private void HandleMovement()
    {
        isGrounded = Physics.CheckSphere(transform.position + Vector3.down * 0.5f,
                                         0.15f, LayerMask.GetMask("Ground"));

        if (camTransform == null)
            camTransform = Camera.main ? Camera.main.transform : null;

        // JOYSTICK
        Vector2 joyInput = joystick != null
            ? new Vector2(joystick.Horizontal, joystick.Vertical)
            : Vector2.zero;

        // PRIORITY: KEYBOARD > JOYSTICK
        Vector2 moveInput = keyboardMoveInput.magnitude > 0.1f ? keyboardMoveInput : joyInput;

        if (moveInput == Vector2.zero)
        {
            rigidbody3D.linearVelocity = new Vector3(0, rigidbody3D.linearVelocity.y, 0);
            return;
        }

        // Convert input ke arah dunia (berdasarkan kamera)
        Vector3 camForward = camTransform.forward;
        Vector3 camRight = camTransform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camForward * moveInput.y + camRight * moveInput.x).normalized;

        // ROTATE TOWARDS MOVEMENT
        if (moveInput.magnitude > 0.1f)
        {
            Quaternion desiredRot = Quaternion.LookRotation(moveDir);
            currentLookRotation = Quaternion.Slerp(currentLookRotation, desiredRot, Time.deltaTime * turnSpeed);
            mainJoint.targetRotation = Quaternion.Inverse(currentLookRotation);
        }

        // APPLY VELOCITY
        Vector3 currentVel = rigidbody3D.linearVelocity;
        Vector3 targetVel = moveDir * moveSpeed;

        rigidbody3D.linearVelocity = new Vector3(targetVel.x, currentVel.y, targetVel.z);
    }

    // ========================= JUMP ========================= //
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

    // ========================= GRAB FROM MOUSE ========================= //
    private void UpdateGrabState()
    {
        if (Input.GetMouseButtonDown(0))
        {
            IsGrabingActive = true;
            foreach (var hnd in handGrabHandlers)
                hnd?.UpdateState();
        }

        if (Input.GetMouseButtonUp(0))
        {
            IsGrabingActive = false;
            foreach (var hnd in handGrabHandlers)
                hnd?.UpdateState();
        }
    }


    // ========================= RESPAWN ========================= //
    public void Die()
    {
        Debug.Log("Player mati! Respawn dalam " + respawnDelay + " detik");

        IsActiveRagdoll = false;

        if (cameraController != null)
        {
            cameraController.SetStandby(true);
            Invoke(nameof(ResumeCamera), 1f);
        }

        Invoke(nameof(Respawn), respawnDelay);
    }

    private void Respawn()
    {
        Vector3 respawnPos;

        if (CheckpointManager.Instance != null && CheckpointManager.Instance.hasCheckpoint)
            respawnPos = CheckpointManager.Instance.GetRespawnPosition();
        else
            respawnPos = defaultSpawnPosition;

        transform.position = respawnPos;
        transform.rotation = Quaternion.identity;

        rigidbody3D.linearVelocity = Vector3.zero;
        rigidbody3D.angularVelocity = Vector3.zero;

        Debug.Log("Player Respawn!");
    }

    private void ResumeCamera()
    {
        if (cameraController != null)
            cameraController.SetStandby(false);
    }

    public void GrabButtonDown()
    {
        player.OnGrabButtonDown();
    }

    public void GrabButtonUp()
    {
        player.OnGrabButtonUp();
    }

    public void MouseGrabInput()
    {
        if (Input.GetMouseButtonDown(0)) // Klik kiri = Grab mulai
        {
            GrabButtonDown();
        }

        if (Input.GetMouseButtonUp(0)) // Lepas klik = Grab berhenti
        {
            GrabButtonUp();
        }
    }

    //
}
