using System;
using System.Collections;
using System.Collections.Generic;
using SmallHedge.SoundManager;
using UnityEngine;

public class BatteryReceiverInteractable : RobotInteractable
{
    protected override bool CanInteract => true;

    protected override void HandleInteraction(Player player)
    {
        if (!player.HasBattery)
        {
            return;
        }
        var battery = player.RemoveBattery();
        Destroy(battery);
        robot.ReceiveBattery();
        SoundManager.PlaySound(SoundType.DepositingBattery);
    }
}