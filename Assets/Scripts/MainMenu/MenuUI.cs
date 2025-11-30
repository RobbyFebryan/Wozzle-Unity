using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    // Deklarasi tombol
    private Button playButton;
    private Button settingsButton;
    private Button exitButton;
    private Button closeButton;
    private VisualElement settingsParent;


    // Method Start dipanggil saat skrip diaktifkan
    void Start()
    {
        
        // Mendapatkan VisualElement root dari UIDocument
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        // Mendapatkan referensi tombol berdasarkan nama/query
        playButton = root.Q<Button>("PlayButton");
        settingsButton = root.Q<Button>("SettingsButton");
        exitButton = root.Q<Button>("ExitButton");
        closeButton = root.Q<Button>("CloseSettingsButton");
        settingsParent = root.Q<VisualElement>("SettingsParent");

        // Mendaftarkan callback (fungsi) yang akan dipanggil saat tombol diklik
        playButton.RegisterCallback<ClickEvent>(OnPlayButtonClicked);
        settingsButton.RegisterCallback<ClickEvent>(OnSettingsButtonClicked);
        exitButton.RegisterCallback<ClickEvent>(OnExitButtonClicked);
        closeButton.RegisterCallback<ClickEvent>(OnCloseButtonClicked);

        settingsParent.AddToClassList("SettingsDown");
    }

    // Fungsi yang dipanggil saat tombol Play diklik
    private void OnPlayButtonClicked(ClickEvent clickEvent)
    {
        SceneManager.LoadScene("Gameplay R");
    }

    // Fungsi yang dipanggil saat tombol Settings diklik
    private void OnSettingsButtonClicked(ClickEvent clickEvent)
    {
        settingsParent.RemoveFromClassList("SettingsDown");
    }

    // Fungsi yang dipanggil saat tombol Quit diklik
    private void OnExitButtonClicked(ClickEvent clickEvent)
    {
        Application.Quit();
    }

        private void OnCloseButtonClicked(ClickEvent clickEvent)
    {
        settingsParent.AddToClassList("SettingsDown");
    }

}
