using UnityEngine;
using UnityEngine.UI;

public class Result : MonoBehaviour
{
    public static bool gameOver;
    [SerializeField] Text scoreText;

    private void OnEnable()
    {
        gameOver = true;
    }

    private void OnDestroy()
    {
        gameOver = false;
    }

    public static void Instant(int score)
    {
		if(gameOver)
		{
			return;
		}
		
        var go = Instantiate(Resources.Load<Result>("result"));
        go.scoreText.text = $"{score}";
    }
}
