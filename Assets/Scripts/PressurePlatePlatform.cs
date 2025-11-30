using UnityEngine;

public class PressurePlatePlatform : MonoBehaviour
{
    public PlatformActivator platformActivator;

    private Renderer plateRenderer;
    private int objectCount = 0;

    private void Start()
    {
        plateRenderer = GetComponent<Renderer>();
        plateRenderer.material.color = Color.red; // awal: tidak aktif
    }

    private void OnTriggerEnter(Collider other)
    {
        objectCount++;

        platformActivator.ActivatePlatform(true);   // mulai bergerak
        plateRenderer.material.color = Color.green; // warna aktif
    }

    private void OnTriggerExit(Collider other)
    {
        objectCount--;

        if (objectCount <= 0)
        {
            platformActivator.ActivatePlatform(false); // berhenti
            plateRenderer.material.color = Color.red;  // warna mati
        }
    }
}
