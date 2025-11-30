using UnityEngine;

public class CannonballCollision : MonoBehaviour
{
    private CannonFSM cannon;
    private bool hasHit = false;

    public void SetCannon(CannonFSM cannonReference)
    {
        cannon = cannonReference;
    }

    private void Start()
    {
        Destroy(gameObject, 10f); // auto destroy
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit)
            return;

        hasHit = true;

        // Jika kena player → kasih damage
        if (other.CompareTag("Player"))
        {
            PlayerHealth hp = other.GetComponent<PlayerHealth>();
            if (hp != null)
            {
                hp.TakeDamage(1);
                Debug.Log("Cannonball kena player!");
            }
        }

        // Apapun yang kena → cannonball hancur
        Destroy(gameObject);
    }
}
