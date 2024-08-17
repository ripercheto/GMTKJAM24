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
    public enum ShieldStateType
    {
        Idle,
        Preparing,
        Prepared,
        Shielding,
    }
    public int chargePerBattery = 4;

    [BoxGroup("Shield")]
    public int costPerShield = 1;
    [BoxGroup("Shield")]
    public float shieldPrepareTime = 2;
    [BoxGroup("Shield")]
    public float shieldActiveTime = 2;
    [BoxGroup("Shield")]
    public PowerDirectionType shieldDirection = PowerDirectionType.Left;

    [ShowInInspector, ReadOnly]
    private int charges;
    [ShowInInspector, ReadOnly]
    private PowerDirectionType powerDirectionType;

    public ShieldStateType ShieldState { get; private set; }

    public void ReceiveBattery()
    {
        charges = chargePerBattery;
        CheckShieldState();
    }

    public void CyclePowerDirection()
    {
        powerDirectionType = (PowerDirectionType)(((int)powerDirectionType + 1) % Enum.GetValues(typeof(PowerDirectionType)).Length);
        CheckShieldState();
    }

    public void PrepareShield()
    {
        ShieldState = ShieldStateType.Preparing;
        StartCoroutine(HandlePreparing());

        IEnumerator HandlePreparing()
        {
            yield return new WaitForSeconds(shieldPrepareTime);
            ShieldState = ShieldStateType.Prepared;
            CheckShieldState();
        }
    }

    private void CheckShieldState()
    {
        if (charges < costPerShield)
        {
            return;
        }
        if (ShieldState != ShieldStateType.Prepared)
        {
            return;
        }
        if (powerDirectionType != shieldDirection)
        {
            return;
        }

        ShieldState = ShieldStateType.Shielding;
        charges -= costPerShield;

        StartCoroutine(HandleCooldown());

        IEnumerator HandleCooldown()
        {
            yield return new WaitForSeconds(shieldActiveTime);
            ShieldState = ShieldStateType.Idle;
        }
    }

    public void PreparePunch()
    {
    }
}