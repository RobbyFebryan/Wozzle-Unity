using UnityEngine;
using TMPro; 

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Object References")]
    public GameObject PrisonTrigger;   // Trigger yang akan aktif setelah key = 3

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI keyText;

    private int currentKey = 0;
    private int currentScore = 0;

    // Public getter untuk membaca jumlah key dari script lain
    public int CurrentKey => currentKey;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        UpdateScoreUI();
        UpdateKeyUI();
    }

    private void Update()
    {
        // Jika key sudah 3, aktifkan trigger,
        // tapi TIDAK merotasi pintu di sini (biarkan PrisonTrigger yang handle animasi)
        if (currentKey >= 3)
        {
            if (!PrisonTrigger.activeSelf)
                PrisonTrigger.SetActive(true);
        }
    }

    public void AddScore(int amount)
    {
        currentScore += amount;
        UpdateScoreUI();
    }

    public void AddKey(int keyAmount)
    {
        currentKey += keyAmount;
        UpdateKeyUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString("D9");
        }
    }

    private void UpdateKeyUI() 
    {
        if (keyText != null)
        {
            keyText.text = currentKey.ToString() + " / 3";
        }
    }
}
