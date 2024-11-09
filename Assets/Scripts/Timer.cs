using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    private float current;
    private Text timerText;

    private void Awake()
    {
        timerText = GetComponent<Text>();

        var min = Mathf.FloorToInt(current / 60);
        var sec = Mathf.FloorToInt(current % 60);

        timerText.text = string.Format("{0:00}:{1:00}", min, sec);
    }

    private void OnDestroy()
    {
        if (current > RecordText.Value)
        {
            RecordText.Value = current;
        }
    }

    private void Update()
    {
		if(SettingsBtn.IsPressed || ExitPopup.IsOpened || PausePopup.isActive)
        {
            return;
        }
		
        current += Time.deltaTime;

        var min = Mathf.FloorToInt(current / 60);
        var sec = Mathf.Floor(current % 60);

        timerText.text = string.Format("TIME: {0:00}:{1:00}", min, sec);
    }
}
