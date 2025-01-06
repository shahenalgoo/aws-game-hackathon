using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.SceneManagement;


[DefaultExecutionOrder(-100)]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    private Bus _musicBus;
    private Bus _sfxBus;

    private bool _isSfxMuted = false;
    public bool SfxMuted { get { return _isSfxMuted; } }
    private bool _isMusicMuted = false;
    public bool MusicMuted { get { return _isMusicMuted; } }

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
    [SerializeField] private EventReference _playerHurtBulletRef;
    public EventInstance _playerHurtBulletSfx;
    [SerializeField] private EventReference _playerHurtSharpRef;
    public EventInstance _playerHurtSharpSfx;

    [SerializeField] private EventReference _playerHurtMeleeRef;
    public EventInstance _playerHurtMeleeSfx;

    [SerializeField] private EventReference _playerHurtLaserRef;
    public EventInstance _playerHurtLaserSfx;

    [SerializeField] private EventReference _playerDeathRef;
    public EventInstance _playerDeathSfx;
    [SerializeField] private EventReference _playerDeath2Ref;
    public EventInstance _playerDeath2Sfx;

    [SerializeField] private EventReference _playerEntranceRef;
    public EventInstance _playerEntranceSfx;

    [Header("TARGETS/TURRETS")]
    [SerializeField] private EventReference _targetShotRef;
    public EventInstance _targetShotSfx;

    [SerializeField] private EventReference _targetMeleeRef;
    public EventInstance _targetMeleeSfx;

    [SerializeField] private EventReference _targetHitRef;
    public EventInstance _targetHitSfx;

    [SerializeField] private EventReference _targetExplodeRef;
    public EventInstance _targetExplodeSfx;

    [SerializeField] private EventReference _starCollectedRef;
    public EventInstance _starCollectedSfx;

    [Header("TRAPS")]
    [SerializeField] private EventReference _spikeTrapRef;
    public EventInstance _spikeTrapSfx;

    [Header("EXTRACTION")]
    [SerializeField] private EventReference _extractionReadyRef;
    public EventInstance _extractionReadySfx;
    [SerializeField] private EventReference _playerExtractRef;
    public EventInstance _playerExtractSfx;

    [Header("BOSS SFX")]
    [SerializeField] private EventReference _sawBladeLaunchRef;
    public EventInstance _sawBladeLaunchSfx;

    [SerializeField] private EventReference _sawBladeRicochetRef;
    public EventInstance _sawBladeRicochetSfx;

    [SerializeField] private EventReference _missileLaunchRef;
    public EventInstance _missileLaunchSfx;
    [SerializeField] private EventReference _missileExplodeRef;
    public EventInstance _missileExplodeSfx;

    [SerializeField] private EventReference _laserbeamFireRef;
    public EventInstance _laserbeamFireSfx;



    [Header("MUSIC")]
    [SerializeField] private EventReference _bgAmbianceRef;
    public EventInstance _bgAmbiance;
    private PLAYBACK_STATE _bgAmbianceState;

    [SerializeField] private EventReference _bossMusicRef;
    public EventInstance _bossMusic;
    private PLAYBACK_STATE _bossMusicState;


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

        // Get the specific buses
        _sfxBus = RuntimeManager.GetBus("bus:/SFX");
        _musicBus = RuntimeManager.GetBus("bus:/Music");

        // Load saved states
        LoadAudioStates();

        // Instantiate SFXs
        InstantiateSFXs();

        // Instantiate Music
        InstantiateMusic();
    }

    void Start()
    {
    }

    private void LoadAudioStates()
    {
        _isSfxMuted = PlayerPrefs.GetInt(PlayerConstants.SFX_MUTE_PREF_KEY, 0) == 1;
        _isMusicMuted = PlayerPrefs.GetInt(PlayerConstants.MUSIC_MUTE_PREF_KEY, 0) == 1;

        // Apply saved states
        SetSFXMute(_isSfxMuted);
        SetMusicMute(_isMusicMuted);
    }

    public void InstantiateSFXs()
    {
        _gunshotSfx = RuntimeManager.CreateInstance(_gunshotRef);
        _magOutSfx = RuntimeManager.CreateInstance(_magOutRef);
        _reloadedSfx = RuntimeManager.CreateInstance(_reloadedRef);
        _magFullSfx = RuntimeManager.CreateInstance(_magFullRef);
        _dashSfx = RuntimeManager.CreateInstance(_dashRef);
        _dashBoosterSfx = RuntimeManager.CreateInstance(_dashBoosterRef);
        _playerHurtBulletSfx = RuntimeManager.CreateInstance(_playerHurtBulletRef);
        _playerHurtSharpSfx = RuntimeManager.CreateInstance(_playerHurtSharpRef);
        _playerHurtMeleeSfx = RuntimeManager.CreateInstance(_playerHurtMeleeRef);
        _playerHurtLaserSfx = RuntimeManager.CreateInstance(_playerHurtLaserRef);
        _playerDeathSfx = RuntimeManager.CreateInstance(_playerDeathRef);
        _playerDeath2Sfx = RuntimeManager.CreateInstance(_playerDeath2Ref);
        _playerEntranceSfx = RuntimeManager.CreateInstance(_playerEntranceRef);

        _targetShotSfx = RuntimeManager.CreateInstance(_targetShotRef);
        _targetMeleeSfx = RuntimeManager.CreateInstance(_targetMeleeRef);
        _targetHitSfx = RuntimeManager.CreateInstance(_targetHitRef);
        _targetExplodeSfx = RuntimeManager.CreateInstance(_targetExplodeRef);
        _starCollectedSfx = RuntimeManager.CreateInstance(_starCollectedRef);

        _spikeTrapSfx = RuntimeManager.CreateInstance(_spikeTrapRef);

        _extractionReadySfx = RuntimeManager.CreateInstance(_extractionReadyRef);
        _playerExtractSfx = RuntimeManager.CreateInstance(_playerExtractRef);
        _sawBladeLaunchSfx = RuntimeManager.CreateInstance(_sawBladeLaunchRef);
        _sawBladeRicochetSfx = RuntimeManager.CreateInstance(_sawBladeRicochetRef);

        _missileLaunchSfx = RuntimeManager.CreateInstance(_missileLaunchRef);
        _missileExplodeSfx = RuntimeManager.CreateInstance(_missileExplodeRef);

        _laserbeamFireSfx = RuntimeManager.CreateInstance(_laserbeamFireRef);
    }

    public void InstantiateMusic()
    {

        _bgAmbiance = RuntimeManager.CreateInstance(_bgAmbianceRef);
        _bgAmbiance.getPlaybackState(out _bgAmbianceState);

        _bossMusic = RuntimeManager.CreateInstance(_bossMusicRef);
        _bossMusic.getPlaybackState(out _bossMusicState);
    }


    public void SetMusic(bool turnOn)
    {
        if (!_isMusicMuted && turnOn)
        {
            // play boss music
            if (SceneManager.GetActiveScene().buildIndex == SceneIndexes.BossFightSceneIndex)
            {
                bool isBossDead = GameObject.FindObjectOfType<BossHealth>(true).IsDead;

                if (_bossMusicState != PLAYBACK_STATE.PLAYING & !isBossDead)
                {
                    _bossMusic.start();
                }

                return;
            }

            if (_bgAmbianceState != PLAYBACK_STATE.PLAYING)
            {
                _bgAmbiance.start();
            }
        }
        else
        {
            _bgAmbiance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            _bossMusic.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }

    }

    public void PlaySfx(EventInstance sfx)
    {
        sfx.start();
    }

    public void StopSfx(EventInstance sfx)
    {
        sfx.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    public void PauseAudio(bool value)
    {
        _musicBus.setPaused(value);
        _sfxBus.setPaused(value);
    }

    // SFX Controls
    public void ToggleSFX()
    {
        SetSFXMute(!_isSfxMuted);
    }

    public void SetSFXMute(bool mute)
    {
        _isSfxMuted = mute;
        _sfxBus.setMute(mute);
        PlayerPrefs.SetInt(PlayerConstants.SFX_MUTE_PREF_KEY, mute ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void StopAllSFXEvents()
    {
        if (_sfxBus.isValid())
        {
            _sfxBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }

    // Music Controls
    public void ToggleMusic()
    {
        SetMusicMute(!_isMusicMuted);
    }

    public void SetMusicMute(bool mute)
    {
        _isMusicMuted = mute;
        _musicBus.setMute(mute);
        PlayerPrefs.SetInt(PlayerConstants.MUSIC_MUTE_PREF_KEY, mute ? 1 : 0);
        PlayerPrefs.Save();
    }
}
