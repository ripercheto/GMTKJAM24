using System.Collections.Generic;

public class PlayerRobot : Robot
{
    public Player player;
    public List<WalkablePolygon> leftArmAreas, rightArmAreas;
    public WalkablePolygon leftRespawnArea, rightRespawnArea;

    public override void ReceivePunch()
    {
        base.ReceivePunch();
        var leftSide = player.transform.position.x < 0;
        player.MakeFallOff(leftSide ? leftRespawnArea : rightRespawnArea);
    }

    protected override void HandleOnActivatePunch()
    {
        base.HandleOnActivatePunch();
        if (player.isKinematic)
        {
            return;
        }
        if (rightArmAreas.Contains(player.walkableArea))
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
        if (leftArmAreas.Contains(player.walkableArea))
        {
            return;
        }
        player.MakeFallOff(leftRespawnArea);
    }
}