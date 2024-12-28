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
        // stop follow player
        CameraController.setCanFollow?.Invoke(false);

        _initialPos = transform.position;
        _initialSize = Camera.main.orthographicSize;
        _originalCullingMask = Camera.main.cullingMask;

        transform.position = _screenshotCameraPos;
        Camera.main.orthographicSize = _screenshotCameraSize;

        // Remove UI layer from culling mask
        int uiLayer = LayerMask.NameToLayer("UI");
        Camera.main.cullingMask &= ~(1 << uiLayer);

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

        Canvas uiCanvas = UIManager.Instance.gameObject.GetComponent<Canvas>();
        uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        uiCanvas.worldCamera = null;

        // Restore ui layer
        Camera.main.cullingMask = _originalCullingMask;

        // start following player
        transform.position = _initialPos;
        Camera.main.orthographicSize = _initialSize;
        CameraController.setCanFollow?.Invoke(true);
    }
}
