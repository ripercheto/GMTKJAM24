using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatteryReceiverInteractable : BaseRobotInteractable
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
    }
}