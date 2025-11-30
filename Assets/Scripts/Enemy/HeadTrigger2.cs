using UnityEngine;

public class HeadTrigger2 : MonoBehaviour
{
    private EnemyFollow enemyAI;

    void Start()
    {
        enemyAI = GetComponentInParent<EnemyFollow>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Ambil Rigidbody player dengan benar
            Rigidbody playerRb = other.gameObject.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                // Ambil velocity player
                Vector3 vel = playerRb.linearVelocity;

                // Pastikan player datang dari atas (kecepatan Y negatif)
                if (vel.y < 0f)
                {
                    // Matikan musuh
                    enemyAI.Die();

                    // Bounce player ke atas
                    playerRb.linearVelocity = new Vector3(vel.x, 7f, vel.z);
                }
            }
        }
    }
}
