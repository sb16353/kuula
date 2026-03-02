using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText; // Assign in Inspector (optional)
    private float elapsedTime = 0f;
    private bool isRunning = true;

    void Update()
    {
        if (isRunning) {
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    void UpdateTimerDisplay() {      
        if(timerText != null) {
            timerText.text = GameManager.Instance.TimerEnabled ? TimeString : string.Empty;
        }         
    }

    string TimeString
    {
        get 
        {
            float time = elapsedTime;
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            int tenths = Mathf.FloorToInt((time * 10f) % 10f);

            return string.Format("{0:00}:{1:00}.{2}", minutes, seconds, tenths);
        }
    }

    // Call this when the game ends
    public void StopTimer(bool addRecord = true)
    {
        isRunning = false;
        Debug.Log("Total Time: " + elapsedTime + " seconds");
        if(addRecord)
            GameManager.Instance.levelCompletionTimes.Add(TimeString);
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }
}
