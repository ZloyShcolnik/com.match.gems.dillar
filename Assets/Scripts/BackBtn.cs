using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackBtn : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            SelectSfx.Instant();
            SettingsBtn.IsPressed = false;
            SceneManager.UnloadSceneAsync("settings");
        });
    }
}
