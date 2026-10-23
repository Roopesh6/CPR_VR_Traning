using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CPRSession : MonoBehaviour
{
    public static CPRSession Instance { get; private set; }
    public event Action<int, float> CompressionRegistered;

    public int CompressionCount { get; private set; }
    public float CurrentRate { get; private set; }
    public float SessionSeconds { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsTutorial => SceneManager.GetActiveScene().name == "Tutorial";

    private float lastCompression = -1f;
    private float rateWindowStart;
    private int rateWindowCount;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Update()
    {
        if (IsActive) SessionSeconds += Time.deltaTime;
        if (IsActive && Input.GetKeyDown(KeyCode.Space)) RegisterCompression();
        if (IsActive && SceneManager.GetActiveScene().name == "Testing" && SessionSeconds >= 60f)
            FinishTest();
    }

    public void Begin()
    {
        CompressionCount = 0; SessionSeconds = 0; CurrentRate = 0;
        lastCompression = -1; rateWindowStart = Time.time; rateWindowCount = 0;
        IsActive = true;
    }

    public void RegisterCompression()
    {
        if (!IsActive) return;
        CompressionCount++;
        if (lastCompression >= 0)
        {
            float interval = Time.time - lastCompression;
            if (interval > 0.1f) CurrentRate = 60f / interval;
        }
        lastCompression = Time.time;
        rateWindowCount++;
        CompressionRegistered?.Invoke(CompressionCount, CurrentRate);
    }

    public void FinishTest()
    {
        if (!IsActive) return;
        IsActive = false;
        float score = CalculateScore();
        PlayerPrefs.SetInt("CPR_Practices", PlayerPrefs.GetInt("CPR_Practices", 0) + 1);
        PlayerPrefs.SetFloat("CPR_BestScore", Mathf.Max(score, PlayerPrefs.GetFloat("CPR_BestScore", 0f)));
        PlayerPrefs.Save();
    }

    public float CalculateScore()
    {
        // Demo metric: proportion of intervals within 100–120/min, scaled to 100.
        if (CompressionCount < 2 || lastCompression < 0) return 0f;
        return Mathf.Clamp(100f - Mathf.Abs(CurrentRate - 110f) * 2f, 0f, 100f);
    }
}
