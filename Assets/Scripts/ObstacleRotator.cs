using UnityEngine;

public class ObstacleRotator : MonoBehaviour
{
    [Header("Rotasi Per Detik")]
    public Vector3 rotationSpeed;

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
