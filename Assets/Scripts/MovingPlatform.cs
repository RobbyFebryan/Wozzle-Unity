// Script untuk platform bergerak dengan waypoint system (Unity 3D)
// Pasang script ini pada GameObject platform

using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Pengaturan Waypoint")]
    public Transform[] waypoints;          // Array waypoint yang akan dilalui
    public float moveSpeed = 2f;           // Kecepatan gerakan
    public float waitTime = 1f;            // Waktu tunggu di setiap waypoint
    
    [Header("Opsi Gerakan")]
    public bool loop = true;               // Loop terus menerus
    public bool reverseAtEnd = false;      // Balik arah saat sampai waypoint terakhir
    public bool smoothRotation = false;    // Rotasi smooth menghadap arah gerakan
    
    private int currentWaypointIndex = 0;
    private bool isWaiting = false;
    private float waitTimer = 0f;
    private bool movingForward = true;

    void Start()
    {
        // Validasi waypoint
        if (waypoints.Length == 0)
        {
            Debug.LogError("Platform tidak memiliki waypoint! Tambahkan minimal 2 waypoint.");
            enabled = false;
            return;
        }
        
        // Pindahkan platform ke waypoint pertama
        transform.position = waypoints[0].position;
    }

    void Update()
    {
        if (waypoints.Length == 0) return;

        // Jika sedang menunggu di waypoint
        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                isWaiting = false;
                waitTimer = 0f;
            }
            return;
        }

        // Gerakkan platform ke waypoint target
        MovePlatform();
    }

    void MovePlatform()
    {
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = targetWaypoint.position - transform.position;
        
        // Rotasi smooth ke arah gerakan (opsional)
        if (smoothRotation && direction.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * moveSpeed);
        }
        
        // Gerakkan platform
        transform.position = Vector3.MoveTowards(
            transform.position, 
            targetWaypoint.position, 
            moveSpeed * Time.deltaTime
        );

        // Cek apakah sudah sampai waypoint
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.01f)
        {
            OnReachWaypoint();
        }
    }

    void OnReachWaypoint()
    {
        // Mulai timer tunggu
        isWaiting = true;
        
        // Tentukan waypoint selanjutnya
        if (reverseAtEnd)
        {
            // Mode bolak-balik
            if (movingForward)
            {
                currentWaypointIndex++;
                if (currentWaypointIndex >= waypoints.Length)
                {
                    currentWaypointIndex = waypoints.Length - 2;
                    movingForward = false;
                }
            }
            else
            {
                currentWaypointIndex--;
                if (currentWaypointIndex < 0)
                {
                    currentWaypointIndex = 1;
                    movingForward = true;
                }
            }
        }
        else
        {
            // Mode loop biasa
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length)
            {
                if (loop)
                {
                    currentWaypointIndex = 0;
                }
                else
                {
                    currentWaypointIndex = waypoints.Length - 1;
                    enabled = false; // Stop platform
                }
            }
        }
    }

    // Visualisasi waypoint di Editor
    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Gizmos.color = Color.cyan;
        
        // Gambar garis antar waypoint
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;
            
            // Gambar sphere di waypoint
            Gizmos.DrawWireSphere(waypoints[i].position, 0.3f);
            
            // Gambar nomor waypoint
            #if UNITY_EDITOR
            UnityEditor.Handles.Label(waypoints[i].position + Vector3.up * 0.5f, "WP " + i);
            #endif
            
            // Gambar garis ke waypoint berikutnya
            if (i < waypoints.Length - 1 && waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                
                // Gambar panah arah
                Vector3 direction = waypoints[i + 1].position - waypoints[i].position;
                Vector3 midPoint = waypoints[i].position + direction * 0.5f;
                DrawArrow(midPoint, direction.normalized * 0.5f);
            }
        }
        
        // Jika loop atau reverse, gambar garis kembali
        if (loop && waypoints.Length > 1 && waypoints[waypoints.Length - 1] != null && waypoints[0] != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(waypoints[waypoints.Length - 1].position, waypoints[0].position);
        }
        
        if (reverseAtEnd && waypoints.Length > 1)
        {
            Gizmos.color = Color.green;
            // Gambar garis bolak-balik
            for (int i = waypoints.Length - 1; i > 0; i--)
            {
                if (waypoints[i] != null && waypoints[i - 1] != null)
                {
                    Vector3 direction = waypoints[i - 1].position - waypoints[i].position;
                    Vector3 midPoint = waypoints[i].position + direction * 0.5f;
                    DrawArrow(midPoint, direction.normalized * 0.5f);
                }
            }
        }
    }

    void DrawArrow(Vector3 position, Vector3 direction)
    {
        Gizmos.DrawRay(position, direction);
        // Gambar ujung panah
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + 20, 0) * new Vector3(0, 0, 1);
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - 20, 0) * new Vector3(0, 0, 1);
        Gizmos.DrawRay(position + direction, right * 0.3f);
        Gizmos.DrawRay(position + direction, left * 0.3f);
    }
}