using UnityEngine;
using System.Collections;

public class PlatformManager : MonoBehaviour
{
    public float hideDelay = 2f;     // Waktu sebelum platform menghilang
    public float reappearDelay = 3f; // Waktu untuk muncul kembali

    private Collider col;
    private MeshRenderer mesh;

    void Start()
    {
        col = GetComponent<Collider>();
        mesh = GetComponent<MeshRenderer>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(HideAndReappear());
        }
    }

    private IEnumerator HideAndReappear()
    {
        // Tunggu sebelum platform hilang
        yield return new WaitForSeconds(hideDelay);

        // Nonaktifkan visual & collider
        mesh.enabled = false;
        col.enabled = false;

        // Tunggu sebelum platform aktif kembali
        yield return new WaitForSeconds(reappearDelay);

        // Aktifkan kembali platform
        mesh.enabled = true;
        col.enabled = true;
    }
}
