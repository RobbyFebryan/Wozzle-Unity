using UnityEngine;
using System.Collections;

public class DetectCollision : MonoBehaviour
{
    NetworkPlayer networkPlayer;
    Rigidbody hitRigidBody;

    private ContactPoint[] contactPoints = new ContactPoint[5];

    private void Awake()
    {
        networkPlayer = GetComponentInParent<NetworkPlayer>();
        hitRigidBody = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!networkPlayer.HasStateAuthority)
            return;

        if (!networkPlayer.IsActiveRagdoll)
            return;

        if (!collision.collider.CompareTag("CauseDamage"))
            return;


        //Avoid having the player hurting themselves
        if (collision.collider.transform.root == networkPlayer.transform)
            return;

        int numberOfContacts = collision.GetContacts(contactPoints);

        for (int i = 0; i < numberOfContacts; i++)
        {
            ContactPoint contactPoint = contactPoints[i];

            Vector3 contactImpulse = contactPoint.impulse / Time.fixedDeltaTime;
            
            if(contactImpulse.magnitude < 15)
                continue;

            networkPlayer.OnPlayerBodyPartHit();

            Vector3 forceDirection = (contactImpulse + Vector3.up) * 0.5f;

            forceDirection = Vector3.ClampMagnitude(forceDirection, 30);

            Debug.DrawRay(hitRigidBody.position, forceDirection * 40, Color.red, 4);

            hitRigidBody.AddForce(forceDirection, ForceMode.Impulse);
        }
    }
}
