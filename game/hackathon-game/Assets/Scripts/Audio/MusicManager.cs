using System.Collections;
using UnityEngine;

public class MusicManager : Singleton<MusicManager>
{
    // Start is called before the first frame update
    void OnEnable()
    {
        // Enable music if audio manager exists
        if (AudioManager.Instance != null) AudioManager.Instance.SetMusic(true);
    }

    void OnDestroy()
    {
        // Disable Music
        if (AudioManager.Instance != null) AudioManager.Instance.SetMusic(false);
    }

    public void FadeBossMusic()
    {
        StartCoroutine(FadeOutBossMusic(2f));
    }

    public IEnumerator FadeOutBossMusic(float fadeTime)
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("AudioManager not available");
            yield break;
        }

        float startVolume = 1f;
        float currentTime = 0;

        while (currentTime <= fadeTime)
        {

            if (AudioManager.Instance == null)
            {
                Debug.LogWarning("Audio system became unavailable during fade");
                yield break;
            }

            currentTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(currentTime / fadeTime);
            float volume = Mathf.Lerp(startVolume, 0f, normalizedTime);

            AudioManager.Instance._bossMusic.setVolume(volume);
            yield return null;
        }

        // Ensure we hit absolute zero volume
        AudioManager.Instance._bossMusic.setVolume(0f);
        AudioManager.Instance._bossMusic.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        AudioManager.Instance._bossMusic.setVolume(startVolume);
    }



}
