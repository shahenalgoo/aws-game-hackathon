using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }
    public static Action<float> _targetHealthUpdater;
    public static Action<float> _maxHealthUpdater;
    public Slider _healthSlider;
    public static Action<int> _lootUpdater;
    public TextMeshProUGUI _lootText;
    public static Action<int> _ammoUpdater;
    public TextMeshProUGUI _ammoText;
    public TextMeshProUGUI _timerText;
    public TextMeshProUGUI _roundText; // For survival mode

    [Header("Notice Box")]
    [SerializeField] private TextMeshProUGUI _noticeText;
    [SerializeField] private GameObject _noticeBox;
    public static Action<string, float> _noticeUpdater;

    // Health bar lerp settings
    private float _targetHealth;
    [SerializeField] private float _lerpSpeed = 10f;
    private bool _isHealthLerping = false;

    private Animator _animator;
    private CanvasGroup _canvasGroup;

    public void Awake()
    {
        SingletonCheck();

        _lootUpdater += UpdateLootText;
        _ammoUpdater += UpdateAmmoText;
        _targetHealthUpdater += SetTargetHealth;
        _maxHealthUpdater += SetMaxHealthBar;
        _noticeUpdater += NotifyOnHUD;

        // Disable rounds, if active
        if (_roundText != null) _roundText.gameObject.SetActive(false);
    }

    void Start()
    {
        _animator = GetComponent<Animator>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _animator.enabled = false;
        bool hudOn = PlayerPrefs.GetInt(PlayerConstants.HUD_PREF_KEY, 1) == 1;
        ToggleAlpha(hudOn);
    }

    public void TriggerFadeOut()
    {
        if (_canvasGroup.alpha == 1)
        {
            _animator.enabled = true;
            _animator.Play("Fade Out");
        }
    }

    public void ToggleAlpha(bool value)
    {
        _canvasGroup.alpha = value ? 1 : 0;
    }

    public void ToggleHUD(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            bool hudOn = PlayerPrefs.GetInt(PlayerConstants.HUD_PREF_KEY, 1) == 1;
            ToggleAlpha(!hudOn);
            Helpers.SaveHUDState(!hudOn);
        }
    }

    void Update()
    {
        UpdateTimerText(GameManager.Instance != null ? GameManager.Instance.GameTimer : 0);

        UpdateHealthBar();
    }

    void SingletonCheck()
    {
        // If there is an instance, and it's not this one, delete this one
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Set the instance
        Instance = this;
    }
    public void UpdateLootText(int amount)
    {
        _lootText.text = "Stars: " + amount.ToString("N0") + "/" + GameManager.Instance?.TotalTargets;
    }

    public void UpdateAmmoText(int amount)
    {
        _ammoText.text = amount.ToString("N0");
    }

    public void UpdateTimerText(float amount)
    {
        _timerText.text = FormatTime(amount);
    }

    public string FormatTime(float time)
    {
        int hours = Mathf.FloorToInt(time / 3600);
        int minutes = Mathf.FloorToInt((time % 3600) / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        if (hours > 0)
        {
            return string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
        }
        else
        {
            return string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public void UpdateHealthBar()
    {
        // Only lerp when damage is taken
        if (_isHealthLerping)
        {
            _healthSlider.value = Mathf.Lerp(_healthSlider.value, _targetHealth, Time.deltaTime * _lerpSpeed);

            // Stop lerping when we're close enough to the target value
            if (Mathf.Abs(_healthSlider.value - _targetHealth) < 0.01f)
            {
                _healthSlider.value = _targetHealth;
                _isHealthLerping = false;
            }
        }
    }

    public void SetTargetHealth(float currentHealth)
    {
        // Only start lerping if health has decreased
        if (currentHealth < _healthSlider.value)
        {
            _isHealthLerping = true;
        }

        _targetHealth = currentHealth;
    }

    public void SetMaxHealthBar(float maxHealth)
    {
        _healthSlider.maxValue = maxHealth;
        _healthSlider.value = maxHealth;
        _targetHealth = maxHealth;
    }


    public void NotifyOnHUD(string message, float noticeTime)
    {
        StopCoroutine(TurnNoticeOff(noticeTime));
        _noticeBox.GetComponent<Animator>().Play("Roll In");
        _noticeText.text = message;
        StartCoroutine(TurnNoticeOff(noticeTime));
    }
    private IEnumerator TurnNoticeOff(float noticeTime)
    {
        yield return new WaitForSeconds(noticeTime);
        _noticeBox.GetComponent<Animator>().Play("Roll Out");
    }

    public void ShowRounds(int round)
    {
        // For Survival Mode
        if (_roundText == null) return;
        _roundText.text = "Round: " + round;
        _roundText.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        // IMPORTANT: Unsubscribe from static events
        _lootUpdater -= UpdateLootText;
        _ammoUpdater -= UpdateAmmoText;
        _targetHealthUpdater -= SetTargetHealth;
        _maxHealthUpdater -= SetMaxHealthBar;
        _noticeUpdater -= NotifyOnHUD;
    }

}
