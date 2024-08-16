using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField, Range(0f, 100f)]
    float maxSpeed = 10f;

    [SerializeField, Range(0f, 100f)]
    float maxAcceleration = 10f;

    [SerializeField]
    private WalkablePolygon walkableArea;

    private Vector3 velocity;
    private bool isJumping;

    [Button]
    public void SetWalkableArea(WalkablePolygon newArea)
    {
        transform.SetParent(newArea.transform);
        var localPos = transform.localPosition;
        localPos.y = 0;
        transform.localPosition = localPos;
        walkableArea = newArea;
    }

    public void SetJumping(bool isActive)
    {
        isJumping = isActive;
    }

    void Update()
    {
        if (isJumping)
        {
            return;
        }
        Vector2 playerInput;
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");
        playerInput = Vector2.ClampMagnitude(playerInput, 1f);

        var desiredVelocity = playerInput * maxSpeed;

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