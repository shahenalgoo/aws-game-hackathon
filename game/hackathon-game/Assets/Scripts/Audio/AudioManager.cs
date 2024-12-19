using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    private Bus musicBus;
    private Bus sfxBus;

    private bool isSfxMuted = false;
    private bool isMusicMuted = false;

    private const string SFX_MUTE_PREF_KEY = "SFXMuted";
    private const string MUSIC_MUTE_PREF_KEY = "MusicMuted";
    public static AudioManager Instance { get; private set; }


    [Header("SFXs")]
    [Header("GUN")]
    [SerializeField] private EventReference _gunshotRef;
    public EventInstance _gunshotSfx;

    [SerializeField] private EventReference _magOutRef;
    public EventInstance _magOutSfx;

    [SerializeField] private EventReference _reloadedRef;
    public EventInstance _reloadedSfx;

    [SerializeField] private EventReference _magFullRef;
    public EventInstance _magFullSfx;

    [Header("PLAYER")]
    [SerializeField] private EventReference _dashRef;
    public EventInstance _dashSfx;
    [SerializeField] private EventReference _dashBoosterRef;
    public EventInstance _dashBoosterSfx;

    [SerializeField] private EventReference _playerDeathRef;
    public EventInstance _playerDeathSfx;

    [Header("TARGETS/TURRETS")]
    [SerializeField] private EventReference _targetShotRef;
    public EventInstance _targetShotSfx;

    [SerializeField] private EventReference _targetMeleeRef;
    public EventInstance _targetMeleeSfx;

    [SerializeField] private EventReference _targetHitRef;
    public EventInstance _targetHitSfx;




    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        // Get the specific buses
        // Note: Replace "bus:/SFX" and "bus:/Music" with your actual FMOD bus paths
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
        musicBus = RuntimeManager.GetBus("bus:/Music");

        // Load saved states
        LoadAudioStates();

        // Instantiate SFXs
        InstantiateSFXs();
        // Instantiate Music
    }

    private void LoadAudioStates()
    {
        isSfxMuted = PlayerPrefs.GetInt(SFX_MUTE_PREF_KEY, 0) == 1;
        isMusicMuted = PlayerPrefs.GetInt(MUSIC_MUTE_PREF_KEY, 0) == 1;

        // Apply saved states
        SetSFXMute(isSfxMuted);
        SetMusicMute(isMusicMuted);
    }

    public void InstantiateSFXs()
    {
        _gunshotSfx = RuntimeManager.CreateInstance(_gunshotRef);
        _magOutSfx = RuntimeManager.CreateInstance(_magOutRef);
        _reloadedSfx = RuntimeManager.CreateInstance(_reloadedRef);
        _magFullSfx = RuntimeManager.CreateInstance(_magFullRef);
        _dashSfx = RuntimeManager.CreateInstance(_dashRef);
        _dashBoosterSfx = RuntimeManager.CreateInstance(_dashBoosterRef);
        _playerDeathSfx = RuntimeManager.CreateInstance(_playerDeathRef);
        _targetShotSfx = RuntimeManager.CreateInstance(_targetShotRef);
        _targetMeleeSfx = RuntimeManager.CreateInstance(_targetMeleeRef);
        _targetHitSfx = RuntimeManager.CreateInstance(_targetHitRef);

    }

    public void PlaySfx(EventInstance sfx)
    {
        sfx.start();
    }

    public void StopSfx(EventInstance sfx)
    {
        sfx.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    // SFX Controls
    public void ToggleSFX()
    {
        SetSFXMute(!isSfxMuted);
    }

    public void SetSFXMute(bool mute)
    {
        isSfxMuted = mute;
        sfxBus.setMute(mute);
        PlayerPrefs.SetInt(SFX_MUTE_PREF_KEY, mute ? 1 : 0);
        PlayerPrefs.Save();
    }

    // Music Controls
    public void ToggleMusic()
    {
        SetMusicMute(!isMusicMuted);
    }

    public void SetMusicMute(bool mute)
    {
        isMusicMuted = mute;
        musicBus.setMute(mute);
        PlayerPrefs.SetInt(MUSIC_MUTE_PREF_KEY, mute ? 1 : 0);
        PlayerPrefs.Save();
    }

    // Volume Controls (if you need them)
    public void SetSFXVolume(float volume)
    {
        sfxBus.setVolume(volume);
    }

    public void SetMusicVolume(float volume)
    {
        musicBus.setVolume(volume);
    }

    // Status Checks
    public bool IsSFXMuted()
    {
        return isSfxMuted;
    }

    public bool IsMusicMuted()
    {
        return isMusicMuted;
    }
}
