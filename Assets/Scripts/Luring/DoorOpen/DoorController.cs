using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Transform door;          // Referensi pintu
    public Transform openPosition;  // Titik posisi pintu saat terbuka
    public float openSpeed = 2f;    // Kecepatan geser

    private bool isOpening = false;
    private Vector3 closedPosition;

    void Start()
    {
        closedPosition = door.position;
    }

    void Update()
    {
        if (isOpening)
        {
            door.position = Vector3.Lerp(door.position, openPosition.position, Time.deltaTime * openSpeed);
        }
        else
        {
            door.position = Vector3.Lerp(door.position, closedPosition, Time.deltaTime * openSpeed);
        }
    }

    public void OpenDoor(bool state)
    {
        isOpening = state;
    }
}