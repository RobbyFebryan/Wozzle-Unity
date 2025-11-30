using UnityEngine;

public class HandleGrabHandler : MonoBehaviour
{
    [SerializeField] Animator animator;

    FixedJoint fixedJoint;

    Rigidbody rigidbody3D;

    NetworkPlayer networkPlayer;

    private void Awake()
    {
        networkPlayer = GetComponentInParent<NetworkPlayer>();
        rigidbody3D = GetComponent<Rigidbody>();

        rigidbody3D.solverIterations = 255;
    }

    public void UpdateState()
    {
        if (networkPlayer.IsGrabingActive)
        {
            animator.SetBool("isGrabing", true);
        }
        else
        {
            if(fixedJoint != null)
            {
                if(fixedJoint.connectedBody != null)
                {
                    float forceAmountMultiplier = 0.1f;

                    if(fixedJoint.connectedBody.transform.root.TryGetComponent( out NetworkPlayer otherPlayerNetworkPlayer))
                    {
                        if (otherPlayerNetworkPlayer.IsActiveRagdoll)
                            forceAmountMultiplier = 10;
                        else forceAmountMultiplier = 15;
                    }
                    fixedJoint.connectedBody.AddForce((networkPlayer.transform.forward + Vector3.up * 0.25f) * forceAmountMultiplier, ForceMode.Impulse);
                }
            Destroy(fixedJoint);
            }
        animator.SetBool("isCarrying", false);
        animator.SetBool("isGrabing", false);
        }
    }

    bool TryCarryObject (Collision collision)//
    {
        if(!networkPlayer.Object.HasStateAuthority)
            return false;

        if(!networkPlayer.IsActiveRagdoll)
            return false;

        if (!networkPlayer.IsGrabingActive)
            return false;

        if(fixedJoint != null)
            return false;

        if(collision.transform.root == networkPlayer.transform) 
            return false;

        if(!collision.collider.TryGetComponent(out Rigidbody otherObjectRigidBody))
            return false;

        fixedJoint = transform.gameObject.AddComponent<FixedJoint>();

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
