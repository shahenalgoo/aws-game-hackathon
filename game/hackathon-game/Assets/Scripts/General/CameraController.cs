using System;
using UnityEngine;


public class CameraController : MonoBehaviour
{
    [SerializeField] private float _panningSpeed = 7f;
    private GameObject _player;
    private PlayerStateMachine _stateMachine;
    private bool _canFollow = true;
    public static Action<bool> _setCanFollow;

    private void Awake()
    {
        _stateMachine = FindObjectOfType<PlayerStateMachine>();
        _player = _stateMachine.gameObject;
    }

    private void OnEnable()
    {
        _setCanFollow += SetCanFollow;
    }
    private void OnDisable()
    {
        _setCanFollow -= SetCanFollow;
    }

    public void SetCanFollow(bool value)
    {
        _canFollow = value;
    }
    void LateUpdate()
    {
        if (_stateMachine == null || !_canFollow) return;


        if (_stateMachine.IsRunning)
        {
            Vector3 followTransform = _player.transform.position;
            transform.position = Vector3.Lerp(transform.position, followTransform, _panningSpeed * Time.deltaTime);
        }
        else
        {
            Vector3 followTransform = _player.transform.position;
            transform.position = Vector3.Lerp(transform.position, followTransform, _panningSpeed / 2f * Time.deltaTime);
        }
    }

}