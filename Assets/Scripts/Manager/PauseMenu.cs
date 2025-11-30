using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    [Header("Pause Button")]
    public GameObject pauseButton;

    [Header("UI References")]
    public CanvasGroup pausePanel;
    public RectTransform pauseContent;
    public Button resumeButton;
    public Button restartButton;
    public Button homeButton;

    [Header("Settings")]
    public float fadeDuration = 0.5f;
    public float hoverScale = 1.15f;     // Besaran scale saat hover
    public float clickScale = 0.9f;      // Besaran scale saat klik
    public float hoverDuration = 0.2f;   // Kecepatan animasi hover

    private bool isPaused = false;

    private Vector3 originalScaleResume;
    private Vector3 originalScaleRestart;
    private Vector3 originalScaleHome;

    void Start()
    {
        // Simpan skala asli
        originalScaleResume = resumeButton.transform.localScale;
        originalScaleRestart = restartButton.transform.localScale;
        originalScaleHome = homeButton.transform.localScale;

        // Kondisi awal
        pausePanel.alpha = 0;
        pausePanel.gameObject.SetActive(false);
        pauseContent.localScale = Vector3.zero;

        resumeButton.transform.localScale = Vector3.zero;
        restartButton.transform.localScale = Vector3.zero;
        homeButton.transform.localScale = Vector3.zero;

        // Tambahkan interaksi animasi ke tiap tombol
        SetupButton(resumeButton, originalScaleResume, OnResumeClicked);
        SetupButton(restartButton, originalScaleRestart, OnRestartClicked);
        SetupButton(homeButton, originalScaleHome, OnHomeClicked);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    // 🧩 Fungsi untuk membuat hover & click animation
    void SetupButton(Button button, Vector3 baseScale, UnityEngine.Events.UnityAction onClickAction)
    {
        button.onClick.AddListener(onClickAction);

        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

        // Hover Masuk
        EventTrigger.Entry enter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
        enter.callback.AddListener((_) =>
        {
            button.transform.DOScale(baseScale * hoverScale, hoverDuration)
                .SetEase(Ease.OutBack)
                .SetUpdate(true);
        });
        trigger.triggers.Add(enter);

        // Hover Keluar
        EventTrigger.Entry exit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
        exit.callback.AddListener((_) =>
        {
            button.transform.DOScale(baseScale, hoverDuration)
                .SetEase(Ease.OutBack)
                .SetUpdate(true);
        });
        trigger.triggers.Add(exit);

        // Klik efek
        EventTrigger.Entry click = new EventTrigger.Entry { eventID = EventTriggerType.PointerClick };
        click.callback.AddListener((_) =>
        {
            Sequence clickSeq = DOTween.Sequence();
            clickSeq.Append(button.transform.DOScale(baseScale * clickScale, 0.1f)
                .SetEase(Ease.InOutSine)
                .SetUpdate(true));
            clickSeq.Append(button.transform.DOScale(baseScale, 0.2f)
                .SetEase(Ease.OutBack)
                .SetUpdate(true));
        });
        trigger.triggers.Add(click);
    }

    // 🕹️ Fungsi Pause Game
    public void PauseGame()
    {
        if (isPaused) return;
        isPaused = true;

        Time.timeScale = 0f; // Pause dulu

        pausePanel.gameObject.SetActive(true);
        pausePanel.alpha = 0;
        pauseContent.localScale = Vector3.zero;

        resumeButton.transform.localScale = Vector3.zero;
        restartButton.transform.localScale = Vector3.zero;
        homeButton.transform.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence();
        seq.SetUpdate(true);
        seq.Append(pausePanel.DOFade(1, fadeDuration));
        seq.Join(pauseContent.DOScale(1f, 0.6f).SetEase(Ease.OutBack));

        // Tombol muncul bersamaan
        seq.AppendCallback(() =>
        {
            resumeButton.transform.DOScale(originalScaleResume, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);
            restartButton.transform.DOScale(originalScaleRestart, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);
            homeButton.transform.DOScale(originalScaleHome, 0.5f).SetEase(Ease.OutBack).SetUpdate(true);
        });

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // 🟢 Resume Game
    public void ResumeGame()
    {
        if (!isPaused) return;
        isPaused = false;

        Sequence seq = DOTween.Sequence();
        seq.SetUpdate(true);
        seq.Append(pauseContent.DOScale(0f, 0.3f).SetEase(Ease.InBack));
        seq.Join(pausePanel.DOFade(0, 0.3f));
        seq.OnComplete(() =>
        {
            pausePanel.gameObject.SetActive(false);
            Time.timeScale = 1f;
        });
    }

    void OnResumeClicked() => ResumeGame();

    void OnRestartClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnHomeClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
