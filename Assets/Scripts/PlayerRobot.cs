using System;
using System.Collections.Generic;

public class PlayerRobot : Robot
{
    public Player player;
    public List<WalkablePolygon> leftArmAreas, rightArmAreas;
    public WalkablePolygon leftRespawnArea, rightRespawnArea;

    private void Start()
    {
        GameCanvas.instance.EmptyBattery();
    }

    public override void ReceivePunch()
    {
        base.ReceivePunch();
        if (shieldAction.State != RobotActionStateType.Active)
        {
            var leftSide = player.transform.position.x < 0;
            player.MakeFallOff(leftSide ? leftRespawnArea : rightRespawnArea);
        }
    }

    protected override void HandleOnActivatePunch()
    {
        base.HandleOnActivatePunch();
        if (player.isKinematic)
        {
            return;
        }
        if (!rightArmAreas.Contains(player.walkableArea))
        {
            return;
        }
        player.MakeFallOff(rightRespawnArea);
    }

    protected override void HandleOnBlockPunch()
    {
        base.HandleOnBlockPunch();
        if (player.isKinematic)
        {
            return;
        }
        if (!leftArmAreas.Contains(player.walkableArea))
        {
            return;
        }
        player.MakeFallOff(leftRespawnArea);
    }

    public override void SetCharges(int newCharges)
    {
        base.SetCharges(newCharges);
        if (charges > 0)
        {
            GameCanvas.instance.HideBattery();
        }
        if (charges == 0)
        {
            GameCanvas.instance.EmptyBattery();
        }
    }
}