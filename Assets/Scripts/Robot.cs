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

    public int health = 4;
    public int chargePerBattery = 4;
    public int laserCost = 4;
    public float laserCooldown = 5;
    public Robot target;

    public RobotAction shieldAction;
    public RobotAction punchAction;

    [ReadOnly]
    public int charges;
    [ReadOnly]
    public PowerDirectionType powerDirectionType;

    public bool CanUseLaser
    {
        get
        {
            if (charges < laserCost)
            {
                return false;
            }

            if (laserUseTime < Time.time)
            {
                return false;
            }

            return true;
        }
    }

    private float laserUseTime;

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

    public virtual void ReceivePunch()
    {
        if (shieldAction.State == RobotActionStateType.Active)
        {
            HandleOnBlockPunch();
            shieldAction.TryCancelActiveState();
        }
        else
        {
            TakeDamage();
        }
    }

    public virtual void ReceiveLaser()
    {
        TakeDamage();
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

    public void TryUseLaser()
    {
        if (!CanUseLaser)
        {
            return;
        }

        charges -= laserCost;
        laserUseTime = Time.time + laserCooldown;
        target.ReceiveLaser();
    }

    protected virtual void HandleOnActivatePunch()
    {
        target.ReceivePunch();
    }

    protected virtual void HandleOnBlockPunch()
    {
        //TODO BREAK SHIELD
    }

    private void TakeDamage()
    {
        health--;
        shieldAction.ForceCancel();
        punchAction.ForceCancel();
        Debug.Log($"{name} took damage. Health: {health}");
    }

    public static PowerDirectionType Cycle(PowerDirectionType type)
    {
        return (PowerDirectionType)(((int)type + 1) % Enum.GetValues(typeof(PowerDirectionType)).Length);
    }
}