using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpStart : JumpTarget
{
    public JumpTarget target;
    public float duration = 0.5f;
    public AnimationCurve jumpHeightCurve;
    public float height = 1f;

    private Coroutine coroutine;
    private bool pressedJump;

    private void Update()
    {
        pressedJump = Input.GetButtonDown("Fire1");
    }

    private void OnTriggerStay(Collider other)
    {
        if (!pressedJump)
        {
            return;
        }
        if (coroutine != null)
        {
            return;
        }
        var player = other.GetComponent<Player>();
        if (player == null)
        {
            return;
        }
        coroutine = StartCoroutine(JumpAnimation(player));
    }

    private IEnumerator JumpAnimation(Player player)
    {
        player.SetJumping(true);
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
        player.SetJumping(false);
        player.SetWalkableArea(target.area);
        coroutine = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, target.transform.position);
    }
}