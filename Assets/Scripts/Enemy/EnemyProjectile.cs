using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Patrol")]
    public Transform[] waypoints;
    public float moveSpeed = 3f;
    private int currentWaypointIndex = 0;

    [Header("Player Detect & Attack")]
    public float detectRange = 8f;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireCooldown = 1.5f;
    public float projectileSpeed = 5f;
    public float projectileLifetime = 3f;

    private float fireTimer = 0f;

    private Transform player;

    public PlayerHealth playerHealth;

    Animator anim;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        fireTimer += Time.deltaTime;

        if (PlayerInRange())
        {
            AttackPlayer();
        }
        else
        {
            Patrol();
        }
    }

    //  PATROL 
    private void Patrol()
    {
        anim.SetBool("IsWalking", true);

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = (targetWaypoint.position - transform.position).normalized;

        transform.position += direction * moveSpeed * Time.deltaTime;

        // Flip + rotate
        HandleRotateTowards(direction);

        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.2f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    //  PLAYER DETECT 
    private bool PlayerInRange()
    {
        if (player == null) return false;

        float dist = Vector3.Distance(transform.position, player.position);
        return dist <= detectRange;
    }

    //  ATTACK PLAYER 
    private void AttackPlayer()
    {
        Vector3 dir = player.position - transform.position;

        // Rotate ke arah player (360 derajat full)
        HandleRotateTowards(dir);

        if (fireTimer >= fireCooldown)
        {
            anim.SetTrigger("Shoot");
            anim.SetBool("IsWalking", false);
            //ShootProjectile();
            fireTimer = 0f;
        }
    }

    //  SHOOT PROJECTILE 
    public void ShootProjectile()
    {
        float directionX = Mathf.Sign(player.position.x - transform.position.x);

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        float zRot = directionX > 0 ? 90f : -90f;
        proj.transform.rotation = Quaternion.Euler(0, 0, zRot);

        Vector3 dirToPlayer = player.position - firePoint.position;
        float yRot = Quaternion.LookRotation(dirToPlayer, Vector3.up).eulerAngles.y;

        if (directionX < 0)
            yRot += 90f;
        else
            yRot -= 90f;

        proj.transform.rotation = Quaternion.Euler(0, yRot, zRot);

        Vector3 moveDir = transform.forward.normalized;

        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = moveDir * projectileSpeed;
        }

        // ---------- AUTO DESTROY SETELAH BEBERAPA DETIK ----------
        Destroy(proj, projectileLifetime);
    }


    //  ROTATE TOWARDS (360°) 
    private void HandleRotateTowards(Vector3 direction)
    {
        if (direction.sqrMagnitude > 0.01f)
        {
            // Rotasi penuh 360 derajat menghadap arah player
            Quaternion targetRot = Quaternion.LookRotation(direction, Vector3.up);

            // NON-AUTOROTATE PITCH/ROLL (biar tidak miring)
            targetRot = Quaternion.Euler(0, targetRot.eulerAngles.y, 0);

            transform.rotation = targetRot;
        }
    }

    //  COLLISION WITH PLAYER 
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerHealth.TakeDamage(1);
        }
    }

    //  DIE 
    public void Die()
    {
        Destroy(gameObject);
    }
}
