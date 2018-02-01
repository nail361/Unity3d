using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource efxSource;
    public static SoundManager instance = null;

    [SerializeField]
    private AudioClip[] clips;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void PlaySound(string clip_name, float delay = 0.0f)
    {
        foreach( AudioClip clip in clips )
            if (clip.name == clip_name)
            {
                efxSource.clip = clip;
                efxSource.PlayDelayed(delay);
                break;
            }
    }

    public void StopPlaying()
    {
        efxSource.Stop();
    }

}