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

    public static Vector2Int GetGridPosition(Transform transform)
    {
        int tileSize = LevelBuilder.Instance.TileSize;

        // get grid pos from world pos
        Vector2Int gridPos = new Vector2Int(Mathf.RoundToInt(transform.position.x / tileSize), Mathf.RoundToInt(transform.position.z / tileSize));
        return gridPos;
    }

    public static void ResetTrackingVariables()
    {
        PlayerPrefs.SetInt(PlayerConstants.PLAYLIST_TRACKER_PREF_KEY, 0);
        PlayerPrefs.SetFloat(PlayerConstants.TIMER_PREF_KEY, 0f);
        PlayerPrefs.Save();
    }

}