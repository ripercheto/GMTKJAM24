using System;
using System.Collections;
using UnityEngine;

public class JumpStartInteractable : Interactable
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

        var startTransform = transform;
        var targetTransform = target.transform;
        var playerTransform = player.transform;

        playerTransform.parent = null;
        var playerOffset = playerTransform.position - startTransform.position;

        var t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            var startPos = startTransform.position + playerOffset;
            var targetPos = targetTransform.position;
            var newPos = Vector3.Lerp(startPos, targetPos, t);
            var rot = Quaternion.Lerp(startTransform.rotation, targetTransform.rotation, t);
            var up = (startTransform.up + targetTransform.up) * 0.5f;
            var yAddition = up * (jumpHeightCurve.Evaluate(t) * height);
            playerTransform.SetPositionAndRotation(newPos + yAddition, rot);
            yield return null;
        }
        player.SetKinematic(false);
        player.SetWalkableArea(target.area);
        coroutine = null;
    }

    private void OnDrawGizmos()
    {
        if (target == null)
        {
            return;
        }
        Gizmos.DrawLine(transform.position, target.transform.position);
    }

    private void OnValidate()
    {
        if (target == null)
        {
            return;
        }
        var targetJumpStart = target.GetComponent<JumpStartInteractable>();
        if (targetJumpStart == null)
        {
            return;
        }
        var thisTarget = GetComponent<JumpTarget>();
        if (thisTarget == null)
        {
            return;
        }
        targetJumpStart.target = thisTarget;
    }
}