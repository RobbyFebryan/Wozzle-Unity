using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class StylizedMainMenu : MonoBehaviour
{
    [Header("UI References")]
    public CanvasGroup backgroundPanel;
    public Button playButton;
    public Button optionsButton;
    public Button creditsButton;
    public Button exitButton;

    [Header("Settings Panel")]
    public CanvasGroup settingsPanel;
    public RectTransform settingsContent;
    public Button closeSettingsButton;

    [Header("Credits Panel")]
    public CanvasGroup creditsPanel;
    public RectTransform creditsContent;
    public Button closeCreditsButton;

    [Header("Settings")]
    public float fadeDuration = 1f;
    public float buttonDelay = 0.2f;

    // Simpan skala asli tombol
    private Vector3 originalScalePlay;
    private Vector3 originalScaleOptions;
    private Vector3 originalScaleCredits;
    private Vector3 originalScaleExit;

    void Start()
    {
        // Simpan skala asli dari Scene (misal scale = 2)
        originalScalePlay = playButton.transform.localScale;
        originalScaleOptions = optionsButton.transform.localScale;
        originalScaleCredits = creditsButton.transform.localScale;
        originalScaleExit = exitButton.transform.localScale;

        // Mulai dari kondisi tersembunyi
        backgroundPanel.alpha = 0;
        playButton.transform.localScale = Vector3.zero;
        optionsButton.transform.localScale = Vector3.zero;
        creditsButton.transform.localScale = Vector3.zero;
        exitButton.transform.localScale = Vector3.zero;

        // Panel Settings & Credits disembunyikan dulu
        settingsPanel.alpha = 0;
        settingsPanel.gameObject.SetActive(false);
        settingsContent.localScale = Vector3.zero;

        creditsPanel.alpha = 0;
        creditsPanel.gameObject.SetActive(false);
        creditsContent.localScale = Vector3.zero;

        // Main sequence animasi
        Sequence seq = DOTween.Sequence();
        seq.Append(backgroundPanel.DOFade(1, fadeDuration));
        seq.AppendInterval(0.2f);
        seq.Append(playButton.transform.DOScale(originalScalePlay, 0.5f).SetEase(Ease.OutBack));
        seq.AppendInterval(buttonDelay);
        seq.Append(optionsButton.transform.DOScale(originalScaleOptions, 0.5f).SetEase(Ease.OutBack));
        seq.AppendInterval(buttonDelay);
        seq.Append(creditsButton.transform.DOScale(originalScaleCredits, 0.5f).SetEase(Ease.OutBack));
        seq.AppendInterval(buttonDelay);
        seq.Append(exitButton.transform.DOScale(originalScaleExit, 0.5f).SetEase(Ease.OutBack));
        seq.Play();

        // Efek hover
        AddButtonHoverEffect(playButton, originalScalePlay);
        AddButtonHoverEffect(optionsButton, originalScaleOptions);
        AddButtonHoverEffect(creditsButton, originalScaleCredits);
        AddButtonHoverEffect(exitButton, originalScaleExit);

        // Fungsi tombol
        playButton.onClick.AddListener(OnPlayClicked);
        optionsButton.onClick.AddListener(OnOptionsClicked);
        creditsButton.onClick.AddListener(OnCreditsClicked);
        exitButton.onClick.AddListener(OnExitClicked);
        closeSettingsButton.onClick.AddListener(OnCloseSettingsClicked);
        closeCreditsButton.onClick.AddListener(OnCloseCreditsClicked);
    }

    void AddButtonHoverEffect(Button button, Vector3 originalScale)
    {
        var rect = button.GetComponent<RectTransform>();
        EventTriggerListener.Get(button.gameObject).onEnter = (go) =>
        {
            rect.DOScale(originalScale * 1.1f, 0.2f).SetEase(Ease.OutBack);
        };
        EventTriggerListener.Get(button.gameObject).onExit = (go) =>
        {
            rect.DOScale(originalScale, 0.2f).SetEase(Ease.OutBack);
        };
    }

    void OnPlayClicked()
    {
        backgroundPanel.DOFade(0, 0.5f).OnComplete(() =>
        {
            SceneManager.LoadSceneAsync(1);
        });
    }

    void OnOptionsClicked()
    {
        ShowSettingsPanel();
    }

    void OnCreditsClicked()
    {
        ShowCreditsPanel();
    }

    void OnExitClicked()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }

    void ShowSettingsPanel()
    {
        settingsPanel.gameObject.SetActive(true);
        settingsPanel.alpha = 0;
        settingsContent.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence();
        seq.Append(settingsPanel.DOFade(1, 0.3f));
        seq.Join(settingsContent.DOScale(1f, 0.4f).SetEase(Ease.OutBack));
    }

    void OnCloseSettingsClicked()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(settingsContent.DOScale(0f, 0.3f).SetEase(Ease.InBack));
        seq.Join(settingsPanel.DOFade(0, 0.3f));
        seq.OnComplete(() => settingsPanel.gameObject.SetActive(false));
    }

    void ShowCreditsPanel()
    {
        creditsPanel.gameObject.SetActive(true);
        creditsPanel.alpha = 0;
        creditsContent.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence();
        seq.Append(creditsPanel.DOFade(1, 0.3f));
        seq.Join(creditsContent.DOScale(1f, 0.4f).SetEase(Ease.OutBack));
    }

    void OnCloseCreditsClicked()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(creditsContent.DOScale(0f, 0.3f).SetEase(Ease.InBack));
        seq.Join(creditsPanel.DOFade(0, 0.3f));
        seq.OnComplete(() => creditsPanel.gameObject.SetActive(false));
    }
}
