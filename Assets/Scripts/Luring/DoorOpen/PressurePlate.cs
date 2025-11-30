using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public DoorController doorController;

    private Renderer plateRenderer;
    private int objectCount = 0;

    private void Start()
    {
        // Ambil komponen Renderer dari objek ini
        plateRenderer = GetComponent<Renderer>();

        // Warna awal: merah (belum tertekan)
        plateRenderer.material.color = Color.red;
    }

    private void OnTriggerEnter(Collider other)
    {
        objectCount++;

        // Buka pintu
        doorController.OpenDoor(true);

        // Ganti warna jadi hijau
        plateRenderer.material.color = Color.green;
    }

    private void OnTriggerExit(Collider other)
    {
        objectCount--;

        // Kalau sudah tidak ada benda di atasnya, pintu menutup dan warna kembali merah
        if (objectCount <= 0)
        {
            doorController.OpenDoor(false);
            plateRenderer.material.color = Color.red;
        }
    }
}
