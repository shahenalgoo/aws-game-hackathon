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
}
