using UnityEngine;


public class CameraController : MonoBehaviour
{
    public float panningSpeed;
    public GameObject player;
    private PlayerStateMachine stateMachine;

    private void Awake()
    {
        stateMachine = player.GetComponent<PlayerStateMachine>();
    }
    void Update()
    {
        if (player == null) return;


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