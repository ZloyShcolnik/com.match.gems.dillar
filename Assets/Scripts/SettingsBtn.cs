using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsBtn : MonoBehaviour
{
    public static bool IsPressed;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if(IsPressed)
            {
                return;
            }

            SelectSfx.Instant();
            IsPressed = true;
            SceneManager.LoadScene("settings", LoadSceneMode.Additive);
        });
    }
}
