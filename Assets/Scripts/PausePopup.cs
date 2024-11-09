using UnityEngine;
using UnityEngine.UI;

public class PausePopup : MonoBehaviour
{
    public static bool isActive;
    [SerializeField] Button resume;

    private void Awake()
    {
        isActive = true;
        Time.timeScale = 0;

        resume.onClick.AddListener(() =>
        {
            Destroy(gameObject);
        });
    }

    private void OnDestroy()
    {
        isActive = false;
        Time.timeScale = 1;
    }

    public static void Instant()
    {
        if(isActive)
        {
            return;
        }

        Instantiate(Resources.Load<GameObject>("pause"));
    }
}
