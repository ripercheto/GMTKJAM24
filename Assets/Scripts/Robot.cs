using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class Robot : MonoBehaviour
{
    public enum PowerDirection
    {
        Left,
        Right,
    }

    public int chargePerBattery = 4;

    [ShowInInspector, ReadOnly]
    private int charges;
    [ShowInInspector, ReadOnly]
    private PowerDirection powerDirection;

    public void GetBattery()
    {
        charges = chargePerBattery;
    }

    public void PrepareShield()
    {

    }

    public void PreparePunch()
    {

    }

    public void CyclePowerDirection()
    {
        powerDirection = (PowerDirection)(((int)powerDirection + 1) % Enum.GetValues(typeof(PowerDirection)).Length);
    }
}