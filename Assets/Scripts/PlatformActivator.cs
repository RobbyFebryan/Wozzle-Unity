using UnityEngine;

public class PlatformActivator : MonoBehaviour
{
    public MovingPlatform movingPlatform;

    private void Start()
    {
        if (movingPlatform != null)
        {
            movingPlatform.enabled = false; // platform diam dulu
        }
    }

    public void ActivatePlatform(bool isActive)
    {
        if (movingPlatform != null)
        {
            movingPlatform.enabled = isActive;
        }
    }
}
