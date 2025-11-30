using UnityEngine;

public class PlayerGrab : MonoBehaviour
{
    [Header("Grab Settings")]
    [SerializeField] private Transform leftHandPoint;
    [SerializeField] private Transform rightHandPoint;
    [SerializeField] private float grabRange = 1f;
    [SerializeField] private LayerMask grabLayer;

    [Header("Controls")]
    [SerializeField] private KeyCode leftGrabKey = KeyCode.Mouse0;   // klik kiri untuk tangan kiri
    [SerializeField] private KeyCode rightGrabKey = KeyCode.Mouse1;  // klik kanan untuk tangan kanan

    private FixedJoint leftJoint;
    private FixedJoint rightJoint;
    private Rigidbody leftGrabbedObject;
    private Rigidbody rightGrabbedObject;

    private void Update()
    {
        // Debug garis ray tangan kiri & kanan
        Debug.DrawRay(leftHandPoint.position, leftHandPoint.forward * grabRange, Color.red);
        Debug.DrawRay(rightHandPoint.position, rightHandPoint.forward * grabRange, Color.blue);

        // Tangan kiri
        if (Input.GetKeyDown(leftGrabKey))
            TryGrab(leftHandPoint, ref leftJoint, ref leftGrabbedObject, "LEFT");

        if (Input.GetKeyUp(leftGrabKey))
            ReleaseGrab(ref leftJoint, ref leftGrabbedObject, "LEFT");

        // Tangan kanan
        if (Input.GetKeyDown(rightGrabKey))
            TryGrab(rightHandPoint, ref rightJoint, ref rightGrabbedObject, "RIGHT");

        if (Input.GetKeyUp(rightGrabKey))
            ReleaseGrab(ref rightJoint, ref rightGrabbedObject, "RIGHT");
    }

    private void TryGrab(Transform handPoint, ref FixedJoint joint, ref Rigidbody grabbedObj, string handName)
    {
        Ray ray = new Ray(handPoint.position, handPoint.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, grabRange, grabLayer))
        {
            Rigidbody targetRb = hit.collider.attachedRigidbody;
            if (targetRb != null)
            {
                grabbedObj = targetRb;

                // tambahin joint di tangan
                joint = handPoint.gameObject.AddComponent<FixedJoint>();
                joint.connectedBody = grabbedObj;

                joint.breakForce = 1000f;
                joint.breakTorque = 1000f;

                Debug.Log($"{handName} HAND grabbed: {targetRb.name}");
            }
            else
            {
                Debug.Log($"{handName} HAND hit object but no Rigidbody: {hit.collider.name}");
            }
        }
        else
        {
            Debug.Log($"{handName} HAND tried to grab but found nothing.");
        }
    }

    private void ReleaseGrab(ref FixedJoint joint, ref Rigidbody grabbedObj, string handName)
    {
        if (joint != null)
        {
            Debug.Log($"{handName} HAND released: {(grabbedObj != null ? grabbedObj.name : "Nothing")}");
            Destroy(joint);
            grabbedObj = null;
        }
    }

    // Debug gizmo supaya kelihatan di Scene view
    private void OnDrawGizmos()
    {
        if (leftHandPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(leftHandPoint.position, leftHandPoint.position + leftHandPoint.forward * grabRange);
        }
        if (rightHandPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(rightHandPoint.position, rightHandPoint.position + rightHandPoint.forward * grabRange);
        }
    }
}