using UnityEngine;

public class DeathZone : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // Kalau benda (seperti box) jatuh
        RespawnableObject respawnable = other.GetComponent<RespawnableObject>();
        if (respawnable != null)
        {
            respawnable.Respawn();
        }
    }
}
