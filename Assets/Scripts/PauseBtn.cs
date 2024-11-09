using UnityEngine;
using UnityEngine.UI;

public class PauseBtn : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            PausePopup.Instant();
        });
    }
}
