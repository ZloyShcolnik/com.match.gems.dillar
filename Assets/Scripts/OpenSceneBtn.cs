using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OpenSceneBtn : MonoBehaviour
{
    [SerializeField] string sceneName;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if(SettingsBtn.IsPressed)
            {
                return;
            }

            SelectSfx.Instant();
            SceneManager.LoadScene(sceneName);
        });
    }
}
