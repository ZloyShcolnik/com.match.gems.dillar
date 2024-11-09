using UnityEngine;

public class SetPositionSfx : MonoBehaviour
{
    public static void Instant()
    {
        var go = Instantiate(Resources.Load<GameObject>("setPositionSfx"));
        go.GetComponent<AudioSource>().volume = SoundOption.Volume;
        Destroy(go, 1.0f);
    }
}
