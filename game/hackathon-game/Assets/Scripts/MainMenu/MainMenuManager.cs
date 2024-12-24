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
    }

    public void RequestMainMenuFromReact()
    {
#if UNITY_WEBGL == true && UNITY_EDITOR == false
    ActivateMainMenu();
    Debug.Log("Main menu requested");
#endif
    }

    public void StartNormalMode()
    {
        // Set up array
        string[] playlist = new string[2];
        playlist[0] = "{\"grid\":[[0,0,0,1,2,1,0,0,2,1,4,1,0,0,0,0,2,1,0,0],[0,2,3,1,0,4,1,0,0,3,0,5,1,2,1,0,0,4,0,0],[0,1,0,2,0,1,2,1,0,1,0,0,0,0,5,6,0,1,0,0],[0,5,0,1,0,0,0,3,0,4,1,4,1,0,0,2,1,3,1,0],[1,1,0,6,2,1,4,1,1,0,0,0,2,0,2,1,2,0,2,1],[0,0,0,0,0,3,0,0,2,1,5,0,0,0,0,3,5,0,0,0],[0,2,6,4,1,1,0,0,0,2,1,6,1,2,1,1,2,5,0,0],[0,0,3,0,0,5,1,2,0,0,0,0,0,0,3,2,0,2,0,0],[0,0,2,7,0,0,0,1,4,1,2,5,7,0,1,0,0,0,0,0]]}";
        playlist[1] = "{\"grid\":[[0,0,0,2,1,4,1,2,0,0,0,2,1,7,0,0,0,0,0,0],[0,2,3,1,0,0,0,1,4,1,0,0,0,1,2,1,4,2,0,0],[0,1,0,5,1,2,0,3,0,5,1,2,0,0,0,0,0,1,0,0],[2,4,0,1,0,1,0,1,0,0,0,1,4,1,2,3,0,5,2,0],[1,1,6,2,0,3,1,2,1,6,1,0,0,0,1,1,0,1,3,1],[0,0,0,1,0,1,0,0,0,0,2,1,3,0,5,2,0,0,0,0],[0,2,3,2,0,4,1,2,0,0,0,0,1,0,1,4,1,2,0,0],[0,1,0,5,1,0,0,1,4,1,2,0,5,0,0,0,0,1,7,0],[0,2,0,1,2,0,0,0,0,0,1,3,2,1,4,2,0,2,1,0]]}";

        // Set playlist
        SetPlaylist(playlist);

        // Reset trackers
        ResetTrackingVariables();

        // Start game
        StartGame();
    }

    public void StartAILevelMode()
    {
        // Set up array
        string[] playlist = new string[1];
        playlist[0] = "{\"grid\":[[0,0,0,2,1,4,1,2,0,0,0,2,1,7,0,0,0,0,0,0],[0,2,3,1,0,0,0,1,4,1,0,0,0,1,2,1,4,2,0,0],[0,1,0,5,1,2,0,3,0,5,1,2,0,0,0,0,0,1,0,0],[2,4,0,1,0,1,0,1,0,0,0,1,4,1,2,3,0,5,2,0],[1,1,6,2,0,3,1,2,1,6,1,0,0,0,1,1,0,1,3,1],[0,0,0,1,0,1,0,0,0,0,2,1,3,0,5,2,0,0,0,0],[0,2,3,2,0,4,1,2,0,0,0,0,1,0,1,4,1,2,0,0],[0,1,0,5,1,0,0,1,4,1,2,0,5,0,0,0,0,1,7,0],[0,2,0,1,2,0,0,0,0,0,1,3,2,1,4,2,0,2,1,0]]}";

        // Set playlist
        SetPlaylist(playlist);

        // Reset trackers
        ResetTrackingVariables();

        // Start game
        StartGame();
    }

    public void StartBossFight()
    {
        // Reset trackers
        ResetTrackingVariables();
        SceneManager.LoadScene(SceneIndexes.BossFightSceneIndex);
    }

    public void ResetTrackingVariables()
    {
        PlayerPrefs.SetInt(PlayerConstants.PLAYLIST_TRACKER_PREF_KEY, 0);
        PlayerPrefs.SetFloat(PlayerConstants.TIMER_PREF_KEY, 0f);
        PlayerPrefs.Save();
    }

    private void SetPlaylist(string[] playlist)
    {
        string savedPlaylistString = string.Join("###", playlist);
        PlayerPrefs.SetString(PlayerConstants.PLAYLIST_PREF_KEY, savedPlaylistString);
        PlayerPrefs.Save();
    }
    public void StartGame()
    {
        SceneManager.LoadScene(SceneIndexes.GameSceneIndex);
    }
}
