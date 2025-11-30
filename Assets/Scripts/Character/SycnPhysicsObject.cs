using UnityEngine;

public class SycnPhysicsObject : MonoBehaviour
{
    private Rigidbody rigidBody3D;
    private ConfigurableJoint joint;
    [SerializeField] Rigidbody animatedRigidBody3D;
    [SerializeField] bool syncAnimation = false;
    Quaternion startLocalRotation;
    float startSlerpPositionSpring = 0.0f;

    private void Awake()
    {
       rigidBody3D = GetComponent<Rigidbody>();
       joint = GetComponent<ConfigurableJoint>();

        startLocalRotation = transform.localRotation;
        startSlerpPositionSpring = joint.slerpDrive.positionSpring;
    }
   
    public void UpdateJointFromAnimation()
    {
        if (!syncAnimation) 
            return;

        ConfigurableJointExtensions.SetTargetRotationLocal(joint, animatedRigidBody3D.transform.localRotation, startLocalRotation); 
    }

    public void MakeRagdoll()
    {
        JointDrive jointDrive = joint.slerpDrive;
        jointDrive.positionSpring = 1;
        joint.slerpDrive = jointDrive;
    }

    public void MakeActiveRagdoll()
    {
        JointDrive jointDrive = joint.slerpDrive;
        jointDrive.positionSpring = startSlerpPositionSpring;
        joint.slerpDrive = jointDrive;
    }
}
