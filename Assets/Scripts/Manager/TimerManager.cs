using UnityEngine;
using UnityEngine.UI;

public class TimerManager : MonoBehaviour
{
    [Header("Timer Settings")]
    public float startTime = 10f;      // Waktu awal timer (bisa diatur)
    private float currentTime;

    [Header("UI")]
    public Slider timerSlider;         // Slider untuk menampilkan waktu
    public bool autoStart = true;      // Timer mulai otomatis

    private bool isRunning = false;

    private void Start()
    {
        currentTime = startTime;

        if (timerSlider != null)
        {
            timerSlider.maxValue = startTime;
            timerSlider.value = startTime;
        }

        if (autoStart)
            StartTimer();
    }

    private void Update()
    {
        if (!isRunning) return;

        currentTime -= Time.deltaTime;

        if (timerSlider != null)
            timerSlider.value = currentTime;

        if (currentTime <= 0)
        {
            currentTime = 0;
            isRunning = false;
            TimerFinished();
        }
    }

    public void StartTimer()
    {
        currentTime = startTime;
        if (timerSlider != null)
        {
            timerSlider.maxValue = startTime;
            timerSlider.value = startTime;
        }
        isRunning = true;
    }

    
    // Fungsi jika timer selesai
    private void TimerFinished()
    {
        Debug.Log("Timer selesai!");

        //Taro UI Lose Disini
    }

    
    // Fungsi reset timer
    public void ResetTimer()
    {
        currentTime = startTime;
        isRunning = false;

        if (timerSlider != null)
            timerSlider.value = startTime;
    }

    
    // Fungsi pause timer
    public void PauseTimer()
    {
        isRunning = false;
    }

    
    // Fungsi resume timer
    public void ResumeTimer()
    {
        isRunning = true;
    }
}
