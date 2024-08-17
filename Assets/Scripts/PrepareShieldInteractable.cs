using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrepareShieldInteractable : RobotInteractable
{
    protected override bool CanInteract => robot.shieldAction.State == RobotActionStateType.Idle;

    protected override void HandleInteraction(Player player)
    {
        robot.PrepareShield();
    }
}