using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    [Header("Cutscene Panels (Urutkan)")]
    public GameObject[] cutscenePanels;

    [Header("Fade Duration")]
    public float fadeDuration = 0.5f;

    [Header("Auto Skip Delay (detik)")]
    public float autoSkipTime = 2f;

    [Header("Skip Buttons (boleh lebih dari 1)")]
    public Button[] panelSkipButtons;  // ← banyak tombol skip panel

    [Header("Skip langsung ke scene berikutnya")]
    public Button skipSceneButton;     // ← skip seluruh cutscene

    [Header("Scene Tujuan")]
    public string nameNextScene;

    private int currentIndex = -1;
    private bool isTransitioning = false;
    private bool skipRequested = false;
    private bool skipSceneRequested = false;

    void Start()
    {
        foreach (var p in cutscenePanels)
            p.SetActive(false);

        // === REGISTER MULTI SKIP PANEL BUTTON ===
        if (panelSkipButtons != null)
        {
            foreach (var btn in panelSkipButtons)
            {
                btn.onClick.AddListener(SkipPanelDelay);
            }
        }

        // === REGISTER SKIP TO SCENE BUTTON ===
        if (skipSceneButton != null)
        {
            skipSceneButton.onClick.AddListener(SkipSceneDirect);
        }

        ShowNextPanel();
    }

    // Dipanggil ketika tombol skip panel ditekan
    private void SkipPanelDelay()
    {
        skipRequested = true;
    }

    // Dipanggil ketika skip langsung ke scene
    private void SkipSceneDirect()
    {
        skipSceneRequested = true;
        StopAllCoroutines();
        StartCoroutine(FadeOutAndEnd());
    }

    public void ShowNextPanel()
    {
        if (isTransitioning) return;

        currentIndex++;

        if (currentIndex >= cutscenePanels.Length)
        {
            StartCoroutine(FadeOutAndEnd());
            return;
        }

        StartCoroutine(TransitionToPanel(currentIndex));
    }

    private IEnumerator TransitionToPanel(int index)
    {
        isTransitioning = true;
        skipRequested = false;

        if (index > 0)
        {
            yield return StartCoroutine(FadePanel(cutscenePanels[index - 1], false));
            cutscenePanels[index - 1].SetActive(false);
        }

        cutscenePanels[index].SetActive(true);
        yield return StartCoroutine(FadePanel(cutscenePanels[index], true));

        // Tunggu auto skip atau skip button
        float timer = 0f;
        while (timer < autoSkipTime && !skipRequested && !skipSceneRequested)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (skipSceneRequested)
            yield break; // langsung keluar karena skipSceneButton ditekan

        isTransitioning = false;
        ShowNextPanel();
    }

    private IEnumerator FadePanel(GameObject panel, bool fadeIn)
    {
        CanvasGroup cg = panel.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = panel.AddComponent<CanvasGroup>();

        float start = fadeIn ? 0 : 1;
        float end = fadeIn ? 1 : 0;

        cg.alpha = start;

        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, end, t / fadeDuration);
            yield return null;
        }

        cg.alpha = end;
    }

    private IEnumerator FadeOutAndEnd()
    {
        Debug.Log("Mengakhiri cutscene...");

        GameObject fadeObj = new GameObject("FadeOut");
        fadeObj.transform.SetParent(transform);

        Image fadeImg = fadeObj.AddComponent<Image>();
        fadeImg.color = new Color(0, 0, 0, 0);

        RectTransform rt = fadeObj.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeImg.color = new Color(0, 0, 0, Mathf.Lerp(0, 1, t / fadeDuration));
            yield return null;
        }

        SceneManager.LoadScene(nameNextScene);
    }
}
