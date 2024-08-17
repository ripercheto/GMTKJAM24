using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrepareShieldInteractable : BaseRobotInteractable
{
    protected override bool CanInteract => robot.shieldAction.State == PlayerRobot.RobotActionStateType.Idle;

    protected override void HandleInteraction(Player player)
    {
        robot.PrepareShield();
    }
}