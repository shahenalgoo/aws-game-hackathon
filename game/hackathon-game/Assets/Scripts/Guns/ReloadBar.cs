using UnityEngine;
using System;
using UnityEngine.UI;
using System.Linq;
using UnityEditor.Animations;

public class ReloadBar : MonoBehaviour
{
    private Slider _reloadSlider;
    private float _reloadTime;
    private float _currentReloadTime;
    private bool _isReloading;
    private Quaternion _initialRotation; // Store the initial rotation
    public static Action _activateReloadSlider;
    public static Action _cancelReloadSlider;

    public void Start()
    {
        _reloadSlider = GetComponent<Slider>();

        _activateReloadSlider += StartReload;
        _cancelReloadSlider += DeactivateReload;

        // Store the initial rotation we want to maintain
        _initialRotation = transform.rotation;

        // Get animation length from animator
        Animator animator = GetComponentInParent<Animator>();
        AnimatorController animController = animator.runtimeAnimatorController as AnimatorController;
        AnimationClip clip = animController.animationClips.First(x => x.name == "Reload");
        _reloadTime = clip.length;

        // Deactive initially
        _reloadSlider.gameObject.SetActive(false);

    }

    public void StartReload()
    {
        _currentReloadTime = 0f;
        _isReloading = true;
        _reloadSlider.gameObject.SetActive(true);
        _reloadSlider.value = 0f;
    }

    public void DeactivateReload()
    {
        _isReloading = false;
        _reloadSlider.gameObject.SetActive(false);

    }

    void Update()
    {
        if (_isReloading)
        {
            _currentReloadTime += Time.deltaTime;
            _reloadSlider.value = Mathf.Lerp(0f, 1f, _currentReloadTime / _reloadTime);

            if (_currentReloadTime >= _reloadTime)
            {
                DeactivateReload();
            }
        }
    }

    // Alternative approach using LateUpdate if Update isn't smooth enough
    void LateUpdate()
    {
        if (_isReloading)
        {
            // This ensures the rotation reset happens after all other updates
            transform.rotation = _initialRotation;
        }
    }
}