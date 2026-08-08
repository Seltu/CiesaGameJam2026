using UnityEngine;

public class PlayMusic : MonoBehaviour
{
    public AudioClip clip;

    public void PlayMusicHelper()
    {
        AudioManager.instance.PlayOneShotTheme(clip);
    }
}
