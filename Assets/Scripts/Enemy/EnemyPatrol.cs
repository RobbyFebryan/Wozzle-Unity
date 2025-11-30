using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    public Transform[] waypoints;     
    public float moveSpeed = 3f;      
    private int currentWaypointIndex = 0;

    public PlayerHealth playerHealth;

    private void Update()
    {
        Patrol();
    }

    private void Patrol()
    {
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;

        // Gerak musuh
        transform.position += direction * moveSpeed * Time.deltaTime;

        // 🔥 Flip menggunakan rotate Y
        HandleFlip(direction);

        // Ganti waypoint jika sudah dekat
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.2f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    private void HandleFlip(Vector3 direction)
    {
        // Hanya flip kalau memiliki arah horizontal
        if (Mathf.Abs(direction.x) > 0.01f)
        {
            // Jika bergerak ke kanan: Y = 0
            // Jika bergerak ke kiri : Y = 180
            float targetYRotation = direction.x > 0 ? 0f : 180f;

            Vector3 newRotation = transform.eulerAngles;
            newRotation.y = targetYRotation;
            transform.eulerAngles = newRotation;
        }
    }

    void OnTriggerEnter (Collider other) {
        if (other.CompareTag("Player")) {
            playerHealth.TakeDamage(1);
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
