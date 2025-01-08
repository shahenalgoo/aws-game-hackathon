using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections;


public class ScreenshotController : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void TakeScreenshot();
    public Vector3 _screenshotCameraPos;
    public float _screenshotCameraSize;

    private Vector3 _initialPos;
    private float _initialSize;
    private int _originalCullingMask;
    void Start()
    {
        // set hud canvas to screen space camera
        Canvas hudCanvas = HUDManager.Instance.gameObject.GetComponent<Canvas>();
        hudCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        hudCanvas.worldCamera = Camera.main;

        // stop follow player
        CameraController._setCanFollow?.Invoke(false);

        _initialPos = transform.position;
        _initialSize = Camera.main.orthographicSize;
        _originalCullingMask = Camera.main.cullingMask;

        transform.position = _screenshotCameraPos;
        Camera.main.orthographicSize = _screenshotCameraSize;

        // Remove HUD layer from culling mask
        int hudLayer = LayerMask.NameToLayer("HUD");
        Camera.main.cullingMask &= ~(1 << hudLayer);

        StartCoroutine(OnScreenshotTaken());
    }

    private IEnumerator OnScreenshotTaken()
    {
        yield return new WaitForSeconds(0.5f);

        // send out event to take screenshot
#if UNITY_WEBGL == true && UNITY_EDITOR == false
        TakeScreenshot();
        Debug.Log("Screenshot requested");
#endif

        yield return new WaitForSeconds(0.5f);
        // set hud canvas to overlay
        Canvas hudCanvas = HUDManager.Instance.gameObject.GetComponent<Canvas>();
        hudCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        hudCanvas.worldCamera = null;

        // Restore ui layer
        Camera.main.cullingMask = _originalCullingMask;

        // start following player
        transform.position = _initialPos;
        Camera.main.orthographicSize = _initialSize;
        CameraController._setCanFollow?.Invoke(true);
    }
}
