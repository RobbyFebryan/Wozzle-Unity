using UnityEngine;
using UnityEngine.SceneManagement;

public class WinManager : MonoBehaviour
{
    public GameObject winUI;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Time.timeScale = 0f;      // Pause game
            winUI.SetActive(true);    // Tampilkan UI Menang
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void NextScene()
    {
        // Kembalikan TimeScale agar scene baru berjalan normal
        Time.timeScale = 1f;

        // Ambil index scene saat ini
        int currentScene = SceneManager.GetActiveScene().buildIndex;

        // Pindah ke scene berikutnya
        SceneManager.LoadScene(currentScene + 1);
    }
}
