using UnityEngine;

public class RotateObjectLuring : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rigidbody3D;

    [SerializeField]
    private Vector3 rotationAmount;

    private void FixedUpdate()
    {
        Vector3 rotateBy = transform.rotation.eulerAngles + rotationAmount * Time.deltaTime;

        if (rigidbody3D != null)
        {
            rigidbody3D.MoveRotation(Quaternion.Euler(rotateBy));
        }
        else
        {
            transform.rotation = Quaternion.Euler(rotateBy);
        }
    }
}
