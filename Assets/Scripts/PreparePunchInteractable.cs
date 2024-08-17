using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreparePunchInteractable : BaseRobotInteractable
{
    protected override bool CanInteract => true;
    
    protected override void HandleInteraction(Player player)
    {
        robot.PreparePunch();
    }
}
