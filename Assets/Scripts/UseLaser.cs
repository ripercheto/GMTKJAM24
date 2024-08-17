using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseLaser : RobotInteractable
{
    protected override bool CanInteract => robot.CanUseLaser;
    protected override void HandleInteraction(Player player)
    {
        robot.TryUseLaser();
    }
}
