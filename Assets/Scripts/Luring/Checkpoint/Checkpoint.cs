using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public float heightOffset = 1f; // Tinggi spawn di atas cube

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 spawnPos = transform.position + Vector3.up * heightOffset;
            CheckpointManager.Instance.SetCheckpoint(spawnPos);
        }
    }
}
