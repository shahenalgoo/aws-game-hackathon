using UnityEngine;

public static class Helpers
{
    private static Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, -45, 0));
    public static Vector3 ToIso(this Vector3 input) => _isoMatrix.MultiplyPoint3x4(input);

    public static Vector3 TargetMousePosition(Camera mainCamera, Vector3 cursorPosition, float lookYOffset)
    {
        Ray cameraRay = mainCamera.ScreenPointToRay(cursorPosition);
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, lookYOffset, 0));
        float rayLength;


        if (groundPlane.Raycast(cameraRay, out rayLength))
        {
            Vector3 collidedPoint = cameraRay.GetPoint(rayLength);
            Debug.DrawLine(cameraRay.origin, collidedPoint, Color.blue);
            return collidedPoint;
        }

        return Vector3.zero;

    }

    public static bool HasNonZeroNeighbor(int[,] grid, int row, int col)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        // Check up
        if (row > 0 && grid[row - 1, col] != 0)
            return true;

        // Check down
        if (row < rows - 1 && grid[row + 1, col] != 0)
            return true;

        // Check left
        if (col > 0 && grid[row, col - 1] != 0)
            return true;

        // Check right
        if (col < cols - 1 && grid[row, col + 1] != 0)
            return true;

        return false;
    }

    public static Vector2Int GetGridPosition(Transform transform, int tileSize)
    {
        // get grid pos from world pos
        Vector2Int gridPos = new Vector2Int(Mathf.RoundToInt(transform.position.x / tileSize), Mathf.RoundToInt(transform.position.z / tileSize));
        return gridPos;
    }

    public static void SetPlaylist(string[] playlist)
    {
        string savedPlaylistString = string.Join("###", playlist);
        PlayerPrefs.SetString(PlayerConstants.PLAYLIST_PREF_KEY, savedPlaylistString);
        PlayerPrefs.Save();
    }

    public static void ResetTrackingVariables()
    {
        PlayerPrefs.SetInt(PlayerConstants.PLAYLIST_TRACKER_PREF_KEY, 0);
        PlayerPrefs.SetFloat(PlayerConstants.TIMER_PREF_KEY, 0f);
        PlayerPrefs.SetInt(PlayerConstants.IS_SURVIVAL_MODE_PREF_KEY, 0);
        PlayerPrefs.SetInt(PlayerConstants.ROUNDS_SURVIVED_PREF_KEY, 0);
        PlayerPrefs.Save();
    }

    public static void SetSurvivalMode(bool value)
    {
        PlayerPrefs.SetInt(PlayerConstants.IS_SURVIVAL_MODE_PREF_KEY, value ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static bool IsSurvivalMode()
    {
        return PlayerPrefs.GetInt(PlayerConstants.IS_SURVIVAL_MODE_PREF_KEY, 0) == 1;
    }

    public static void ModeHasBossFight(bool value)
    {
        PlayerPrefs.SetInt(PlayerConstants.HAS_BOSS_PREF_KEY, value ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static void ResetTrackersOnRestart(bool value)
    {
        PlayerPrefs.SetInt(PlayerConstants.RESET_TRACKERS_ON_RESTART, value ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static void SaveHUDState(bool value)
    {
        PlayerPrefs.SetInt(PlayerConstants.HUD_PREF_KEY, value ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static void RecordTime(float time)
    {
        // Save time
        PlayerPrefs.SetFloat(PlayerConstants.TIMER_PREF_KEY, time);
        PlayerPrefs.Save();
    }

    public static void RecordRoundReached(int round)
    {
        // Save round
        PlayerPrefs.SetInt(PlayerConstants.ROUNDS_SURVIVED_PREF_KEY, round);
        PlayerPrefs.Save();
    }



}