using UnityEngine;
using UnityEngine.UI;

public class BalanceText : MonoBehaviour
{
    private Text balanceText;
    private static float current;

    public static int Count
    {
        get => PlayerPrefs.GetInt("balance", 0);
        set => PlayerPrefs.SetInt("balance", value);
    }

    private void Awake()
    {
		Count = 0;
        balanceText = GetComponent<Text>();
        balanceText.text = $"{Count}";
        current = Count;
    }

    private void Update()
    {
        current = Mathf.MoveTowards(current, Count, 250 * Time.deltaTime);
        balanceText.text = $"{Mathf.RoundToInt(current)}";
    }
}
