using UnityEngine.UI;
using UnityEngine;

public class RecordText : MonoBehaviour
{
    public static float Value
    {
        get => PlayerPrefs.GetFloat("record", 0);
        set => PlayerPrefs.SetFloat("record", value);
    }

    private Text recordText;

    private void Awake()
    {
        recordText = GetComponent<Text>();

        var min = Mathf.FloorToInt(Value / 60);
        var sec = Mathf.Floor(Value % 60);

        recordText.text = string.Format("TIME: {0:00}:{1:00}", min, sec);
    }
}
