using UnityEngine;

public class HandleGrabHandlerLuring : MonoBehaviour
{
    [SerializeField] Animator animator;

    private FixedJoint fixedJoint;
    private Rigidbody rigidbody3D;
    private PlayerLuring playerLuring;

    private void Awake()
    {
        playerLuring = GetComponentInParent<PlayerLuring>();
        rigidbody3D = GetComponent<Rigidbody>();

        rigidbody3D.solverIterations = 255;
    }

    public void UpdateState()
    {
        if (playerLuring == null)
            return;

        if (playerLuring.IsGrabingActive)
        {
            animator.SetBool("isGrabing", true);
        }
        else
        {
            // Lepas objek kalau ada joint
            if (fixedJoint != null)
            {
                if (fixedJoint.connectedBody != null)
                {
                    float forceAmountMultiplier = 4f;
                }

                Destroy(fixedJoint);
            }

            animator.SetBool("isCarrying", false);
            animator.SetBool("isGrabing", false);
        }
    }

    private bool TryCarryObject(Collision collision)
    {
        if (!playerLuring.IsActiveRagdoll)
            return false;

        if (!playerLuring.IsGrabingActive)
            return false;

        if (fixedJoint != null)
            return false;

        if (collision.transform.root == playerLuring.transform)
            return false;

        if (!collision.collider.TryGetComponent(out Rigidbody otherObjectRigidBody))
            return false;

        fixedJoint = gameObject.AddComponent<FixedJoint>();
        fixedJoint.connectedBody = otherObjectRigidBody;
        fixedJoint.autoConfigureConnectedAnchor = false;
        fixedJoint.connectedAnchor = collision.transform.InverseTransformPoint(collision.GetContact(0).point);

        animator.SetBool("isCarrying", true);

        return true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryCarryObject(collision);
    }
}
