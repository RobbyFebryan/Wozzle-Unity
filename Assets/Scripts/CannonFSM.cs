using UnityEngine;

public class CannonFSM : MonoBehaviour
{
    private enum CannonState { Idle, Detect, Shoot }
    private CannonState currentState = CannonState.Idle;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float detectionAngle = 30f;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Shooting")]
    [SerializeField] private Transform cannonBarrel;
    [SerializeField] private GameObject canonballPrefab;
    [SerializeField] private float shootForce = 30f;
    [SerializeField] private float shootCooldown = 2f;
    private float shootTimer = 0f;

    [Header("Debug")]
    [SerializeField] private bool showDebugRays = true;

    private void Update()
    {
        shootTimer -= Time.deltaTime;

        switch (currentState)
        {
            case CannonState.Idle:
                HandleIdleState();
                break;

            case CannonState.Detect:
                HandleDetectState();
                break;

            case CannonState.Shoot:
                HandleShootState();
                break;
        }
    }


    private void HandleIdleState()
    {
        if (IsPlayerInFront())
            ChangeState(CannonState.Detect);
    }

    private void HandleDetectState()
    {
        // Kalau player keluar area → berhenti
        if (!IsPlayerInFront())
        {
            ChangeState(CannonState.Idle);
            return;
        }

        // Player masih di depan → terus menembak
        if (shootTimer <= 0f)
        {
            ShootCannonball();
            shootTimer = shootCooldown;
        }
    }


    private void HandleShootState()
    {
        ShootCannonball();
        shootTimer = shootCooldown;
        ChangeState(CannonState.Detect);
    }


    // ======================================================
    //  DETECTION FIX (mengikuti arah moncong cannon)
    // ======================================================
    private bool IsPlayerInFront()
    {
        if (playerTransform == null)
            return false;

        float distanceToPlayer = Vector3.Distance(cannonBarrel.position, playerTransform.position);
        if (distanceToPlayer > detectionRange)
            return false;

        // Arah moncong cannon → mengikuti script tembak (-up)
        Vector3 cannonDirection = -cannonBarrel.up;

        // Arah ke player
        Vector3 directionToPlayer = (playerTransform.position - cannonBarrel.position).normalized;

        // Sudut antara moncong & player
        float angleToPlayer = Vector3.Angle(cannonDirection, directionToPlayer);

        if (angleToPlayer > detectionAngle)
            return false;

        // Raycast dari moncong, bukan dari badan cannon
        RaycastHit hit;
        if (Physics.Raycast(cannonBarrel.position, directionToPlayer, out hit, detectionRange, ~obstacleLayer))
        {
            if (hit.transform != playerTransform)
                return false;
        }

        return true;
    }


    // ======================================================
    //  SHOOTING
    // ======================================================
    private void ShootCannonball()
    {
        if (canonballPrefab == null || cannonBarrel == null)
        {
            Debug.LogError("Canonball prefab atau cannon barrel belum diset!");
            return;
        }

        GameObject canonball = Instantiate(canonballPrefab, cannonBarrel.position, cannonBarrel.rotation);

        Rigidbody rb = canonball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 shootDirection = -cannonBarrel.up;
            rb.linearVelocity = shootDirection * shootForce;
        }

        // Add collision script
        CannonballCollision ballCollision = canonball.AddComponent<CannonballCollision>();
        ballCollision.SetCannon(this);

        Debug.Log("TEMBAK!");
    }


    // ======================================================
    //  STATE CHANGE
    // ======================================================
    private void ChangeState(CannonState newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
            Debug.Log("Cannon State: " + currentState);
        }
    }


    // ======================================================
    //  DEBUG GIZMOS (mengikuti arah moncong)
    // ======================================================
    private void OnDrawGizmosSelected()
    {
        if (!showDebugRays || cannonBarrel == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(cannonBarrel.position, detectionRange);

        Vector3 baseDir = -cannonBarrel.up; 
        Gizmos.color = Color.green;

        // Boundary kanan/kiri cone
        Vector3 rightBoundary = Quaternion.Euler(0, detectionAngle, 0) * baseDir * detectionRange;
        Vector3 leftBoundary = Quaternion.Euler(0, -detectionAngle, 0) * baseDir * detectionRange;

        Gizmos.DrawLine(cannonBarrel.position, cannonBarrel.position + rightBoundary);
        Gizmos.DrawLine(cannonBarrel.position, cannonBarrel.position + leftBoundary);
        Gizmos.DrawLine(cannonBarrel.position, cannonBarrel.position + baseDir * detectionRange);

        // Arc untuk cone
        for (int i = 0; i < 10; i++)
        {
            float a1 = -detectionAngle + (detectionAngle * 2 / 10) * i;
            float a2 = -detectionAngle + (detectionAngle * 2 / 10) * (i + 1);

            Vector3 dir1 = Quaternion.Euler(0, a1, 0) * baseDir;
            Vector3 dir2 = Quaternion.Euler(0, a2, 0) * baseDir;

            Gizmos.DrawLine(
                cannonBarrel.position + dir1 * detectionRange,
                cannonBarrel.position + dir2 * detectionRange
            );
        }
    }


    public void SetPlayer(Transform player)
    {
        playerTransform = player;
    }
}
