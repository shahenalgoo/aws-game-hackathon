using UnityEngine;
using System;
using UnityEngine.UI;
using System.Linq;

public class ReloadBar : MonoBehaviour
{
    private Slider _reloadSlider;
    private float _reloadTime;
    private float _currentReloadTime;
    private bool _isReloading;
    private Quaternion _initialRotation; // Store the initial rotation
    public static Action _activateReloadSlider;
    public static Action _cancelReloadSlider;

    private GameObject _player;
    private CanvasGroup _canvasGroup;
    private float alphaOn = 0.8f;

    public void Start()
    {
        _reloadSlider = GetComponentInChildren<Slider>();

        _activateReloadSlider += StartReload;
        _cancelReloadSlider += DeactivateReload;

        // Store the initial rotation we want to maintain
        _initialRotation = transform.rotation;

        // Get animation length from animator
        _player = GameObject.FindGameObjectWithTag("Player");
        Animator animator = _player.GetComponentInParent<Animator>();
        RuntimeAnimatorController animController = animator.runtimeAnimatorController;
        AnimationClip clip = animController.animationClips.First(x => x.name == "Reload");
        _reloadTime = clip.length;

        // Deactive initially
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;
    }

    public void StartReload()
    {
        _currentReloadTime = 0f;
        _isReloading = true;
        _canvasGroup.alpha = alphaOn;
        _reloadSlider.value = 0f;
    }

    public void DeactivateReload()
    {
        _canvasGroup.alpha = 0;
        _isReloading = false;

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

    void LateUpdate()
    {
        transform.rotation = _initialRotation;
    }
}