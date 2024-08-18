using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

public class Player : MonoBehaviour
{
    public AnimatorHandler dieBoolHandler;
    public AnimatorHandler climbFloatHandler;

    public float maxSpeed = 10f;
    public float maxAcceleration = 10f;
    public float respawnDelay = 3;
    public Vector3 ragDollImpulse;
    public Transform batterySocket;
    public Rigidbody body;

    public bool isKinematic;
    public WalkablePolygon walkableArea;

    private Vector3 velocity;
    private GameObject battery;
    public bool HasBattery => battery != null;

    private void Awake()
    {
        if (walkableArea == null)
        {
            isKinematic = true;
            return;
        }
        SetWalkableArea(walkableArea);
    }

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

        GameCanvas.instance.InsertBattery();
    }

    public GameObject RemoveBattery()
    {
        GameCanvas.instance.InsertBattery();
        battery.transform.parent = null;
        return battery;
    }

    public void MakeFallOff(WalkablePolygon respawnArea)
    {
        isKinematic = true;
        body.isKinematic = false;
        transform.parent = null;
        dieBoolHandler.SetBool(true);
        body.AddForce(ragDollImpulse, ForceMode.Impulse);

        StartCoroutine(Respawn());

        IEnumerator Respawn()
        {
            yield return new WaitForSeconds(respawnDelay);
            body.isKinematic = true;
            isKinematic = false;
            SetWalkableArea(respawnArea);
            dieBoolHandler.SetBool(false);
        }
    }

    void Update()
    {
        if (isKinematic)
        {
            return;
        }
        var directional = PlayerInput.Directional;
        climbFloatHandler.SetFloat(directional.magnitude);
        var desiredVelocity = directional * maxSpeed;

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