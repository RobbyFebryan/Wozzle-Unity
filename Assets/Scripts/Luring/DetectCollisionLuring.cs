using UnityEngine;

public class DetectCollisionLuring : MonoBehaviour
{
    private PlayerLuring playerController;
    private Rigidbody hitRigidBody;

    private ContactPoint[] contactPoints = new ContactPoint[5];

    private void Awake()
    {
        playerController = GetComponentInParent<PlayerLuring>();
        hitRigidBody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (playerController == null)
            return;

        // Jangan proses kalau ragdoll lagi nonaktif
        if (!playerController.enabled)
            return;

        // Cuma reaksi kalau objek penabrak punya tag "CauseDamage"
        if (!collision.collider.CompareTag("CauseDamage"))
            return;

        // Hindari self-hit (tabrakan sama tubuh sendiri)
        if (collision.collider.transform.root == playerController.transform)
            return;

        int numberOfContacts = collision.GetContacts(contactPoints);

        for (int i = 0; i < numberOfContacts; i++)
        {
            ContactPoint contactPoint = contactPoints[i];
            Vector3 contactImpulse = contactPoint.normal * collision.relativeVelocity.magnitude;

            // Jika impact terlalu kecil, abaikan
            if (contactImpulse.magnitude < 15f)
                continue;

            // Aktifkan ragdoll
            playerController.OnPlayerBodyPartHit();

            // Tambahkan gaya pantulan/efek fisik
            Vector3 forceDirection = (contactImpulse + Vector3.up) * 0.5f;
            forceDirection = Vector3.ClampMagnitude(forceDirection, 30f);

            Debug.DrawRay(hitRigidBody.position, forceDirection * 2f, Color.red, 4f);

            hitRigidBody.AddForce(forceDirection, ForceMode.Impulse);
        }
    }
}
