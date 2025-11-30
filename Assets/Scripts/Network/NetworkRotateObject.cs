using UnityEngine;
using Fusion;

public class NetworkRotateObject : NetworkBehaviour
{
    [SerializeField]
    Rigidbody Rigidbody3D;

    [SerializeField]
    Vector3 rotationAmount;


    void Start()
    {
        
    }


    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            Vector3 rotateBy = transform.rotation.eulerAngles + rotationAmount*Runner.DeltaTime;

            if (Rigidbody3D != null)
            {
                Rigidbody3D.MoveRotation(Quaternion.Euler(rotateBy));
            }
            else
            {
                transform.rotation = Quaternion.Euler(rotateBy);
            }
        }
        
    }
}
