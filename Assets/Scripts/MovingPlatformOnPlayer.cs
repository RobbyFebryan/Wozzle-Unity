using UnityEngine;

public class MovingPlatformOnPlayer : MonoBehaviour
{
    public MovingPlatform movingPlatform;
    public string playerTag = "Player";

    [Header("Behavior Options")]
    public bool stopWhenPlayerLeaves = true;  
    // true  → platform berhenti ketika player turun
    // false → platform terus berjalan meski player turun

    private int playerCount = 0;

    private void Start()
    {
        if (movingPlatform != null)
        {
            movingPlatform.enabled = false; // platform diam dulu
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerCount++;
            if (movingPlatform != null)
                movingPlatform.enabled = true;   // mulai gerak
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerCount--;

            if (playerCount <= 0 && stopWhenPlayerLeaves)
            {
                if (movingPlatform != null)
                    movingPlatform.enabled = false;  // stop gerak
            }
        }
    }
}
