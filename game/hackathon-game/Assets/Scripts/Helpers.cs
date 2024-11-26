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

}