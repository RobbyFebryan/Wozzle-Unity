using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    public PlayerHealth playerHealth;

    void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Player")) 
        {
            playerHealth.TakeDamage(1);
        }
    }
}
