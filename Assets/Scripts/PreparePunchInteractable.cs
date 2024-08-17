using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreparePunchInteractable : BaseRobotInteractable
{
    protected override bool CanInteract => robot.punchAction.State == Robot.RobotActionStateType.Idle;

    protected override void HandleInteraction(Player player)
    {
        robot.PreparePunch();
    }
}