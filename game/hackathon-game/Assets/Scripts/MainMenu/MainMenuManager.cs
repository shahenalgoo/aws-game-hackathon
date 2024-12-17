using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void ActivateMainMenu();

    // Start is called before the first frame update
    void Start()
    {
        RequestMainMenuFromReact();

        // Invoke("StartGame", 2f);
    }

    public void RequestMainMenuFromReact()
    {
#if UNITY_WEBGL == true && UNITY_EDITOR == false
    ActivateMainMenu();
    Debug.Log("Main menu requested");
#endif
    }

    public void StartGame()
    {
        SceneManager.LoadScene(SceneIndexes.GameSceneIndex);
    }
}
