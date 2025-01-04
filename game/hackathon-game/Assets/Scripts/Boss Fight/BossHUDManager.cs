using UnityEngine;
using UnityEngine.UI;

public class BossHUDManager : MonoBehaviour
{
    // Health bar lerp settings
    private float _targetHealth;
    [SerializeField] private float _lerpSpeed = 10f;
    [SerializeField] private bool _isHealthLerping = false;
    public Slider _healthSlider;


    public void SetMaxHealthBar(float maxHealth)
    {
        _healthSlider.maxValue = maxHealth;
        _healthSlider.value = maxHealth;
        _targetHealth = maxHealth;
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

    void Update()
    {
        UpdateHealthBar();
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
}
