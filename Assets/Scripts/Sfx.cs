using UnityEngine;

public class Sfx : MonoBehaviour
{
    public static void Instant()
    {
        var go = Instantiate(Resources.Load<GameObject>("sfx"));
        go.GetComponent<AudioSource>().volume = SoundOption.Volume;
        Destroy(go, 1.0f);
    }
}
