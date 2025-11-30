using UnityEngine;

public class IgnoreCollision : MonoBehaviour
{
    [SerializeField] private Collider thisCollider;
    [SerializeField] private Collider[] ColliderToIgnore;

    private void Start()
    {
        foreach (Collider otherCollider in ColliderToIgnore)
        { 
            Physics.IgnoreCollision(thisCollider, otherCollider, true);
        }
    }
}
