using System.Collections;
using UnityEngine;

public class JumpStart : Interactible
{
    public JumpTarget target;
    public float duration = 0.5f;
    public AnimationCurve jumpHeightCurve;
    public float height = 1f;

    private Coroutine coroutine;
    
    public void TryTriggerJump(Player player)
    {
        if (player.isKinematic)
        {
            return;
        }

        if (player == null)
        {
            return;
        }
        coroutine = StartCoroutine(JumpAnimation(player));
    }
    
    protected override bool CanInteract => coroutine == null;
    protected override void HandleInteraction(Player player)
    {
        TryTriggerJump(player);
    }

    private IEnumerator JumpAnimation(Player player)
    {
        player.SetKinematic(true);
        var playerOffset = player.transform.position - transform.position;
        player.transform.parent = null;
        var t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            var startPos = transform.position + playerOffset;
            var targetPos = target.transform.position;
            var newPos = Vector3.Lerp(startPos, targetPos, t);
            var yAddition = transform.up * (jumpHeightCurve.Evaluate(t) * height);
            player.transform.position = newPos + yAddition;
            yield return null;
        }
        player.SetKinematic(false);
        player.SetWalkableArea(target.area);
        coroutine = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, target.transform.position);
    }
}