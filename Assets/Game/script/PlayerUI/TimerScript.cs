using UnityEngine;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    public Text timerText;
    public float totalTime = 60f; // Total time for the countdown in seconds
    private float timeRemaining; // Time remaining for the countdown

    private void Start()
    {
        timeRemaining = totalTime;
    }

    private void Update()
    {
        if (timeRemaining > 0f)
        {
            // Update the time remaining
            timeRemaining -= Time.deltaTime;

            // Update the timer text
            UpdateTimerText();
        }
        else
        {
            
            timeRemaining = 0f;
            GameManager.instance.ShowGameOverPanel();
        }
    }

    private void UpdateTimerText()
    {
        // Format the time remaining into minutes and seconds
        int minutes = Mathf.FloorToInt(timeRemaining / 60f);
        int seconds = Mathf.FloorToInt(timeRemaining % 60f);

       
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
