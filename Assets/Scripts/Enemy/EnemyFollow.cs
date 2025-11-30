using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    [Header("Patrol")]
    public Transform[] waypoints;
    public float moveSpeed = 3f;
    private int currentWaypointIndex = 0;

    [Header("Player Detect")]
    public float detectRange = 6f;    // Jarak deteksi player
    private Transform player;

    private bool isFollowingPlayer = false;

    public PlayerHealth playerHealth;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (PlayerInRange())
        {
            FollowPlayer();
        }
        else
        {
            isFollowingPlayer = false;
            Patrol();
        }
    }

    
    // PATROL SYSTEM
    private void Patrol()
    {
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;

        transform.position += direction * moveSpeed * Time.deltaTime;

        HandleFlip(direction);

        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.2f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    
    // PLAYER DETECTION
    private bool PlayerInRange()
    {
        if (player == null) return false;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= detectRange)
        {
            isFollowingPlayer = true;
        }
        else if (dist > detectRange + 1f)  
        {
            isFollowingPlayer = false;
        }

        return isFollowingPlayer;
    }

    
    // FOLLOW PLAYER (X only)
    private void FollowPlayer()
    {
        // Hitung selisih posisi hanya pada sumbu X
        float dx = player.position.x - transform.position.x;

        // Tentukan arah (-1 = kiri, 1 = kanan)
        float directionX = Mathf.Sign(dx);

        // Gerakkan enemy hanya pada sumbu X
        Vector3 move = new Vector3(directionX, 0f, 0f);

        transform.position += move * moveSpeed * Time.deltaTime;

        // Flip berdasarkan arah X
        HandleFlip(new Vector3(directionX, 0, 0));
    }

    
    // FLIP ROTATE Y
    private void HandleFlip(Vector3 direction)
    {
        if (Mathf.Abs(direction.x) > 0.01f)
        {
            float targetYRotation = direction.x > 0 ? 0f : 180f;

            Vector3 newRotation = transform.eulerAngles;
            newRotation.y = targetYRotation;
            transform.eulerAngles = newRotation;
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter (Collider other) {
        if (other.CompareTag("Player")) {
            playerHealth.TakeDamage(1);
        }
    }
}
