using Fusion;
using UnityEngine;
using Cinemachine;
using Fusion.Addons.Physics;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public static NetworkPlayer Local { get; private set; }

    [Header("References")]
    [SerializeField] private Rigidbody rigidbody3D;
    [SerializeField] private NetworkRigidbody3D networkRigidbody3D;
    [SerializeField] private ConfigurableJoint mainJoint;
    [SerializeField] private Animator animator;

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
    private float jumpTimeCounter;

    private bool localGrounded;

    private Vector2 moveInputVector;
    //private bool isJumpPressed;
    private bool isReviveButtonPressed = false;
    private bool isGrabButtonPressed = false; 

    private bool isJumpHeld;
    private bool isJumping;
    private bool isGrounded;
    private bool isActiveRagdoll = true;
    private bool isGrabingActive = false;

    public bool IsActiveRagdoll => isActiveRagdoll;
    public bool IsGrabingActive => isGrabingActive;

    private Quaternion currentLookRotation;

    private RaycastHit[] raycastHits = new RaycastHit[10];
    private SycnPhysicsObject[] syncPhysicsObjects;
    private Transform camTransform;

    private float jumpBufferTime = 0.1f;
    private float jumpBufferCounter;
    private bool jumpPredicted;

    [Networked, Capacity(10)] public NetworkArray<Quaternion> networkPhysicsSyncRotation { get; }

    private float startSlerpPositionSpring = 0.0f;

    private float lastTimeBecameRagdoll = 0;

    private HandleGrabHandler[] handleGrabHandlers; 


    private void Start()
    {
        startSlerpPositionSpring = mainJoint.slerpDrive.positionSpring;    
    }

    private void Awake()
    {
        syncPhysicsObjects = GetComponentsInChildren<SycnPhysicsObject>();
        handleGrabHandlers = GetComponentsInChildren<HandleGrabHandler>();
        currentLookRotation = transform.rotation;
    }

    private bool CheckGroundedLocal()
    {
        return Physics.CheckSphere(transform.position + Vector3.down * 0.5f, 0.15f, LayerMask.GetMask("Ground"), QueryTriggerInteraction.Ignore);
    }

    // ========== UPDATE INPUT ==========
    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();

        if (Input.GetKeyDown(KeyCode.Space))
            jumpBufferCounter = jumpBufferTime;

        if (jumpBufferCounter > 0)
            jumpBufferCounter -= Time.deltaTime;

        if (isReviveButtonPressed)
            networkInputData.isReviveButtonPressed = true;

        if (isGrabButtonPressed)
            networkInputData.isGrabPressed = true;


        Vector3 camForward = Vector3.forward;
        Vector3 camRight = Vector3.right;

        if (Camera.main != null)
        {
            camForward = Camera.main.transform.forward;
            camRight = Camera.main.transform.right;

            camForward.y = 0;
            camRight.y = 0;

            camForward.Normalize();
            camRight.Normalize();
        }

        //bool reviveButtonPressed = Input.GetKeyDown(KeyCode.R);
        bool grabButtonPressed = Input.GetMouseButton(0);

        NetworkInputData input = new NetworkInputData
        {
            movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")),
            isJumpPressed = jumpBufferCounter > 0,
            isReviveButtonPressed = false,
            isJumpHeld = Input.GetKey(KeyCode.Space),
            camForward = camForward,
            camRight = camRight,
            isGrabPressed = grabButtonPressed,

        };

        isReviveButtonPressed = false;

        return input;
    }

    public void OnPlayerBodyPartHit()
    {
        if (!IsActiveRagdoll)
            return;

        MakeRagdoll();
    }

    void MakeRagdoll()
    {
        if (!Object.HasStateAuthority)
            return;

        JointDrive jointDrive = mainJoint.slerpDrive;
        jointDrive.positionSpring = 0;
        mainJoint.slerpDrive = jointDrive;

        for (int i = 0; i < syncPhysicsObjects.Length; i++)
        {
            syncPhysicsObjects[i].MakeRagdoll();
        }

        lastTimeBecameRagdoll = Runner.SimulationTime;
        isActiveRagdoll = false;
        isGrabingActive = false;
    }

    void MakeActiveRagdoll()
    {
        if (!Object.HasStateAuthority)
            return;

        JointDrive jointDrive = mainJoint.slerpDrive;
        jointDrive.positionSpring = startSlerpPositionSpring;
        mainJoint.slerpDrive = jointDrive;

        for (int i = 0; i < syncPhysicsObjects.Length; i++)
        {
            syncPhysicsObjects[i].MakeActiveRagdoll();
        }

        isActiveRagdoll = true;
        isGrabingActive = false;
    }

    // ========== FIXED UPDATE NETWORK ==========
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData input))
        {
            isGrabingActive = input.isGrabPressed;

            if (isActiveRagdoll)
            {

                    HandleMovement(input);
                    HandleJump(input);
                

                if (Object.HasStateAuthority)
                {
                    animator.SetFloat("movementSpeed", input.movementInput.magnitude * 0.2f);

                    for (int i = 0; i < syncPhysicsObjects.Length; i++)
                    {
                        if (isActiveRagdoll)
                            syncPhysicsObjects[i].UpdateJointFromAnimation();
                        networkPhysicsSyncRotation.Set(i, syncPhysicsObjects[i].transform.localRotation);
                    }

                    if (transform.position.y < -10)
                    {
                        networkRigidbody3D.Teleport(Vector3.zero, Quaternion.identity);
                        MakeActiveRagdoll();
                    }

                    foreach (HandleGrabHandler handleGrabHandler in handleGrabHandlers)
                    {
                        handleGrabHandler.UpdateState();
                    }
                }

                HandleGravity();
                jumpPredicted = false; // reset prediksi
            }
            else
            {
                // Revive otomatis setelah 1 detik sejak jadi ragdoll
                if (Runner.SimulationTime - lastTimeBecameRagdoll > 3)
                {
                    MakeActiveRagdoll();
                }
            }
        }
    }

    // ========== MOVEMENT ==========
    private void HandleMovement(NetworkInputData input)
    {
        isGrounded = false;
        int hitCount = Physics.SphereCastNonAlloc(rigidbody3D.position, 0.15f, Vector3.down, raycastHits, 0.7f);
        for (int i = 0; i < hitCount; i++)
        {
            if (raycastHits[i].transform.root == transform) continue;
            isGrounded = true;
            break;
        }

        if (camTransform == null)
            camTransform = Camera.main ? Camera.main.transform : null;

        if (camTransform == null || input.movementInput == Vector2.zero)
        {
            rigidbody3D.linearVelocity = new Vector3(0, rigidbody3D.linearVelocity.y, 0);
            return;
        }

        Vector3 camForward = input.camForward;
        Vector3 camRight = input.camRight;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir = (camForward * input.movementInput.y + camRight * input.movementInput.x).normalized;

        // Arah pandang mengikuti arah kamera, bukan arah gerak
        Vector3 lookDir = camForward;
        lookDir.y = 0f;
        lookDir.Normalize();

        // Kalau ada input gerak, karakter menghadap ke arah kamera
        if (input.movementInput.magnitude > 0.1f)
        {
            Quaternion desiredRot = Quaternion.LookRotation(lookDir);
            currentLookRotation = Quaternion.Slerp(currentLookRotation, desiredRot, Runner.DeltaTime * turnSpeed);
            mainJoint.targetRotation = Quaternion.Inverse(currentLookRotation);
        }

        // Gerak tetap berdasarkan input relatif kamera
        Vector3 currentVel = rigidbody3D.linearVelocity;
        Vector3 targetVel = moveDir * moveSpeed;
        rigidbody3D.linearVelocity = new Vector3(targetVel.x, currentVel.y, targetVel.z);
    }

    // ========== JUMP HANDLING ==========
    private void HandleJump(NetworkInputData input)
    {
        if (isGrounded && input.isJumpPressed && !jumpPredicted)
        {
            Vector3 vel = rigidbody3D.linearVelocity;
            vel.y = jumpForce;
            rigidbody3D.linearVelocity = vel;
            isJumping = true;
            jumpTimeCounter = maxJumpTime;
        }

        if (isJumping && input.isJumpHeld)
        {
            if (jumpTimeCounter > 0)
            {
                rigidbody3D.AddForce(Vector3.up * holdJumpForce, ForceMode.Force);
                jumpTimeCounter -= Runner.DeltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        if (!input.isJumpHeld)
            isJumping = false;
    }

    // ========== IMMEDIATE LOCAL EXECUTION ==========
    public override void Render()
    {
        if (Object.HasInputAuthority)
        {
            bool localGrounded = Physics.CheckSphere(transform.position + Vector3.down * 0.5f, 0.15f, LayerMask.GetMask("Ground"), QueryTriggerInteraction.Ignore);

            // Prediksi loncat langsung biar responsif
            if (Input.GetKeyDown(KeyCode.Space) && localGrounded)
            {
                Vector3 vel = rigidbody3D.linearVelocity;
                vel.y = jumpForce;
                rigidbody3D.linearVelocity = vel;
                jumpPredicted = true;
            }
        }

        // Sync rotation dari network
        if (!Object.HasStateAuthority)
        {
            var interpolated = new NetworkBehaviourBufferInterpolator(this);
            for (int i = 0; i < syncPhysicsObjects.Length; i++)
            {
                syncPhysicsObjects[i].transform.localRotation = Quaternion.Slerp(
                    syncPhysicsObjects[i].transform.localRotation,
                    networkPhysicsSyncRotation.Get(i),
                    interpolated.Alpha
                );
            }
        }
    }

    // ========== GRAVITY ==========
    private void HandleGravity()
    {
        if (rigidbody3D.linearVelocity.y < 0)
        {
            rigidbody3D.linearVelocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Runner.DeltaTime;
        }
        else if (rigidbody3D.linearVelocity.y > 0 && !isJumpHeld)
        {
            rigidbody3D.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Runner.DeltaTime;
        }
    }
    /*
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("CauseDamage"))
        MakeRagdoll();
    }*/

    // ========== SPAWN ==========
    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            Local = this;
            var freeLookCam = FindObjectOfType<CinemachineFreeLook>();
            if (freeLookCam != null)
            {
                freeLookCam.Follow = transform;
                freeLookCam.LookAt = transform;
            }
            camTransform = Camera.main ? Camera.main.transform : null;
        }

        transform.name = $"P_{Object.Id}";
    }

    public void PlayerLeft(PlayerRef player) { }
}
