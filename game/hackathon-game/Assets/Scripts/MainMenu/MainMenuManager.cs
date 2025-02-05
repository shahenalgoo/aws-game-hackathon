using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class MainMenuManager : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void ActivateMainMenu();

    [SerializeField] private GameObject _menu;
    void Start()
    {
        Time.timeScale = 1f;

        RequestMainMenuFromReact();
    }

    public void RequestMainMenuFromReact()
    {
#if UNITY_WEBGL == true && UNITY_EDITOR == false
    _menu.SetActive(false);
    ActivateMainMenu();
    Debug.Log("Main menu requested");
#endif
    }

    public void StartNormalMode()
    {
        // Set up playlist for normal mode / campaign
        string[] playlist = new string[3];
        playlist[0] = "{\"grid\":[[0,0,2,0,2,0,0,0,2,7,0,0,0,2,1,0,0,2,0,0],[0,1,1,5,1,2,0,2,1,3,0,2,1,6,4,2,0,1,0,0],[2,4,0,0,7,1,3,1,0,1,4,1,0,5,0,4,0,7,1,0],[0,6,0,0,2,0,0,5,0,2,1,0,0,1,3,1,2,1,8,0],[1,3,0,0,6,1,2,1,0,0,5,0,0,7,0,0,6,0,1,2],[0,1,2,4,1,4,0,2,0,0,1,2,0,2,1,0,4,0,0,0],[0,0,0,0,0,1,0,7,1,6,0,1,5,0,5,3,1,0,0,0],[0,0,0,2,1,5,2,1,3,2,0,0,1,0,0,1,2,0,0,0],[0,0,0,0,0,0,0,0,0,2,6,2,3,2,0,2,0,0,0,0]]}";
        playlist[1] = "{\"grid\":[[0,0,0,0,2,4,2,0,0,1,0,0,0,1,7,2,0,0,0,0],[0,2,4,1,7,0,1,2,1,3,0,0,2,1,5,0,0,2,0,0],[0,5,0,0,6,0,0,7,0,1,2,1,6,0,1,3,1,4,0,0],[2,1,0,0,1,0,0,2,0,5,0,0,3,0,0,0,0,1,2,0],[1,1,3,2,7,2,0,1,1,6,0,0,2,1,2,0,0,5,0,0],[0,0,0,0,6,0,0,0,0,1,2,1,7,0,4,1,2,1,0,0],[0,0,2,1,4,1,2,0,0,5,0,0,2,0,0,0,0,3,0,0],[0,0,5,0,2,0,1,4,2,1,0,0,1,2,1,3,2,6,2,0],[0,0,1,2,0,0,0,0,0,8,0,0,0,0,0,0,0,1,0,0]]}";
        playlist[2] = "{\"grid\":[[0,0,0,2,1,8,1,2,0,0,2,4,0,0,0,0,3,2,0,0],[0,2,7,5,0,7,0,6,0,0,1,3,0,2,1,4,1,0,0,0],[1,1,2,1,0,2,0,1,2,0,5,1,2,7,5,0,6,0,0,0],[4,2,0,3,0,1,0,0,1,0,0,0,0,2,1,0,2,0,0,0],[1,6,0,1,2,5,2,0,4,0,0,0,0,7,3,0,1,2,0,0],[0,1,0,0,0,1,6,0,1,2,0,0,2,1,4,0,5,0,0,0],[0,3,2,0,0,0,1,0,0,7,0,2,6,0,1,0,2,0,0,0],[0,1,5,4,2,0,3,1,0,2,0,1,2,0,5,0,1,2,0,0],[0,0,0,0,1,2,0,2,0,1,2,0,0,0,2,0,0,1,2,0]]}";

        // string[] playlist = new string[2];
        // playlist[0] = "{\"grid\":[[0,0,0,1,2,1,0,0,2,1,4,1,0,0,0,0,2,1,0,0],[0,2,3,1,0,4,1,0,0,3,0,5,1,2,1,0,0,4,0,0],[0,8,0,2,0,1,2,1,0,1,0,0,0,0,5,6,0,1,0,0],[0,5,0,1,0,0,0,3,0,4,1,4,1,0,0,2,1,3,1,0],[1,1,0,6,2,1,4,1,1,0,0,0,2,0,2,1,2,0,2,1],[0,0,0,0,0,3,0,0,2,1,5,0,0,0,0,3,5,0,0,0],[0,2,6,4,1,1,0,0,0,2,1,6,1,2,1,1,2,5,0,0],[0,0,3,0,0,5,1,2,0,0,0,0,0,0,3,2,0,2,0,0],[0,0,2,7,0,0,0,1,4,1,2,5,7,0,1,0,0,0,0,0]]}";
        // playlist[1] = "{\"grid\":[[0,0,0,2,1,4,1,2,0,0,0,2,1,7,0,0,0,0,0,0],[0,2,3,1,0,0,0,1,4,1,0,0,0,1,2,1,4,2,0,0],[0,1,0,5,1,2,0,3,0,5,1,2,0,0,0,0,0,1,0,0],[2,4,0,1,0,1,0,8,0,0,0,1,4,1,2,3,0,5,2,0],[1,1,6,2,0,3,1,2,1,6,1,0,0,0,1,1,0,1,3,1],[0,0,0,1,0,1,0,0,0,0,2,1,3,0,5,2,0,0,0,0],[0,2,3,2,0,4,1,2,0,0,0,0,1,0,1,4,1,2,0,0],[0,1,0,5,1,0,0,1,4,1,2,0,5,0,0,0,0,1,7,0],[0,2,0,1,2,0,0,0,0,0,1,3,2,1,4,2,0,2,1,0]]}";

        // Set playlist
        Helpers.SetPlaylist(playlist);

        // Reset trackers
        Helpers.ResetTrackingVariables();
        Helpers.ModeHasBossFight(true);
        Helpers.ResetTrackersOnRestart(false);
        Helpers.SetSurvivalMode(false);

        // Start game
        StartGame();
    }

    public void StartCampaignModeTest()
    {
        string[] playlist = new string[] {
            "{\"grid\":[[0,0,2,0,2,0,0,0,2,7,0,0,0,2,1,0,0,2,0,0],[0,1,1,5,1,2,0,2,1,3,0,2,1,6,4,2,0,1,0,0],[2,4,0,0,7,1,3,1,0,1,4,1,0,5,0,4,0,7,1,0],[0,6,0,0,2,0,0,5,0,2,1,0,0,1,3,1,2,1,8,0],[1,3,0,0,6,1,2,1,0,0,5,0,0,7,0,0,6,0,1,2],[0,1,2,4,1,4,0,2,0,0,1,2,0,2,1,0,4,0,0,0],[0,0,0,0,0,1,0,7,1,6,0,1,5,0,5,3,1,0,0,0],[0,0,0,2,1,5,2,1,3,2,0,0,1,0,0,1,2,0,0,0],[0,0,0,0,0,0,0,0,0,2,6,2,3,2,0,2,0,0,0,0]]}",
            "{\"grid\":[[0,0,0,0,2,4,2,0,0,1,0,0,0,1,7,2,0,0,0,0],[0,2,4,1,7,0,1,2,1,3,0,0,2,1,5,0,0,2,0,0],[0,5,0,0,6,0,0,7,0,1,2,1,6,0,1,3,1,4,0,0],[2,1,0,0,1,0,0,2,0,5,0,0,3,0,0,0,0,1,2,0],[1,1,3,2,7,2,0,1,1,6,0,0,2,1,2,0,0,5,0,0],[0,0,0,0,6,0,0,0,0,1,2,1,7,0,4,1,2,1,0,0],[0,0,2,1,4,1,2,0,0,5,0,0,2,0,0,0,0,3,0,0],[0,0,5,0,2,0,1,4,2,1,0,0,1,2,1,3,2,6,2,0],[0,0,1,2,0,0,0,0,0,8,0,0,0,0,0,0,0,1,0,0]]}",
            "{\"grid\":[[0,0,0,2,1,8,1,2,0,0,2,4,0,0,0,0,3,2,0,0],[0,2,7,5,0,7,0,6,0,0,1,3,0,2,1,4,1,0,0,0],[1,1,2,1,0,2,0,1,2,0,5,1,2,7,5,0,6,0,0,0],[4,2,0,3,0,1,0,0,1,0,0,0,0,2,1,0,2,0,0,0],[1,6,0,1,2,5,2,0,4,0,0,0,0,7,3,0,1,2,0,0],[0,1,0,0,0,1,6,0,1,2,0,0,2,1,4,0,5,0,0,0],[0,3,2,0,0,0,1,0,0,7,0,2,6,0,1,0,2,0,0,0],[0,1,5,4,2,0,3,1,0,2,0,1,2,0,5,0,1,2,0,0],[0,0,0,0,1,2,0,2,0,1,2,0,0,0,2,0,0,1,2,0]]}"
        };

        PlaylistWrapper _wrapper = new PlaylistWrapper();
        _wrapper.playlist = playlist;

        string jsonPlaylist = JsonConvert.SerializeObject(_wrapper);
        // Debug.Log(jsonPlaylist);
        StartCampaignMode(jsonPlaylist);
    }
    public void StartCampaignMode(string jsonPlaylist)
    {
        try
        {
            // Parse the JSON string
            // PlaylistWrapper playlistData = JsonUtility.FromJson<PlaylistWrapper>(jsonPlaylist);
            PlaylistWrapper playlistData = JsonConvert.DeserializeObject<PlaylistWrapper>(jsonPlaylist);

            // Set up playlist with the parsed data
            string[] playlist = playlistData.playlist;
            // Debug.Log(playlist[0]);
            // Debug.Log(playlist[1]);
            // Debug.Log(playlist[2]);

            if (playlist.Length != 3)
            {
                Debug.Log("Playlist length is not 3, starting with fallback levels");
                StartNormalMode();
                return;
            }

            // Set playlist
            Helpers.SetPlaylist(playlist);

            // Reset trackers
            Helpers.ResetTrackingVariables();
            Helpers.ModeHasBossFight(true);
            Helpers.ResetTrackersOnRestart(false);
            Helpers.SetSurvivalMode(false);

            // Start game
            StartGame();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error parsing playlist JSON: {e.Message}");
        }
    }

    public void StartSurvivalMode(string gridData)
    {
        // Set up array
        string[] playlist = new string[1];
        playlist[0] = gridData;

        // Set playlist
        Helpers.SetPlaylist(playlist);

        // Reset trackers
        Helpers.ResetTrackingVariables();
        Helpers.ModeHasBossFight(false);
        Helpers.ResetTrackersOnRestart(false);
        Helpers.SetSurvivalMode(true);

        // Start game
        StartGame();
    }

    public void StartSurvivalModeTest()
    {
        StartSurvivalMode("{\"grid\":[[0,0,0,2,1,2,1,2,0,0,2,4,0,0,0,0,3,2,0,0],[0,2,7,5,0,7,0,6,0,0,1,3,0,2,1,4,1,0,0,0],[1,1,2,1,0,2,0,1,2,0,5,1,2,7,5,0,6,0,0,0],[4,8,0,3,0,1,0,0,1,0,0,0,0,2,1,0,2,0,0,0],[1,6,0,1,2,5,2,0,4,0,0,0,0,7,3,0,1,2,0,0],[0,1,0,0,0,1,6,0,1,2,0,0,2,1,4,0,5,0,0,0],[0,3,2,0,0,0,1,0,0,7,0,2,6,0,1,0,2,0,0,0],[0,1,5,4,2,0,3,1,0,2,0,1,2,0,5,0,1,2,0,0],[0,0,0,0,1,2,0,2,0,1,2,0,0,0,2,0,0,1,2,0]]}");

    }

    public void StartAILevelTest()
    {
        // StartAILevelMode("{\"grid\":[[0,0,0,2,1,4,1,2,0,0,0,2,1,7,0,0,0,0,0,0],[0,2,3,1,0,0,0,1,4,1,0,0,0,1,2,1,4,2,0,0],[0,1,0,5,1,2,0,3,0,5,1,2,0,0,0,0,0,1,0,0],[2,4,0,1,0,1,0,1,0,0,0,1,4,1,2,3,0,5,2,0],[1,1,6,2,0,3,8,2,1,6,1,0,0,0,1,1,0,1,3,1],[0,0,0,1,0,1,0,0,0,0,2,1,3,0,5,2,0,0,0,0],[0,2,3,2,0,4,1,2,0,0,0,0,1,0,1,4,1,2,0,0],[0,1,0,5,1,0,0,1,4,1,2,0,5,0,0,0,0,1,7,0],[0,2,0,1,2,0,0,0,0,0,1,3,2,1,4,2,0,2,1,0]]}");

        // Circular level
        // StartAILevelMode("{\"grid\":[[1,2,1,2,4,6,2,3,1,2,1,3,1,4,2,5,1,2,1,1],[5,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2],[2,0,2,7,1,1,2,1,2,1,6,1,2,1,4,1,5,1,0,7],[3,0,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,4,0,3],[1,0,2,0,2,8,0,0,0,0,0,0,0,0,0,0,0,6,0,6],[0,0,1,0,5,0,0,0,0,0,0,0,0,0,0,0,0,2,0,5],[0,0,2,0,1,1,2,2,3,1,6,1,7,1,4,1,5,1,0,2],[0,0,7,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1],[0,0,1,2,4,1,2,7,1,2,1,3,6,2,1,2,1,7,2,1]]}");

        // Mix of shapes
        // StartAILevelMode("{\"grid\":[[0,0,0,2,7,2,0,0,0,0,0,2,1,2,0,0,2,4,1,6],[0,2,0,0,1,0,0,0,0,1,0,1,0,1,0,1,1,0,0,5],[0,4,1,0,3,1,0,1,3,2,0,1,3,4,0,0,3,0,0,1],[0,0,5,2,6,0,2,0,8,0,0,6,0,1,0,0,6,1,0,3],[1,7,1,0,1,2,0,1,5,1,4,1,5,1,1,0,4,7,0,7],[0,2,0,0,0,0,0,6,1,2,0,0,0,0,3,0,1,2,0,1],[0,1,2,1,7,2,0,5,0,2,1,0,0,0,1,0,0,0,0,4],[0,0,0,0,2,0,0,1,0,0,2,7,0,0,2,1,6,5,1,2],[0,0,0,0,0,0,0,1,2,0,0,2,2,0,0,2,0,0,2,0]]}");

        // Main prompt but with seed number
        // StartAILevelMode("{\"grid\":[[0,0,0,0,2,1,2,0,0,0,0,0,2,7,2,0,0,0,0,0],[0,2,4,1,7,0,1,2,0,0,2,1,3,1,6,2,0,2,0,0],[0,1,0,0,3,0,0,1,0,2,1,0,0,0,1,5,1,4,2,0],[0,5,0,0,1,2,0,6,1,8,3,2,0,0,0,1,0,0,1,0],[1,1,2,0,0,1,0,1,7,1,6,1,2,0,0,4,0,0,3,0],[0,0,6,1,0,3,2,0,2,0,0,0,1,2,0,1,2,0,1,0],[0,0,1,7,1,1,5,0,0,0,0,0,0,1,4,0,5,1,2,0],[0,0,2,0,0,2,1,2,0,0,2,1,0,3,1,0,1,6,0,0],[0,0,0,0,0,0,0,0,0,0,0,2,2,0,2,2,0,2,0,0]]}");

        // seed 35
        // StartAILevelMode("{\"grid\":[[0,0,0,0,2,1,2,0,0,0,2,4,1,2,0,0,0,0,0,0],[0,2,7,3,1,5,1,2,0,0,1,0,6,0,0,2,1,2,0,0],[0,1,2,0,7,0,0,1,0,2,3,0,1,0,0,4,0,1,8,0],[2,4,0,0,1,0,0,5,0,1,0,0,7,2,1,1,0,3,1,2],[1,6,1,2,3,2,0,1,2,6,1,2,1,0,5,0,0,0,0,0],[0,1,0,0,0,1,0,0,0,1,0,0,4,0,1,2,0,0,0,0],[0,5,2,1,2,7,2,0,0,3,0,0,1,0,6,1,2,0,0,0],[0,1,0,6,0,1,0,0,0,2,1,2,5,2,1,0,4,2,0,0],[0,2,0,1,2,0,0,0,0,0,2,0,0,0,2,0,1,7,2,0]]}");

        // seed 943, asked to put 8 at top middle
        StartAILevelMode("{\"grid\":[[0,0,0,2,1,8,1,2,0,0,2,4,0,0,0,0,3,2,0,0],[0,2,7,5,0,7,0,6,0,0,1,3,0,2,1,4,1,0,0,0],[1,1,2,1,0,2,0,1,2,0,5,1,2,7,5,0,6,0,0,0],[4,2,0,3,0,1,0,0,1,0,0,0,0,2,1,0,2,0,0,0],[1,6,0,1,2,5,2,0,4,0,0,0,0,7,3,0,1,2,0,0],[0,1,0,0,0,1,6,0,1,2,0,0,2,1,4,0,5,0,0,0],[0,3,2,0,0,0,1,0,0,7,0,2,6,0,1,0,2,0,0,0],[0,1,5,4,2,0,3,1,0,2,0,1,2,0,5,0,1,2,0,0],[0,0,0,0,1,2,0,2,0,1,2,0,0,0,2,0,0,1,2,0]]}");
    }

    public void StartAILevelMode(string gridData)
    {
        // Set up array
        string[] playlist = new string[1];
        playlist[0] = gridData;

        // Set playlist
        Helpers.SetPlaylist(playlist);

        // Reset trackers
        Helpers.ResetTrackingVariables();
        Helpers.ModeHasBossFight(false);
        Helpers.ResetTrackersOnRestart(true);
        Helpers.SetSurvivalMode(false);

        // Start game
        StartGame();
    }

    public void StartTutorial()
    {
        // TUTORIAL
        // [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        // [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        // [ 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        // [ 1, 1, 0, 1, 1, 0, 1, 1, 1, 0, 0, 0, 0, 1, 7, 1, 0, 0, 0, 0 ],
        // [ 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 3, 1, 4, 1, 0, 5, 0, 0, 1, 1 ],
        // [ 1, 1, 0, 1, 1, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 0 ],
        // [ 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 1, 6, 1, 1, 0 ],
        // [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],
        // [ 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 ],

        // Set up array
        string[] playlist = new string[1];
        playlist[0] = "{\"grid\":[[0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0],[0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0],[0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0],[1,1,0,1,1,0,1,1,1,0,0,0,0,1,7,1,0,0,0,0],[1,1,1,1,1,1,1,1,1,1,3,1,4,1,0,5,0,0,1,8],[1,1,0,1,1,0,1,1,1,0,0,0,0,0,0,1,0,0,1,0],[0,0,0,0,0,0,0,0,2,0,0,0,0,0,0,1,6,1,1,0],[0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0],[0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0]]}";

        // Set playlist
        Helpers.SetPlaylist(playlist);
        // Reset trackers
        Helpers.ResetTrackingVariables();
        Helpers.ModeHasBossFight(false);
        Helpers.ResetTrackersOnRestart(true);
        Helpers.SetSurvivalMode(false);
        SceneManager.LoadScene(SceneIndexes.TutorialSceneIndex);

    }

    public void StartBossFight()
    {
        // Reset trackers
        Helpers.ResetTrackingVariables();
        Helpers.ResetTrackersOnRestart(true);
        Helpers.SetSurvivalMode(false);
        SceneManager.LoadScene(SceneIndexes.BossFightSceneIndex);
    }

    public void StartFootageScene()
    {
        SceneManager.LoadScene(4);

    }


    public void StartGame()
    {
        SceneManager.LoadScene(SceneIndexes.GameSceneIndex);
    }
}

[System.Serializable]
public class PlaylistWrapper
{
    public string[] playlist;
}