using System;
using UnityEngine;


public class CameraController : MonoBehaviour
{
    public float panningSpeed;
    public GameObject player;
    public PlayerStateMachine stateMachine;

    public static Action<bool> setCanFollow;

    private bool canFollow = true;

    private void Awake()
    {
        stateMachine = player.GetComponent<PlayerStateMachine>();
    }

    private void OnEnable()
    {
        setCanFollow += SetCanFollow;
    }
    private void OnDisable()
    {
        setCanFollow -= SetCanFollow;
    }

    public void SetCanFollow(bool value)
    {
        canFollow = value;
    }
    void LateUpdate()
    {
        if (player == null || !canFollow) return;


        if (stateMachine.IsRunning)
        {
            Vector3 followTransform = player.transform.position;
            this.transform.position = Vector3.Lerp(this.transform.position, followTransform, panningSpeed * Time.deltaTime);
        }
        else
        {
            Vector3 followTransform = player.transform.position;
            this.transform.position = Vector3.Lerp(this.transform.position, followTransform, panningSpeed / 2 * Time.deltaTime);
        }
    }

}