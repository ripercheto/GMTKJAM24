using System.Collections;
using System.Collections.Generic;
using SmallHedge.SoundManager;
using UnityEngine;

public class SwitchPowerDirections : RobotInteractable
{
    protected override bool CanInteract => true;
    
    protected override void HandleInteraction(Player player)
    {
        robot.CyclePowerDirection();
        SoundManager.PlaySound(SoundType.SwitchingLever);
    }
}
