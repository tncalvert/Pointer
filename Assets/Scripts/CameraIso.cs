using UnityEngine;
using System.Collections;

/// <summary>
/// Camera class to follow player and allow orbit movement around the player
/// Code from http://wiki.unity3d.com/index.php?title=MouseOrbitImproved
/// </summary>
public class CameraIso : MonoBehaviour {

    /// <summary>
    /// The object we are following (player)
    /// </summary>
    private Transform target;

    /// <summary>
    /// Adjustment of mouse movement speed in the x direction
    /// </summary>
    private float xSpeed = 120.0f;

    /// <summary>
    /// Adjustment of mouse movement speed in the y direction
    /// </summary>
    private float ySpeed = 120.0f;

    /// <summary>
    /// Minimum allowed angle along the y axis (seen as x in Unity inspector)
    /// </summary>
    private float yMinLimit = 0f;

    /// <summary>
    /// Maximum allowed angle along the y axis (seen as y in Unity inspector)
    /// </summary>
    private float yMaxLimit = 80f;

    /// <summary>
    /// The distance from the player that the camera start at
    /// </summary>
    private float distance = 15.0f;

    /// <summary>
    /// The minimum distance possible from the player
    /// </summary>
    private float distanceMin = 5f;

    /// <summary>
    /// The maximum distance possible from the player
    /// </summary>
    private float distanceMax = 25f;
    
    /// <summary>
    /// A variable to hold rotation along the x axis
    /// </summary>
    private float x = 0.0f;

    /// <summary>
    /// A variable to hold rotation along the y axis
    /// </summary>
    private float y = 0.0f;

    // Use this for initialization
    void Start() {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    /// <summary>
    /// Called by the player script to hand in the player transform
    /// </summary>
    /// <param name="player">Player's transform</param>
    public void setup(Transform player) {
        target = player;
    }

    void LateUpdate() {

        if (target) {
            
            if (Input.GetMouseButton(1)) { // Right mouse button

                // Get input along both axii and modify them to move more cleanly around the player
                x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.01f;
                y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
                y = ClampAngle(y, yMinLimit, yMaxLimit);
            }

            // Check for page up and down to modify zoom level (scroll doesn't work for me, and might not for others)
            if (Input.GetKey(KeyCode.PageUp)) {
                distance -= 0.5f;
            }

            if (Input.GetKey(KeyCode.PageDown)) {
                distance += 0.5f;
            }

            // Adjust distance from camera on scrolling
            distance -= Input.GetAxis("Mouse ScrollWheel") * 5;

            // Create a rotation from our current position
            Quaternion rotation = Quaternion.Euler(y, x, 0);

            distance = Mathf.Clamp(distance, distanceMin, distanceMax);

            // Get the distance from the player that we should be at
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            // Create a new position taking in account the position of the
            // target, our distance from it, and the new rotation
            Vector3 position = rotation * negDistance + target.position;

            transform.rotation = rotation;
            transform.position = position;
            
        }

    }

    /// <summary>
    /// Clamps an angle between two values
    /// Also keeps it between -360 and 360 in general
    /// </summary>
    /// <param name="angle">The angle to clamp</param>
    /// <param name="min">The minimum allowed value</param>
    /// <param name="max">The maximum allowed value</param>
    /// <returns>The angle clamped with the supplied range</returns>
    public static float ClampAngle(float angle, float min, float max) {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }


}
