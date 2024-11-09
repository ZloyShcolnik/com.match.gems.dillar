using UnityEngine;

public class MusicDontDestroy : MonoBehaviour
{
    private AudioSource source;
    private static MusicDontDestroy Instance { get; set; }

    private void Awake()
    {
        if(!Instance)
        {
            Instance = this;
            source = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        source.volume = MusicOption.Volume;
    }
}
