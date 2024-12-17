using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

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


    [Header("Notice")]
    [SerializeField] private TextMeshProUGUI _noticeText;
    [SerializeField] private float _noticeTimeToDisappear = 2f;
    public static Action<string> _noticeUpdater;


    // Health bar lerp settings
    private float _targetHealth;
    [SerializeField] private float _lerpSpeed = 10f;
    private bool _isHealthLerping = false;

    public void Awake()
    {
        SingletonCheck();

        _lootUpdater += UpdateLootText;
        _ammoUpdater += UpdateAmmoText;
        _targetHealthUpdater += SetTargetHealth;
        _maxHealthUpdater += SetMaxHealthBar;
        _noticeUpdater += NotifyOnHUD;
    }

    void Update()
    {
        UpdateTimerText(GameManager.Instance.GameTimer);

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
        _lootText.text = "Stars Collected: " + amount.ToString("N0") + "/" + GameManager.Instance.TotalTargets;
    }

    public void UpdateAmmoText(int amount)
    {
        _ammoText.text = amount.ToString("N0");
    }

    public void UpdateTimerText(float amount)
    {
        _timerText.text = "Timer : " + amount.ToString();
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


    public void NotifyOnHUD(string message)
    {
        StopCoroutine(TurnNoticeOff());
        _noticeText.gameObject.SetActive(true);
        _noticeText.text = message;
        StartCoroutine(TurnNoticeOff());
    }
    private IEnumerator TurnNoticeOff()
    {
        yield return new WaitForSeconds(_noticeTimeToDisappear);
        _noticeText.gameObject.SetActive(false);
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
