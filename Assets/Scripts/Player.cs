using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float maxSpeed = 10f;
    public float maxAcceleration = 10f;
    public Transform batterySocket;
    
    public bool isKinematic;
    public WalkablePolygon walkableArea;

    private Vector3 velocity;
    private GameObject battery;
    public bool HasBattery => battery != null;
    
    public void SetWalkableArea(WalkablePolygon newArea)
    {
        transform.SetParent(newArea.transform);
        var localPos = transform.localPosition;
        localPos.y = 0;
        transform.SetLocalPositionAndRotation(localPos, Quaternion.identity);
        walkableArea = newArea;
    }

    public void SetKinematic(bool isActive)
    {
        isKinematic = isActive;
    }

    public void GiveBattery(GameObject batteryObject)
    {
        battery = batteryObject;
        battery.transform.parent = batterySocket;
        battery.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    public GameObject RemoveBattery()
    {
        battery.transform.parent = null;
        return battery;
    }

    void Update()
    {
        if (isKinematic)
        {
            return;
        }
        var desiredVelocity = PlayerInput.Directional * maxSpeed;

        var maxSpeedChange = maxAcceleration * Time.deltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        velocity.z = Mathf.MoveTowards(velocity.z, desiredVelocity.y, maxSpeedChange);

        var displacement = velocity * Time.deltaTime;
        var localPosition = transform.localPosition;
        var newPosition = localPosition + displacement;

        walkableArea.ClampPosition(ref velocity, ref newPosition);

        transform.localPosition = newPosition;
    }
}