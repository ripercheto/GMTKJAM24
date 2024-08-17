using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Robot : MonoBehaviour
{
    public enum PowerDirectionType
    {
        Left,
        Right,
    }

    public enum RobotActionStateType
    {
        Idle,
        Preparing,
        Prepared,
        Active,
    }

    public int chargePerBattery = 4;

    public Player player;
    public List<WalkablePolygon> leftArmAreas, rightArmAreas;
    public WalkablePolygon leftRespawnArea, rightRespawnArea;

    public RobotAction shieldAction;
    public RobotAction punchAction;

    [ReadOnly]
    public int charges;
    [ReadOnly]
    public PowerDirectionType powerDirectionType;

    private void Awake()
    {
        shieldAction.Initialize(this, null);
        punchAction.Initialize(this, HandleOnPlayerPunch);
    }

    public void ReceiveBattery()
    {
        charges = chargePerBattery;
        shieldAction.TryActivateState();
        punchAction.TryActivateState();
    }

    [Button]
    public void ReceivePunch()
    {
        if (shieldAction.State != RobotActionStateType.Active)
        {
            return;
        }
        HandleOnBlockPunch();
        shieldAction.TryCancelActiveState();
    }

    public void CyclePowerDirection()
    {
        powerDirectionType = (PowerDirectionType)(((int)powerDirectionType + 1) % Enum.GetValues(typeof(PowerDirectionType)).Length);
        shieldAction.TryActivateState();
        shieldAction.TryCancelActiveState();
        punchAction.TryActivateState();
    }

    public void PreparePunch()
    {
        punchAction.StartPrepare();
    }

    public void PrepareShield()
    {
        shieldAction.StartPrepare();
    }

    private void HandleOnPlayerPunch()
    {
        if (player.isKinematic)
        {
            return;
        }
        if (rightArmAreas.Contains(player.walkableArea))
        {
            return;
        }
        player.MakeFallOff(rightRespawnArea);
    }

    private void HandleOnBlockPunch()
    {
        if (player.isKinematic)
        {
            return;
        }
        if (leftArmAreas.Contains(player.walkableArea))
        {
            return;
        }
        player.MakeFallOff(leftRespawnArea);
    }
}