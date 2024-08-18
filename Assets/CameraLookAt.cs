using UnityEngine;
// This complete script can be attached to a camera to make it
// continuously point at another object.

public class SmoothLookAt : MonoBehaviour
{
    public Transform target;        // The object to look at
    public Vector3 offset = Vector3.zero; // Offset from the target's position
    public float rotationSpeed = 5f; // Speed at which the camera rotates
    public Vector2 rotationLimitsX = new Vector2(-45f, 45f); // Limits on the X-axis rotation (pitch)
    public Vector2 rotationLimitsY = new Vector2(-45f, 45f); // Limits on the Y-axis rotation (yaw)

    private Quaternion initialRotation; // Store the initial rotation of the camera

    void Start()
    {
        // Store the initial rotation of the camera
        initialRotation = transform.rotation;
    }

    void Update()
    {
        // Determine the direction to look at the target, including the offset
        Vector3 directionToTarget = (target.position + offset) - transform.position;
        
        // Calculate the rotation required to look at the target with the offset
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        // Convert the target rotation to Euler angles to clamp it
        Vector3 targetEulerAngles = targetRotation.eulerAngles;

        // Convert initial rotation to Euler angles
        Vector3 initialEulerAngles = initialRotation.eulerAngles;

        // Clamp the rotation within the specified limits
        float clampedX = ClampAngle(targetEulerAngles.x, initialEulerAngles.x + rotationLimitsX.x, initialEulerAngles.x + rotationLimitsX.y);
        float clampedY = ClampAngle(targetEulerAngles.y, initialEulerAngles.y + rotationLimitsY.x, initialEulerAngles.y + rotationLimitsY.y);

        // Apply the clamped rotation
        Quaternion clampedRotation = Quaternion.Euler(clampedX, clampedY, targetEulerAngles.z);

        // Smoothly interpolate between the current rotation and the clamped rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, clampedRotation, rotationSpeed * Time.deltaTime);
    }

    // Utility method to clamp an angle between a minimum and maximum value
    float ClampAngle(float angle, float min, float max)
    {
        if (angle < 90 || angle > 270) // 0-90 degrees or 270-360 degrees
        {
            if (angle > 180) angle -= 360; // Convert angles greater than 180 to negative equivalent
            if (max > 180) max -= 360;
            if (min > 180) min -= 360;
        }

        angle = Mathf.Clamp(angle, min, max);

        if (angle < 0) angle += 360; // Convert back to 0-360 range
        return angle;
    }
}