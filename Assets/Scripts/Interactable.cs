using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public enum InputType
    {
        Interactable,
        Axis
    }

    IEnumerable AnimatorParameters
    {
        get
        {
            if (animator == null)
            {
                yield break;
            }

            for (var i = 0; i < animator.parameterCount; i++)
            {
                var param = animator.GetParameter(i);
                yield return new ValueDropdownItem<int>(param.name, param.nameHash);
            }
        }
    }

    [BoxGroup("Interaction")]
    public Animator animator;
    [BoxGroup("Interaction"), ValueDropdown(nameof(AnimatorParameters))]
    public int animatorParameter;
    [BoxGroup("Interaction")]
    public float interactionDelay = 0.5f;
    [BoxGroup("Interaction")]
    public bool needsInput = true;
    [BoxGroup("Interaction"), ShowIf(nameof(needsInput))]
    public InputType inputType;
    [BoxGroup("Interaction"), ShowIf(nameof(inputType), InputType.Axis)]
    public Vector2 axisInputNeeded;
    protected abstract bool CanInteract { get; }

    private bool pressedInteract;
    private float interactionTime;

    protected virtual void Update()
    {
        switch (inputType)
        {
            case InputType.Interactable:
                pressedInteract = PlayerInput.PressingAction;
                break;
            case InputType.Axis:
                pressedInteract = Vector2.Dot(PlayerInput.Directional, axisInputNeeded) > 0.5f;
                break;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (needsInput && !pressedInteract)
        {
            return;
        }
        if (Time.time < interactionTime)
        {
            return;
        }
        if (!CanInteract)
        {
            return;
        }
        interactionTime = Time.time + interactionDelay;
        var player = other.GetComponent<Player>();
        HandleInteraction(player);
        if (animator != null)
        {
            animator.SetTrigger(animatorParameter);
        }
    }

    protected abstract void HandleInteraction(Player player);
}