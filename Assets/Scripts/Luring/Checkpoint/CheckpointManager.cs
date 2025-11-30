using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance;
    public Vector3 currentCheckpoint;
    public bool hasCheckpoint = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCheckpoint(Vector3 position)
    {
        currentCheckpoint = position;
        hasCheckpoint = true;
        Debug.Log("Checkpoint disimpan di: " + position);
    }

    public Vector3 GetRespawnPosition()
    {
        return currentCheckpoint;
    }
}
