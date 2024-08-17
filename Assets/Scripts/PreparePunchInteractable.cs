using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreparePunchInteractable : RobotInteractable
{
    protected override bool CanInteract => robot.punchAction.State == RobotActionStateType.Idle;

    protected override void HandleInteraction(Player player)
    {
        robot.PreparePunch();
    }
}