using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Canvas canvas;
    private Camera mainCamera;
    private Quaternion _initialRotation; // Store the initial rotation

    // Lerp settings
    private float targetHealth;
    [SerializeField] private float lerpSpeed = 10f;
    private bool isLerping = false;

    // Visibility settings
    [SerializeField] private float hideDelay = 5f;
    private float lastDamageTime;
    private bool isVisible = false;

    private void Start()
    {
        // Store the initial rotation we want to maintain
        _initialRotation = transform.rotation;

        mainCamera = Camera.main;

        // Ensure the canvas is set to world space
        canvas.renderMode = RenderMode.WorldSpace;

        // Set initial scale of the canvas to be small enough to fit your world
        canvas.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);

        // Hide health bar initially
        canvas.enabled = false;
    }

    private void Update()
    {
        // Smooth lerp to target health value
        // Only lerp when damage is taken
        if (isLerping)
        {
            slider.value = Mathf.Lerp(slider.value, targetHealth, Time.deltaTime * lerpSpeed);

            // Stop lerping when we're close enough to the target value
            if (Mathf.Abs(slider.value - targetHealth) < 0.01f)
            {
                slider.value = targetHealth;
                isLerping = false;
            }
        }

        // Check if we should hide the health bar
        if (isVisible && Time.time - lastDamageTime > hideDelay)
        {
            HideHealthBar();
        }
    }

    void LateUpdate()
    {
        transform.rotation = _initialRotation;
    }

    public void UpdateHealthBar(float currentHealth)
    {
        // Show health bar if it's not visible
        if (!isVisible)
        {
            ShowHealthBar();
        }

        // Only start lerping if health has decreased
        if (currentHealth < slider.value)
        {
            isLerping = true;
            // Update last damage time
            lastDamageTime = Time.time;
        }
        targetHealth = currentHealth;
    }

    private void ShowHealthBar()
    {
        canvas.enabled = true;
        isVisible = true;
        lastDamageTime = Time.time;
    }

    private void HideHealthBar()
    {
        canvas.enabled = false;
        isVisible = false;
    }

    public void SetMaxHealth(float maxHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
        targetHealth = maxHealth;
    }
}
