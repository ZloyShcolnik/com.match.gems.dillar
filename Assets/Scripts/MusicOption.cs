using UnityEngine.UI;
using UnityEngine;

public class MusicOption : MonoBehaviour
{
    public static float Volume
    {
        get => PlayerPrefs.GetFloat("music", 1);
        set => PlayerPrefs.SetFloat("music", value);
    }

    private void Awake()
    {
        var slider = transform.GetComponentInChildren<Slider>();
        slider.value = Volume;

        var filled = transform.GetChild(1).GetComponent<Image>();
        filled.fillAmount = Volume;

        slider.onValueChanged.AddListener((value) =>
        {
            Volume = value;
            filled.fillAmount = Volume;
        });
    }
}
