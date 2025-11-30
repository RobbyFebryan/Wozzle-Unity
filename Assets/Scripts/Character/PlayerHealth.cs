using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;

    private bool isInvincible = false;
    public float invincibleDuration = 2f;

    [Header("UI")]
    public RawImage[] healthUIs;

    [Header("Game Over")]
    public GameObject gameOverPanel;

    private PlayerLuringJoystick playerController;
    //public Animator anim;

    [Header("Blinking Objects")]
    public List<GameObject> blinkObjects = new List<GameObject>();
    public float blinkInterval = 0.15f; // kecepatan kelap-kelip

    [Header("SFX")]
    [SerializeField] private int sfxIndex = 1;


    void Start()
    {
        currentHealth = maxHealth;
        //anim = GetComponent<Animator>();

        playerController = GetComponent<PlayerLuringJoystick>();
        if (playerController == null)
            Debug.LogWarning("PlayerLuringJoystick tidak ditemukan di Player!");

        if (healthUIs.Length != maxHealth)
        {
            Debug.LogError("Jumlah elemen Health UI tidak sama dengan Max Health!");
        }

        // Pastikan panel game over mati di awal
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
            CanvasGroup cg = gameOverPanel.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.alpha = 0f;
                cg.interactable = false;
                cg.blocksRaycasts = false;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        //anim.SetTrigger("isDamaged");
        currentHealth -= damage;
        Debug.Log("Player kena damage! Sisa darah: " + currentHealth);

        UpdateHealthUI();

        StartCoroutine(InvincibilityTimer());
        StartCoroutine(BlinkObjectsForDuration(invincibleDuration));

        if (currentHealth <= 0)
            Die();
    }

    void UpdateHealthUI()
    {
        int livesLost = maxHealth - currentHealth;
        int indexToDisable = livesLost - 1;

        if (indexToDisable >= 0 && indexToDisable < healthUIs.Length)
        {
            healthUIs[indexToDisable].enabled = false;
        }
    }

    private IEnumerator InvincibilityTimer()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleDuration);
        isInvincible = false;
    }

    // ==========================
    // BLINK EFFECT
    // ==========================
    private IEnumerator BlinkObjectsForDuration(float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            foreach (var obj in blinkObjects)
            {
                if (obj != null)
                    obj.SetActive(!obj.activeSelf);
            }

            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }

        // Pastikan semua objek aktif kembali
        foreach (var obj in blinkObjects)
        {
            if (obj != null)
                obj.SetActive(true);
        }
    }


    void Die()
    {
        Debug.Log("PLAYER MATI!");

        // Matikan semua icon HP saat mati
        foreach (var healthUI in healthUIs)
        {
            if (healthUI != null)
                healthUI.enabled = false;
        }

        StartCoroutine(ShowGameOverWithDelay(1f));
    }

    private IEnumerator ShowGameOverWithDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        if (gameOverPanel == null) yield break;

        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);

        gameOverPanel.transform.localScale = Vector3.one;

        CanvasGroup cg = gameOverPanel.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = 1f;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }

        foreach (Transform child in gameOverPanel.transform)
            child.localScale = Vector3.one;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            TakeDamage(1);
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Void"))
        {
            currentHealth = 0;
            UpdateHealthUI();
            Die();
        }
    }
}
