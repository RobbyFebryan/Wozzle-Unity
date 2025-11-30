using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.EventSystems;

public class GameOverMenu : MonoBehaviour
{
    [Header("UI References")]
    public CanvasGroup gameOverPanel;
    public RectTransform gameOverContent;
    public Button restartButton;
    public Button homeButton;

    [Header("Settings")]
    public float fadeDuration = 0.5f;
    public float hoverScale = 1.15f;     // Besaran scale saat hover
    public float clickScale = 0.9f;      // Besaran scale saat klik
    public float hoverDuration = 0.2f;   // Kecepatan animasi hover

    private bool isShown = false;

    private Vector3 originalScaleRestart;
    private Vector3 originalScaleHome;

    void Start()
    {
        // Simpan skala asli
        if (restartButton != null)
            originalScaleRestart = restartButton.transform.localScale;
        
        if (homeButton != null)
            originalScaleHome = homeButton.transform.localScale;

        // Kondisi awal
        if (gameOverPanel != null)
        {
            gameOverPanel.alpha = 0;
            gameOverPanel.interactable = false;
            gameOverPanel.blocksRaycasts = false;
            gameOverPanel.gameObject.SetActive(false);
        }

        if (gameOverContent != null)
            gameOverContent.localScale = Vector3.zero;

        if (restartButton != null)
            restartButton.transform.localScale = Vector3.zero;

        if (homeButton != null)
            homeButton.transform.localScale = Vector3.zero;

        // Tambahkan interaksi animasi ke tiap tombol
        if (restartButton != null)
            SetupButton(restartButton, originalScaleRestart, OnRestartClicked);
        
        if (homeButton != null)
            SetupButton(homeButton, originalScaleHome, OnHomeClicked);
    }

    // 🧩 Fungsi untuk membuat hover & click animation
    void SetupButton(Button button, Vector3 baseScale, UnityEngine.Events.UnityAction onClickAction)
    {
        button.onClick.AddListener(onClickAction);

        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = button.gameObject.AddComponent<EventTrigger>();

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

    // 💀 Show Game Over dengan animasi
    public void ShowGameOver()
    {
        if (isShown) return;
        isShown = true;

        Time.timeScale = 0f; // Pause game

        if (gameOverPanel != null)
        {
            gameOverPanel.gameObject.SetActive(true);
            gameOverPanel.alpha = 0;
            gameOverPanel.interactable = false;
            gameOverPanel.blocksRaycasts = false;
        }

        if (gameOverContent != null)
            gameOverContent.localScale = Vector3.zero;

        if (restartButton != null)
            restartButton.transform.localScale = Vector3.zero;

        if (homeButton != null)
            homeButton.transform.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence();
        seq.SetUpdate(true);

        // Fade in panel
        if (gameOverPanel != null)
            seq.Append(gameOverPanel.DOFade(1, fadeDuration));

        // Scale in content
        if (gameOverContent != null)
            seq.Append(gameOverContent.DOScale(1f, 0.6f).SetEase(Ease.OutBack));

        // Tombol muncul bersamaan
        seq.AppendCallback(() =>
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.interactable = true;
                gameOverPanel.blocksRaycasts = true;
            }

            if (restartButton != null)
                restartButton.transform.DOScale(originalScaleRestart, 0.5f)
                    .SetEase(Ease.OutBack)
                    .SetUpdate(true);

            if (homeButton != null)
                homeButton.transform.DOScale(originalScaleHome, 0.5f)
                    .SetEase(Ease.OutBack)
                    .SetUpdate(true);
        });

        // Tampilkan cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnRestartClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnHomeClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}