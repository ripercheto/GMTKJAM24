using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class Robot : MonoBehaviour
{
    public enum PowerDirectionType
    {
        Left,
        Right
    }

    public enum RobotActionStateType
    {
        Idle,
        Preparing,
        Prepared,
        Active
    }

    public int health = 4;
    public int chargePerBattery = 4;

    public Robot target;

    public RobotAction shieldAction;
    public RobotAction punchAction;

    [ReadOnly]
    public int charges;
    [ReadOnly]
    public PowerDirectionType powerDirectionType;

    private void Awake()
    {
        shieldAction.Initialize(this, null);
        punchAction.Initialize(this, HandleOnActivatePunch);
    }

    public void ReceiveBattery()
    {
        charges = chargePerBattery;
        shieldAction.TryActivateState();
        punchAction.TryActivateState();
    }

    [Button]
    public virtual void ReceivePunch()
    {
        if (shieldAction.State == RobotActionStateType.Active)
        {
            HandleOnBlockPunch();
            shieldAction.TryCancelActiveState();
        }
        else
        {
            health--;
            Debug.Log($"{name} took damage. Health: {health}");
        }
    }

    public void CyclePowerDirection()
    {
        powerDirectionType = Cycle(powerDirectionType);
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

    public void UseLaser()
    {
    }

    protected virtual void HandleOnActivatePunch()
    {
        target.ReceivePunch();
    }

    protected virtual void HandleOnBlockPunch()
    {

    }

    public static PowerDirectionType Cycle(PowerDirectionType type)
    {
        return (PowerDirectionType)(((int)type + 1) % Enum.GetValues(typeof(PowerDirectionType)).Length);
    }
}