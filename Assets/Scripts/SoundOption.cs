using UnityEngine;
using UnityEngine.UI;

public class SoundOption : MonoBehaviour
{
    public static float Volume
    {
        get => PlayerPrefs.GetFloat("sound", 1);
        set => PlayerPrefs.SetFloat("sound", value);
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
